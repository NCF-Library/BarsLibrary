using CeadLibrary.IO;

namespace BarsLibrary.Core
{
    public class AudioMetaDataMarker
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int StartPos { get; set; }
        public int Length { get; set; }

        public AudioMetaDataMarker(CeadReader reader, long strgOffset)
        {
            Id = reader.ReadInt32();
            Name = reader.ReadObject(strgOffset + reader.ReadUInt32(), SeekOrigin.Current, () => reader.ReadString(StringType.Int32CharCount));
            StartPos = reader.ReadInt32();
            Length = reader.ReadInt32();
        }

        public void Write(CeadWriter writer)
        {
            writer.Write(Id);
            writer.Write(0U); // STRG offset
            writer.Write(StartPos);
            writer.Write(Length);
        }
    }
}
