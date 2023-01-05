using CeadLibrary.IO;

namespace BarsLibrary.Core
{
    public class BarsBase
    {
        private readonly int[] UnknownTrackData;

        public Endian Endian { get; set; } = BitConverter.IsLittleEndian ? Endian.Little : Endian.Big;
        public List<Amta> MetaData { get; set; }

        public BarsBase(CeadReader reader)
        {
            reader.CheckMagic("BARS"u8);
            int fileSize = reader.ReadInt32();

            // Read BOM
            Endian = (Endian)reader.ReadInt16();
            reader.Endian = Endian;

            // Skip unknown data
            reader.Seek(sizeof(short), SeekOrigin.Current); // always 0x0101 in botw
            int trackCount = reader.ReadInt32();
            UnknownTrackData = reader.ReadObjects(trackCount, 0, SeekOrigin.Current, reader.ReadInt32);

            // Read offsets
            int[] offsets = reader.ReadObjects(trackCount, 0, SeekOrigin.Current, reader.ReadInt32);

            // Read AMTA headers (sequentially)
            MetaData = reader.ReadObjects<Amta>(trackCount, 0, SeekOrigin.Current).ToList();

            // Read tracks (sequentially?)
            // ...

            // Reset endian after reading AMTA blocks
            reader.Endian = Endian;
        }
    }
}
