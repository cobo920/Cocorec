using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Cocorec.Model
{
    static class WaveInExtensions
    {
        public static IObservable<WaveInEventArgs> DataAvailableAsObservable(this IWaveIn waveIn)
            => Observable.FromEvent<EventHandler<WaveInEventArgs>, WaveInEventArgs>(
                h => (sender, e) => h(e),
                h => waveIn.DataAvailable += h,
                h => waveIn.DataAvailable -= h);

        public static IObservable<WaveInEventArgs> AsObservable(this IWaveIn waveIn)
            => Observable.Create<WaveInEventArgs>(observer =>
            {
                var disposable = waveIn.DataAvailableAsObservable()
                    .Finally(() => waveIn.StopRecording())
                    .Subscribe(observer);
                waveIn.StartRecording();
                return disposable;
            });
    }
}
