namespace DelightfulCode.Orleans.Serialization
{
    /// <summary>
    /// Bois (because its so simple) by itself does not contain any 'options' in the same way that JsonSerializer has.
    /// NOTE: No support for encryption - if you need that then decrypt/encrypt as a separate thing.
    /// </summary>
    public class BoisSerializerOptions
    {
        /// <summary>
        /// Use Lz4 compression, using 'Salar.Bois.LZ4' library.
        /// Note this is an 'all or nothing' setting. However if you do change your mind later
        /// it is possible to use some BOIS routines to change the storage.
        /// </summary>
        public bool UseCompression { get; set; }

        /// <summary>
        /// Standard options for Bois serialization in this application.
        /// Uses compression but does not use encryption.
        /// </summary>
        public static readonly BoisSerializerOptions Basic = new BoisSerializerOptions
        {
            UseCompression = false,
        };
    }
}
