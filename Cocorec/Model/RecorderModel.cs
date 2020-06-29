using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Cocorec.Properties;
using System.Diagnostics;
using NAudio.Wave;

namespace Cocorec.Model
{
    class RecorderModel
    {
        MixedAudioCaptureSource captureSource;
        string saveDirPath;
        string recordingFilename;
        public bool IsRecording { get => captureSource.IsRecording; }
        public IEnumerable<string> EnumerateRecordingDevices()
            => captureSource.EnumerateCaptureDevices()
                .Select(info => info.ProductName);

        public RecorderModel()
        {
            captureSource = new MixedAudioCaptureSource();
            saveDirPath = NormalizeSaveDirPath(Settings.Default.SaveDirectory);
        }

        string GetSaveFilename() => $"{DateTime.Now: yyyy-MM-dd_HH-mm-ss-fff}.mp3";

        string NormalizeSaveDirPath(string savedir)
        {
            var specialDir = Enum.GetValues(typeof(Environment.SpecialFolder))
                .Cast<Environment.SpecialFolder?>()
                .First(d => savedir.StartsWith($"{{{d}}}"));

            if (specialDir == null)
                return Path.GetFullPath(savedir);
            string specialDirName = specialDir.ToString();
            string specialDirPath = Environment.GetFolderPath(specialDir.Value);
            if (savedir.Length > specialDirName.Length + 3)
            {
                string followingPath = savedir.Substring(specialDirName.Length + 3);
                return Path.Combine(specialDirPath, followingPath);
            }
            return specialDirPath;
        }

        public void StartRecording(IEnumerable<int> captureDevices)
        {
            Directory.CreateDirectory(saveDirPath);
            recordingFilename = Path.Combine(saveDirPath, GetSaveFilename());
            captureSource.StartRecording(captureDevices, new Mp3FileFormat(44100, 16, 2), recordingFilename);
        }

        public void StopRecording() => StopRecording(false);

        public void StopRecording(bool showRecordedFile)
        {
            captureSource.StopRecording();
            if (showRecordedFile)
                ShowRecordedFile(recordingFilename);
            recordingFilename = null;
        }

        public void ShowRecordedFile(string filename)
            => Process.Start(new ProcessStartInfo()
            {
                FileName = "explorer",
                Arguments = $"/e, /select, \"{recordingFilename}\""
            });

    }
}
