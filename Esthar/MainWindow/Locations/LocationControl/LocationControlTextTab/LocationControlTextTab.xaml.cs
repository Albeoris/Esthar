using System;
using System.Windows;
using Esthar.Data.Transform;
using Esthar.UI;

namespace Esthar
{
    /// <summary>
    /// Логика взаимодействия для LocationControlTextTab.xaml
    /// </summary>
    public partial class LocationControlTextTab : IDisposable
    {
        private readonly LocationTextControlContext _context;
        private readonly LocationTextWatcher _watcher;

        public LocationControlTextTab()
        {
            InitializeComponent();
            _context = new LocationTextControlContext(_list, _edit, _preview);
            _watcher = new LocationTextWatcher(_context);
            DataContextChanged += OnDataContextChanged;
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                _context.ChangeLocation((Location)DataContext);
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }
    }
}