using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Lame;

namespace Cocorec.Model
{
    class Mp3FileFormat: IAudioFileFormat
    {
        public WaveFormat WaveFormat { get; }

        public Mp3FileFormat(int sampleRate, int bits, int channels)
            => WaveFormat = new WaveFormat(sampleRate, bits, channels);

        public Stream CreateAudioFileWriter(string filename)
            => new LameMP3FileWriter(filename, WaveFormat, LAMEPreset.V6);
    }
}
