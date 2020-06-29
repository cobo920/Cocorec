using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Cocorec.Model
{
    class WaveFileFormat : IAudioFileFormat
    {
        public WaveFormat WaveFormat { get; }

        public WaveFileFormat(int sampleRate, int bits, int channels)
            => WaveFormat = new WaveFormat(sampleRate, bits, channels);

        public Stream CreateAudioFileWriter(string filename)
            => new WaveFileWriter(filename, WaveFormat);
    }
}
