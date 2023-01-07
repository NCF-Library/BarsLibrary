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

        public Track() => Meta = new();

        public void Read(CeadReader reader)
        {
            // Delegate to a designated lib
            // only hold type-switch impl here
        }

        public new void Write(CeadWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
