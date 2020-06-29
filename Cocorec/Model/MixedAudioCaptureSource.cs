using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using System.Reactive.Linq;
using System.IO;

namespace Cocorec.Model
{
    class MixedAudioCaptureSource
    {
        protected IDisposable stopRecordingDisposable;

        public bool IsRecording { get => stopRecordingDisposable != null; }

        public IEnumerable<WaveInCapabilities> EnumerateCaptureDevices()
            => Enumerable.Range(0, WaveIn.DeviceCount)
                .Select(i => WaveIn.GetCapabilities(i))
                .Where(info => info.Channels == 2);

        public void StartRecording(IEnumerable<int> captureDevices, IAudioFileFormat format, string filename)
            => stopRecordingDisposable = CreateWritingAudioStream(captureDevices, format, filename).Subscribe();

        public void StopRecording()
        {
            stopRecordingDisposable.Dispose();
            stopRecordingDisposable = null;
        }

        IObservable<byte[]> CreateWritingAudioStream(IEnumerable<int> captureDevices, IAudioFileFormat format, string filename)
            => Observable.Using(() => format.CreateAudioFileWriter(filename),
                writer => CreateMixedCaptureStreamAsObservable(captureDevices, format.WaveFormat)
                    .Do(buffer => writer.Write(buffer, 0, buffer.Length))
                    .Finally(() => writer.Flush()));

        IObservable<byte[]> CreateMixedCaptureStreamAsObservable(IEnumerable<int> captureDevices, WaveFormat format)
            => captureDevices.Select(deviceNumber => CreateCaptureStreamAsObservable(deviceNumber, format))
                .Zip(buffers => CreateMixedAudioBuffer(buffers));

        byte[] CreateMixedAudioBuffer(IList<byte[]> buffers)
        {
            if (buffers.Select(buf => buf.Length).Distinct().Count() > 1)
                throw new InvalidDataException("Audio capture buffer size mismatch.");
            return Enumerable.Range(0, buffers[0].Length / 2)
                .Select(i => buffers.Select(buf => BitConverter.ToInt16(buf, i * 2)).Average(v => v))
                .Select(v => (short)v)
                .SelectMany(BitConverter.GetBytes)
                .ToArray();
        }

        IObservable<byte[]> CreateCaptureStreamAsObservable(int deviceNumber, WaveFormat format)
            => Observable.Using(() => new WaveIn
            {
                DeviceNumber = deviceNumber,
                WaveFormat = format
            },
            waveIn => waveIn.AsObservable().Select(e => e.Buffer));
    }
}
