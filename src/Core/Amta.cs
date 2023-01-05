using CeadLibrary.Generics;
using CeadLibrary.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class Amta : ICeadObject
    {
        public string Name { get; set; }
        public TrackType TrackType { get; set; }
        public int WaveChannelCount { get; set; }
        public int StreamTrackCount { get; set; }
        public int Flags { get; set; }
        public float Volume { get; set; }
        public int SamplerRate { get; set; }
        public int LoopStart { get; set; }
        public int LoopEnd { get; set; }
        public int Loudness { get; set; }
        public List<(int ChannelCount, int Volume)> StreamTracks { get; set; } = new();

        public ICeadObject Read(CeadReader reader)
        {
            reader.CheckMagic("AMTA"u8);
            reader.Endian = (Endian)reader.ReadInt16();
            reader.Seek(sizeof(short), SeekOrigin.Current);

            // Skip unused offsets (?)
            // reader.Seek(sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int), SeekOrigin.Current);
            // 
            // int amtaSize = reader.ReadInt32();
            // int dataOffset = reader.ReadInt32();
            // int markOffset = reader.ReadInt32();
            // int extOffset = reader.ReadInt32();
            // int strgOffset = reader.ReadInt32();

            reader.CheckMagic("DATA"u8);
            long unknown = reader.ReadInt64();
            TrackType = (TrackType)reader.ReadByte();
            WaveChannelCount = reader.ReadByte();
            StreamTrackCount = reader.ReadByte();
            Flags = reader.ReadByte();
            Volume = reader.ReadSingle();
            SamplerRate = reader.ReadInt32();
            LoopStart = reader.ReadInt32();
            LoopEnd = reader.ReadInt32();
            Loudness = reader.ReadInt32();
            StreamTracks = reader.ReadObjects(8, 0, SeekOrigin.Current, () => (reader.ReadInt32(), reader.ReadInt32())).ToList();
            reader.Seek(sizeof(int), SeekOrigin.Current); // unknwon

            reader.CheckMagic("MARK"u8);
            reader.Seek(sizeof(long), SeekOrigin.Current); // unknwon

            reader.CheckMagic("EXT_"u8);
            reader.Seek(sizeof(long), SeekOrigin.Current); // unknwon

            reader.CheckMagic("STRG"u8);
            Name = reader.ReadString(StringType.Int32CharCount);

            return this;
        }

        public void Write(CeadWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
