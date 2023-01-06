using CeadLibrary.Generics;
using CeadLibrary.IO;

namespace BarsLibrary.Core
{
    public enum SoundEncoding : byte
    {
        PCM8 = 0,
        PCM16 = 1,
        DSP_ADPCM = 2,
        IMA_ADPCM = 3,
    }

    public class Track
    {
        public SoundEncoding Encoding { get; set; }
        public bool IsLooped { get; set; }
        public int SampleRate { get; set; }
        public uint LoopStartInFrames { get; set; }
        public uint LoopEndInFrames { get; set; }
        public uint LoopStartFrame { get; set; }
        public AudioMetaData Meta { get; set; }
        public uint Version { get; set; }

        public Track()
        {
            Meta = new();
        }

        public void Read(CeadReader reader)
        {
            reader.CheckMagic("FWAV"u8); // Are other data containers supported?
            reader.ReadByteOrderMark();
            reader.Seek(sizeof(short), SeekOrigin.Current); // Block size (0x40)
            Version = reader.ReadUInt32();

            // FileSize + SectionCount + Padding +
            // InfoFlag + Padding + InfoOffset + InfoSize +
            // DataFlag + Padding + DataOffset + DataSize + Padding
            reader.Seek(
                sizeof(uint) + sizeof(short) + 2 +
                sizeof(short) + 2 + sizeof(uint) + sizeof(uint) +
                sizeof(short) + 2 + sizeof(uint) + sizeof(uint) + 0x14,
                SeekOrigin.Current);

            reader.CheckMagic("INFO"u8);
            reader.Seek(sizeof(uint), SeekOrigin.Current); // Block size
            Encoding = (SoundEncoding)reader.ReadByte();
            IsLooped = reader.ReadBool(BoolType.Byte);
            reader.Seek(2, SeekOrigin.Current); // Padding
            SampleRate = reader.ReadInt32();
            LoopStartInFrames = reader.ReadUInt32();
            LoopEndInFrames = reader.ReadUInt32();
            LoopStartFrame = reader.ReadUInt32();

            // Read ref table
            long relOffset = reader.BaseStream.Position;
            (ushort Flag, uint RelOffset)[] refs = new (ushort Flag, uint RelOffset)[reader.ReadUInt32()];
            for (int i = 0; i < refs.Length; i++) {
                ushort flag = reader.ReadUInt16();
                reader.Seek(2, SeekOrigin.Current); // Padding
                refs[i] = (flag, reader.ReadUInt32());
            }
        }

        public new void Write(CeadWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
