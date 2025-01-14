using Salar.Bois;
using Salar.Bois.LZ4;

namespace DelightfulCode
{
    /// <summary>
    /// Bois serialization is fantastic. https://github.com/salarcode/Bois
    /// It's not async, it's super-fast, so much faster than BinaryFormatting
    /// </summary>
    [Author("Warren James", 2022 - 2025)]
    [Health(CodeStability.SuperStable)]
    public static class Archiver
    {
        /// <summary>
        /// Serialize using BOIS, with optional compression using LZ4.
        /// NOTE: If you do compress you need to keep track of that flag so you can use the same for Deserialize.
        /// </summary>
        /// <param name="payload">the object you want to serizile ready for storage or transmission</param>
        /// <param name="compress">Optional compression ... uses LZ4 which is the one chosen by Salar.BOIS</param>
        public static byte[] Serialize(object payload, bool compress)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload), "Payload cannot be null.");

            byte[] output_bytes;

            // STEP 1: Get the serialized version of the object, in bytes

            // NOTE: Bois does not require the [Serialized] attribute to work with objects :)
            // NOTE: Bois needs an empty constructor for the object, to do its work correctly
            // NOTE: Bois can serialize most random objects but can't do polymorphic ones
            // NOTE: Bois may get an extra speed boost using the 'Initialize<MyClass>', but it's not mandatory.
            // NOTE: Bois can perform direct serialise from FileStream as well (see their documentation) IF NEEDED.

            if (compress == true)
            {
                BoisLz4Serializer boisLz4Serializer = new BoisLz4Serializer();
                using (MemoryStream ms = new MemoryStream())
                {
                    boisLz4Serializer.Pickle(payload, ms);
                    output_bytes = ms.ToArray();
                }
            }
            else
            {
                BoisSerializer bois = new BoisSerializer();
                Type payloadType = payload.GetType();

                using (MemoryStream ms = new MemoryStream())
                {
                    bois.Serialize(payload, payloadType, ms);
                    output_bytes = ms.ToArray();
                }
            }

            return output_bytes;
        }

        /// <summary>
        /// Takes some bytes and re-hydrates it back into an object of the specified type,
        /// uses the Bois Serilizer to do so, optinally decompressing (using Lz4) along the way.
        /// NOTE: it's up to the caller to keep track of (a) the type, (b) whether the item is actually compressed or not!
        /// </summary>
        /// <param name="bytes">the payload you wish to turn back into the requested thing</param>
        /// <param name="decompress">force decompression if known to be compressed (BOIS uses Lz4)</param>
        public static T Deserialize<T>(byte[] bytes, bool decompress)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes), "Bytes cannot be null.");

            using (MemoryStream data = new MemoryStream(bytes))
            {
                if (decompress == true)
                {
                    BoisLz4Serializer boisLz4Serializer = new BoisLz4Serializer();
                    return boisLz4Serializer.Unpickle<T>(data);
                }
                else
                {
                    BoisSerializer bois = new BoisSerializer();
                    return bois.Deserialize<T>(data);
                }
            }
        }

        public static T DeserializeFromFile<T>(FileInfo file_to_loadfrom, bool decompress = true)
        {
            if (!file_to_loadfrom.Exists) throw new System.Exception($"Expected archive File not found: {file_to_loadfrom.FullName} please verify ..");

            byte[] output_bytes = File.ReadAllBytes(file_to_loadfrom.FullName); // open the file and grab all the bytes[] content

            return Deserialize<T>(output_bytes, decompress);
        }

        /// <summary>
        /// Write a file locally ...
        /// </summary>
        public static async Task SerializeToFileAsync(FileInfo file_to_overwrite, object object_to_store, bool compress = true, bool protective = false)
        {
            byte[] data_to_commit_to_storage = Serialize(object_to_store, compress);

            if (protective && file_to_overwrite.Exists)
            {
                // rename the current file first, so we can preserve it
                // rename the extension
                File.Move(file_to_overwrite.FullName, Path.ChangeExtension(file_to_overwrite.FullName, $".old_{DateTime.Now:yyyyMMddHHmmssff}"));
            }

            await File.WriteAllBytesAsync(file_to_overwrite.FullName, data_to_commit_to_storage);
        }
    }
}
