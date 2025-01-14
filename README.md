NOTE: This implementation is Orleans-specific

# Why use Bois as a serializer?

https://github.com/salarcode/Bois 

1. Because it's fast and it's compact.
2. Because it has a native compression library (well, Lz4 at least)
3. Because if you use this to store your state, it's easy to deserialize outside of Orleans if you need to

# Why is LZ4 a better choice over Brotli compression?

(a) Because Lz4 is focussed on Speed, and is less CPU-intensive.
(b) We have lots of storage space in Azure, so the 'slightly worse' compression payload is less of an issue.
LZ4 comes with a Bois library directly

REF: https://stackoverflow.com/questions/37614410/comparison-between-lz4-vs-lz4-hc-vs-blosc-vs-snappy-vs-fastlz

# Anything to watch out for?

Yep, changing your class structure is not really supported


# STARTUP in services:

	BoisSerializerOptions bso = new BoisSerializerOptions();
	bso.UseCompression = true;

	.Services.AddSingleton<IGrainStorageSerializer>(serviceProvider => new BoisGrainStorageSerializer(bso));