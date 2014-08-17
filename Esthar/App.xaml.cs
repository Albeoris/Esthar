using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Esthar.Core;
using Esthar.UI;

namespace Esthar
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App
    {
        private void Initialize()
        {
            bool shiftPressed = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            Log.Message("Инициализация приложения. Перенастройка: {0}", shiftPressed);
            try
            {
                Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                CheckOptionsPaths(shiftPressed);
                CheckOptionsArchives(shiftPressed);
                ShowMainWindow();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при инициализации приложения.");
                Current.Shutdown();
            }
        }

        private void ShowMainWindow()
        {
            MainWindow window = new MainWindow();
            Current.MainWindow = window;
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            window.Show();
        }

        private void CheckOptionsPaths(bool shiftPressed)
        {
            OptionsPathsWindow window = new OptionsPathsWindow();
            Current.MainWindow = window;
            if ((shiftPressed || !window.Check()) && window.ShowDialog() != true)
                throw new OperationCanceledException();
        }

        private void CheckOptionsArchives(bool shiftPressed)
        {
            OptionsArchivesWindow window = new OptionsArchivesWindow();
            Current.MainWindow = window;
            if ((shiftPressed || !window.Check()) && window.ShowDialog() != true)
                throw new OperationCanceledException();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Current.Exit += OnExit;

            if (IsAdministrator())
            {
                Initialize();
                base.OnStartup(e);
                return;
            }

            var exeName = Process.GetCurrentProcess().MainModule.FileName;
            ProcessStartInfo startInfo = new ProcessStartInfo(exeName) { Verb = "runas" };
            Process.Start(startInfo);
            Current.Shutdown();
        }

        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            UIHelper.ShowError((Exception)e.ExceptionObject, "Unhandled exception.");
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            UIHelper.ShowError(e.Exception, "Unhandled exception.");
            e.Handled = true;
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            DisposableSerivce.DisposeAll();
        }
    }
}