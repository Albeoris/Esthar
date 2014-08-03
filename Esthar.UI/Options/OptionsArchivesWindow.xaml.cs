using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Esthar.Core;
using Esthar.Data;
using Esthar.OpenGL;

namespace Esthar.UI
{
    /// <summary>
    /// Логика взаимодействия для OptionsPathsControl.xaml
    /// </summary>
    public partial class OptionsArchivesWindow
    {
        private readonly DispatcherOperation _loadingTask;

        public OptionsArchivesWindow()
        {
            InitializeComponent();
            Subscribe();

            _loadingTask = Dispatcher.InvokeAsync(LoadOptions);
        }

        private void Subscribe()
        {
            ContinueButton.Click += OnContinueButtonClick;
            ArchivesList.OptimizeButton.Click += OnOptimizeButtonClick;
        }

        private void OnOptimizeButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ArchivesList.OptimizeButton.IsEnabled = false;
                ContinueButton.IsEnabled = false;

                List<Task> tasks = new List<Task>();

                ObservableCollection<ArchiveInformationView> archivesCollection = Dispatcher.Invoke(() => ArchivesList.Archives);
                foreach (ArchiveInformationView view in archivesCollection)
                {
                    if (!view.IsSelected)
                        continue;

                    tasks.Add(Task.Run(() => Optimize(view)));
                }

                Task.Run(() => WaitingForComplete(tasks));
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        private void Optimize(ArchiveInformationView view)
        {
            ArchiveOptimizer optimizer = null;
            ArchiveInformation newInfo = null;

            Dispatcher.Invoke(() =>
            {
                newInfo = view.Info;
                optimizer = new ArchiveOptimizer(view.Info);
                view.StartProgress();
                optimizer.Progress += view.OnProgress;
            });

            try
            {
                newInfo = optimizer.Optimize();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }

            Dispatcher.Invoke(() =>
            {
                view.Info = newInfo;
                view.StopProgress();
            });
        }

        private void WaitingForComplete(List<Task> tasks)
        {
            try
            {
                Task.WaitAll(tasks.ToArray());

                Dispatcher.Invoke(() =>
                {
                    Check();
                    ContinueButton.IsEnabled = true;
                    ArchivesList.OptimizeButton.IsEnabled = true;
                });
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        private void LoadOptions()
        {
            try
            {
                ContinueButton.IsEnabled = false;

                ObservableCollection<ArchiveInformationView> archiveCollection = ArchivesList.Archives;
                foreach (string archivePath in Options.GetGameArchivesPaths())
                {
                    ArchiveInformationAccessor accessor = new ArchiveInformationAccessor(archivePath);
                    ArchiveInformation info = accessor.ReadOrCreate();
                    ArchiveInformationView view = new ArchiveInformationView(info);
                    archiveCollection.Add(view);
                }

                ReserveControl.AbsoluteValue = Options.AbsoluteReserve;
                ReserveControl.RelativeValue = Options.RelativeReserve;

                ContinueButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        private void SaveOptions()
        {
            try
            {
                Options.AbsoluteReserve = ReserveControl.AbsoluteValue;
                Options.RelativeReserve = ReserveControl.RelativeValue;

                Options.Save();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        public bool Check()
        {
            _loadingTask.Wait();

            ObservableCollection<ArchiveInformationView> archiveCollection = ArchivesList.Archives;
            foreach (ArchiveInformationView view in archiveCollection)
            {
                if (view.Info.IsOptimized)
                    continue;

                ValidationHelper.SetInvalid(ArchivesList, new ValidationResult(false, "Не все архивы оптимизированы"));
                return false;
            }

            Archives.Initialize(ArchivesList.Archives.Select(view => view.Info).ToArray());
            return true;
        }

        private void OnContinueButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Check())
                {
                    SaveOptions();
                    DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }
    }
}