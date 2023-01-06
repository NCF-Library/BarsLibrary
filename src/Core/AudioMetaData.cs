using CeadLibrary.Generics;
using CeadLibrary.IO;

namespace BarsLibrary.Core
{
    public enum TrackType : byte
    {
        Wave = 0,
        Stream = 1
    }

    /// <summary>
    /// <b>A</b>udio <b>M</b>eta Da<b>ta</b> <see cref="ICeadObject"/> structure
    /// </summary>
    public class AudioMetaData
    {
        public string Name { get; set; } = string.Empty;
        public TrackType TrackType { get; set; }
        public int WaveChannelCount { get; set; }
        public int StreamTrackCount { get; set; }
        public int Flags { get; set; }
        public float Duration { get; set; }
        public int SamplerRate { get; set; }
        public int LoopStart { get; set; }
        public int LoopEnd { get; set; }
        public float Loudness { get; set; }
        public float? AmplitudePeak { get; set; }
        public List<(int ChannelCount, float Volume)> StreamTracks { get; set; } = new();
        public List<AudioMetaDataMarker> Markers { get; set; } = new();
        public List<(int Unknown1, int Unknown2)> ExtEntries { get; set; } = new();
        public int Version { get; set; }

        public AudioMetaData() { }
        public AudioMetaData(CeadReader reader)
        {
            long amtaHeaderStart = reader.BaseStream.Position;

            reader.CheckMagic("AMTA"u8);
            reader.ReadByteOrderMark();
            Version = reader.ReadByte();
            reader.Seek(1 + 4 * sizeof(int), SeekOrigin.Current); // byte padding + unused offsets
            long strgOffset = amtaHeaderStart + reader.ReadInt32() + 4;

            reader.CheckMagic("DATA"u8);
            reader.Seek(sizeof(int), SeekOrigin.Current); // section size
            uint offset = reader.ReadUInt32();
            Name = reader.ReadObject(strgOffset + offset, SeekOrigin.Begin, () => reader.ReadString(StringType.Int32CharCount));
            reader.Seek(sizeof(int), SeekOrigin.Current); // unknown
            TrackType = (TrackType)reader.ReadByte();
            WaveChannelCount = reader.ReadByte();
            StreamTrackCount = reader.ReadByte();
            Flags = reader.ReadByte();
            Duration = reader.ReadSingle();
            SamplerRate = reader.ReadInt32();
            LoopStart = reader.ReadInt32();
            LoopEnd = reader.ReadInt32();
            Loudness = reader.ReadSingle();
            StreamTracks = reader.ReadObjects(8, () => (reader.ReadInt32(), reader.ReadSingle())).ToList();
            AmplitudePeak = Version == 4 ? reader.ReadSingle() : null;

            reader.CheckMagic("MARK"u8);
            reader.Seek(sizeof(int), SeekOrigin.Current);
            int markCount = reader.ReadInt32();
            Markers = reader.ReadObjects(markCount, () => new AudioMetaDataMarker(reader, strgOffset)).ToList();

            reader.CheckMagic("EXT_"u8);
            int extCount = reader.ReadInt32();
            ExtEntries = reader.ReadObjects(extCount, () => (reader.ReadInt32(), reader.ReadInt32())).ToList();
        }

        public void Write(CeadWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
