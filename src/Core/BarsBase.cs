using CeadLibrary.IO;

namespace BarsLibrary.Core
{
    public class BarsBase : Dictionary<string, Track>
    {
        public Endian Endian { get; set; } = BitConverter.IsLittleEndian ? Endian.Little : Endian.Big;

        public BarsBase(CeadReader reader)
        {
            reader.CheckMagic("BARS"u8);
            reader.Seek(sizeof(uint), SeekOrigin.Current); // File size
            Endian = reader.ReadByteOrderMark();
            reader.Seek(sizeof(short), SeekOrigin.Current); // Block size (0x0100)
            int trackCount = reader.ReadInt32();
            reader.Seek(trackCount * sizeof(uint), SeekOrigin.Current); // Hashed amta names for binary searches
            uint[] offsets = reader.ReadObjects(trackCount * 2, reader.ReadUInt32); // Meta Data and Track offsets

            // Read audio meta data
            for (int i = 0; i < trackCount; i++) {
                reader.Seek(offsets[i * 2], SeekOrigin.Begin);
                Track track = new() {
                    Meta = new(reader)
                };
                Add(track.Meta.Name, track);

                // Reset reader endian (theoretically could
                // be switch while reading the amta)
                reader.Endian = Endian;
            }

            // Read tracks
            int index = 0;
            foreach ((var _, Track track) in this) {
                long offset = offsets[index * 2 + 1];
                if (offset == -1) {
                    continue; // Meta data only
                }

                reader.Seek(offset, SeekOrigin.Begin);
                track.Read(reader);
                index++;
            }
        }
    }
}
