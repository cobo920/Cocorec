using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using Cocorec.Model;
using System.Reactive.Disposables;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Collections.ObjectModel;
using Cocorec.Properties;
using System.Text.RegularExpressions;

namespace Cocorec.ViewModel
{
    class RecorderViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        CompositeDisposable Disposables { get; } = new CompositeDisposable();
        Subject<bool> isRecordingSubject;
        public ReactiveCommand StartRecordingCommand { get; }
        public ReactiveCommand<bool> StopRecordingCommand { get; }
        public ReactiveCommand<bool> ToggleRecordingCommand { get; }
        public ReactiveProperty<bool> IsRecording { get; }
        public ReactiveProperty<bool> IsNotRecording { get; }

        public ObservableCollection<RecordingDevice> RecordingDevices { get; }

        RecorderModel model;

        public RecorderViewModel(RecorderModel model)
        {
            this.model = model;
            isRecordingSubject = new Subject<bool>().AddTo(Disposables);
            // IsRecording = isRecordingSubject.ToReadOnlyReactiveProperty().AddTo(Disposables);
            // IsNotRecording = isRecordingSubject.Select(p => !p).ToReadOnlyReactiveProperty().AddTo(Disposables);
            IsRecording = new ReactiveProperty<bool>(isRecordingSubject).AddTo(Disposables);
            IsNotRecording = new ReactiveProperty<bool>(isRecordingSubject.Select(p => !p)).AddTo(Disposables);
            StartRecordingCommand = new ReactiveCommand(isRecordingSubject.Select(p => !p)).AddTo(Disposables);
            StopRecordingCommand = new ReactiveCommand<bool>(isRecordingSubject, false).AddTo(Disposables);
            ToggleRecordingCommand = new ReactiveCommand<bool>().AddTo(Disposables);

            var deviceSearchPatterns = Settings.Default.AudioDeviceSearchPatterns
                .Cast<string>()
                .Select(pattern => new Regex(pattern, RegexOptions.Compiled))
                .ToList();

            RecordingDevices = new ObservableCollection<RecordingDevice>(
                model.EnumerateRecordingDevices()
                .Select((name, i) => new RecordingDevice(i, name,
                    deviceSearchPatterns.Any(pattern => pattern.IsMatch(name)
                    )))
                );

            StartRecordingCommand.Subscribe(_ => StartRecording());
            StopRecordingCommand.Subscribe(showFile => StopRecording(showFile));

            ToggleRecordingCommand.Subscribe(showFile =>
            {
                if (model.IsRecording)
                    StopRecording(showFile);
                else
                    StartRecording();

            });
        }

        void StartRecording()
        {
            isRecordingSubject.OnNext(true);
            model.StartRecording(RecordingDevices
                .Where(device => device.ShouldRecord)
                .Select(device => device.Number)
            );
        }

        void StopRecording(bool showFile)
        {
            model.StopRecording(showFile);
            isRecordingSubject.OnNext(false);
        }

        public void Dispose() => Dispose();
    }
}
