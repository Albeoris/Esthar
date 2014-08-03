using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using Esthar.Core;
using Timer = System.Timers.Timer;

namespace Esthar.UI
{
    /// <summary>
    /// Логика взаимодействия для ProgressWindow.xaml
    /// </summary>
    public sealed partial class ProgressWindow
    {
        private readonly Timer _timer;

        private long _processedCount, _totalCount;
        private DateTime _begin;

        public ProgressWindow(string title)
        {
            Loaded += OnLoaded;
            Closing += OnClosing;

            _timer = new Timer(500);
            _timer.Elapsed += OnTimer;

            DataContext = this;
            InitializeComponent();

            _titleText.Text = title;
        }

        public void SetTotal(long totalCount)
        {
            Interlocked.Add(ref _totalCount, totalCount);
        }

        public void Increment(long processedCount)
        {
            Interlocked.Add(ref _processedCount, processedCount);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _begin = DateTime.Now;
            _timer.Start();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            _timer.Stop();
            _timer.Elapsed -= OnTimer;
        }

        private void OnTimer(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Dispatcher.Invoke(DispatcherPriority.DataBind, (Action)(UpdateProgress));
        }

        private void UpdateProgress()
        {
            _timer.Elapsed -= OnTimer;

            _progressBar.Maximum = _totalCount;
            _progressBar.Value = _processedCount;

            double percents = (_totalCount == 0) ? 0.0 : 100 * _processedCount / (double)_totalCount;
            TimeSpan elapsed = DateTime.Now - _begin;
            double speed = _processedCount / Math.Max(elapsed.TotalSeconds, 1);
            if (speed < 1) speed = 1;
            TimeSpan left = TimeSpan.FromSeconds((_totalCount - _processedCount) / speed);

            _progressText.Text = String.Format("{0:F2}%", percents);
            _beginTimeText.Text = String.Format("{1}: {0:mm\\:ss}", elapsed, Lang.MeasurementsElapsed);
            _processedText.Text = String.Format("{0} / {1}", _processedCount, _totalCount);
            _endTimeText.Text = String.Format("{1}: {0:mm\\:ss}", left, Lang.MeasurementsRemaining);

            _timer.Elapsed += OnTimer;
        }
    }
}