using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cocorec.Model
{
    interface IAudioFileFormat
    {
        WaveFormat WaveFormat { get; }
        Stream CreateAudioFileWriter(string filename);
    }
}
