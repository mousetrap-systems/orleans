NOTE: This implementation is Orleans-specific
NOTE: It's a very rough version, sorry about that. Built originally for proof-of concept and testing comparisons.

# Why use Bois as a serializer?

read about the library here:
https://github.com/salarcode/Bois 

1. Because it's fast and it's compact, and it's ELEGANT.
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

# WHY USEFUL ... DEPENDING ON YOUR USE CASE OF COURSE ... for preserving state in Orleans:

MAYBE:
1. You need, and can benefit from, compression (e.g. longer term storage, 'not very hot' state).
2. You don't have direct control over the classes that you're serializing and you don't wish to subclass it.

CAVEATS:
- This serializer isn't (intended to be) version-forgiving (like the Orleans Native serializer), this means if your class definition changes then the stuff in storage needs conversion.
- Binary data is only safe if you have full control and security on your data write/consume, of course.

UPSIDES:
(a) The POCO's don't require special decoration.
(b) It's easy to deserialize and read the state from the file outside of the Orleans grain context, e.g. if you're processing last years' data archive in a separate system.
(c) Wide range of objects and data primitives supported.
(d) The Bois serializer is pretty fast compared with others in its class - this matters if you have a massive processing workload.
(e) Pretty good 'serialized size', even better with the Lz4 option.