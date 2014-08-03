using System;
using System.IO;
using System.Windows;
using Esthar.Core;
using Esthar.Data;

namespace Esthar.UI
{
    public sealed class ArchiveInformationView : DependencyObject, IComparable<ArchiveInformationView>, IComparable
    {
        public ArchiveInformationView(ArchiveInformation info)
        {
            Exceptions.CheckArgumentNull(info, "info");

            Info = info;
            Refresh();
        }

        public void Refresh()
        {
            DisplayName = Path.GetFileNameWithoutExtension(Info.FilePath);
            State = Info.IsOptimized ? "Оптимизирован" : "Сжат";
        }

        public static readonly DependencyProperty InfoProperty = DependencyProperty.Register("Info", typeof(ArchiveInformation), typeof(ArchiveInformationView), new PropertyMetadata(default(ArchiveInformation)));
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(object), typeof(ArchiveInformationView), new PropertyMetadata(default(object)));
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(ArchiveInformationView), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register("DisplayName", typeof(string), typeof(ArchiveInformationView), new PropertyMetadata(default(string)));

        public ArchiveInformation Info
        {
            get { return (ArchiveInformation)GetValue(InfoProperty); }
            set { SetValue(InfoProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        public object State
        {
            get { return (object)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public int CompareTo(ArchiveInformationView other)
        {
            return other == null ? 1 : (DisplayName.CompareTo(other.DisplayName));
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as ArchiveInformationView);
        }

        public void StartProgress()
        {
            ProgressControl progressControl = new ProgressControl();
            progressControl.Begin();
            State = progressControl;
        }

        public void StopProgress()
        {
            ((ProgressControl)State).End();
            State = Info.IsOptimized ? "Оптимизирован" : "Сжат";
        }

        public void OnProgress(object sender, ProgressArgs e)
        {
            Dispatcher.Invoke(() => ((ProgressControl)State).UpdateSize(e.Key, e.ProcessedSize, e.TotalSize));
        }
    }
}