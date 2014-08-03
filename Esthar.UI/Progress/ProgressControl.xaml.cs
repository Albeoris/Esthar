using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using Esthar.Core;
using Timer = System.Timers.Timer;

namespace Esthar.UI
{
    public partial class ProgressControl
    {
        private readonly ConcurrentDictionary<object, ProgressEntry> _entries;
        private readonly Timer _timer;

        private long _processedSize, _totalSize;
        private DateTime _begin;

        public ProgressControl()
        {
            _entries = new ConcurrentDictionary<object, ProgressEntry>();
            _timer = new Timer(500);

            DataContext = this;
            InitializeComponent();
        }

        public void Begin()
        {
            _begin = DateTime.Now;
            _timer.Elapsed += OnTimer;
            _timer.Start();
            
            Visibility = Visibility.Visible;
        }

        public void End()
        {
            _timer.Stop();
            _timer.Elapsed -= OnTimer;

            Reset();
        }

        private void Reset()
        {
            Visibility = Visibility.Collapsed;

            _entries.Clear();
            _processedSize = 0;
            _totalSize = 1;

            _progressBar.Maximum = 0;
            _progressBar.Value = 0;
            _progressText.Text = "0 %";
            _beginTimeText.Text = Lang.MeasurementsElapsed + ": 00:00";
            _speedText.Text = String.Format("0 {0} / {1}", Lang.MeasurementsKByteAbbr, Lang.MeasurementsSecondAbbr);
            _endTimeText.Text = Lang.MeasurementsRemaining + ": 00:00";
        }

        public void UpdateSize(object key, long processedSize, long totalSize)
        {
            ProgressEntry entry = _entries.GetOrAdd(key, k => new ProgressEntry(k));
            Interlocked.Add(ref _processedSize, entry.UpdateProcessedSize(processedSize));
            Interlocked.Add(ref _totalSize, entry.UpdateTotalSize(totalSize));
        }

        private void OnTimer(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Dispatcher.Invoke(DispatcherPriority.DataBind, (Action)(UpdateProgress));
        }

        private void UpdateProgress()
        {
            _timer.Elapsed -= OnTimer;

            _progressBar.Maximum = _totalSize;
            _progressBar.Value = _processedSize;

            double percents = (_totalSize == 0) ? 0.0 : 100 * _processedSize / (double)_totalSize;
            TimeSpan elapsed = DateTime.Now - _begin;
            double speed = _processedSize / Math.Max(elapsed.TotalSeconds, 1);
            if (speed < 1) speed = 1;
            TimeSpan left = TimeSpan.FromSeconds((_totalSize - _processedSize) / speed);

            _progressText.Text = String.Format("{0:F2}%", percents);
            _beginTimeText.Text = String.Format("{1}: {0:mm\\:ss}", elapsed, Lang.MeasurementsElapsed);
            _speedText.Text = FormatHelper.BytesFormat(speed) + " / " + Lang.MeasurementsSecondAbbr;
            _endTimeText.Text = String.Format("{1}: {0:mm\\:ss}", left, Lang.MeasurementsRemaining);

            _timer.Elapsed += OnTimer;
        }
    }
}