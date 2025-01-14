using Microsoft.Extensions.Options;
using Orleans.Storage;
using Salar.Bois;
using Salar.Bois.LZ4;

namespace DelightfulCode.Orleans.Serialization
{
    /// <summary>
    /// Grain storage serializer that uses the Salar.Bois <see cref="BoisSerializer"/>.
    /// </summary>
    public class BoisGrainStorageSerializer : IGrainStorageSerializer
    {
        private readonly BoisSerializer boisSerializer;
        private readonly BoisLz4Serializer boisLz4Serializer;
        private readonly BoisSerializerOptions settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoisGrainStorageSerializer"/> class.
        /// This pattern is used by all the other implementations.
        /// </summary>
        public BoisGrainStorageSerializer(IOptions<BoisSerializerOptions> options)
        {
            this.boisSerializer = new BoisSerializer();
            this.boisLz4Serializer = new BoisLz4Serializer();
            this.settings = options.Value;
        }

        // Constructor that accepts BoisSerializerOptions directly, this is our version.
        public BoisGrainStorageSerializer(BoisSerializerOptions options)
        {
            this.boisSerializer = new BoisSerializer();
            this.boisLz4Serializer = new BoisLz4Serializer();
            this.settings = options;
        }

        /// <inheritdoc/>
        public BinaryData Serialize<T>(T payload)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                if (settings.UseCompression == true)
                {
                    boisLz4Serializer.Pickle(payload, ms);
                }
                else
                {
                    boisSerializer.Serialize(payload, ms);
                }

                return new BinaryData(ms.ToArray());
            }
        }

        /// <inheritdoc/>
        public T Deserialize<T>(BinaryData input)
        {
            using (MemoryStream data = new MemoryStream(input.ToArray()))
            {
                if (settings.UseCompression == true)
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
    }
}
