using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Esthar.Data.Transform;

namespace Esthar
{
    /// <summary>
    /// Логика взаимодействия для LocationControlTextTabList.xaml
    /// </summary>
    public partial class LocationControlTextTabList : UserControl
    {
        public LocationTextControlContext Context { get; set; }

        public LocationControlTextTabList()
        {
            InitializeComponent();
        }

        private ListBox _listBoxInstance;
        private List<MessageWindow> _windows;
        private string _lastFilter;

        private void OnListLoaded(object sender, RoutedEventArgs e)
        {
            _listBoxInstance = (ListBox)sender;

            Style itemContainerStyle = new Style(typeof(ListBoxItem));
            itemContainerStyle.Setters.Add(new Setter(AllowDropProperty, true));
            itemContainerStyle.Setters.Add(new EventSetter(KeyUpEvent, new KeyEventHandler(OnKeyUpEvent)));
            itemContainerStyle.Setters.Add(new EventSetter(PreviewMouseMoveEvent, new MouseEventHandler(OnPreviewMouseMoveEvent)));
            itemContainerStyle.Setters.Add(new EventSetter(DropEvent, new DragEventHandler(OnListBoxDrop)));
            _listBoxInstance.ItemContainerStyle = itemContainerStyle;

            _listBoxInstance.SelectionChanged += OnSelectionChanged;
            Filter.FilterChanged += OnFilterChanged;
        }

        private void OnKeyUpEvent(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
                return;

            ListBoxItem item = sender as ListBoxItem;
            if (item == null)
                return;

            MessageWindow window = (MessageWindow)item.DataContext;
            if (window == null)
                return;

            window.IsIndent = !window.IsIndent;
            OnFilterChanged(_lastFilter);
        }

        private void OnPreviewMouseMoveEvent(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
                return;

            ListBoxItem draggedItem = sender as ListBoxItem;
            if (draggedItem == null)
                return;

            DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
        }

        private void OnListBoxDrop(object sender, DragEventArgs e)
        {
            MessageWindow droppedWindow = (MessageWindow)e.Data.GetData(typeof(MessageWindow));
            MessageWindow targetWindow = (MessageWindow)((ListBoxItem)sender).DataContext;

            if (droppedWindow == null || targetWindow == null || droppedWindow == targetWindow)
                return;

            int oldIndex = droppedWindow.Order;
            int newIndex = targetWindow.Order;
            if (targetWindow.Order < droppedWindow.Order)
                newIndex++;

            droppedWindow.Order = newIndex;
            
            if (newIndex < oldIndex)
            {
                foreach (MessageWindow window in _windows)
                {
                    if (window.Order >= newIndex && window.Order < oldIndex && window.Message != droppedWindow.Message)
                        window.Order++;
                }
            }
            else if (newIndex > oldIndex)
            {
                foreach (MessageWindow window in _windows)
                {
                    if (window.Order <= newIndex && window.Order > oldIndex && window.Message != droppedWindow.Message)
                        window.Order--;
                }
            }

            OnFilterChanged(_lastFilter);
        }

        private void OnFilterChanged(string filter)
        {
            _lastFilter = filter;
            if (_windows == null)
            {
                _listBoxInstance.ItemsSource = null;
                return;
            }

            IEnumerable<MessageWindow> windows = _windows.OrderBy(w => w.Order);
            if (!string.IsNullOrEmpty(filter))
                windows = windows.Where(wnd => wnd.Message.MatchFilter(filter));

            _listBoxInstance.ItemsSource = windows;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IList newItems = e.AddedItems;
            MessageWindow window = (MessageWindow)(newItems.Count > 0 ? newItems[0] : null);
            Context.ChangeWindow(window);
        }

        public void SetMessageWindows(List<MessageWindow> windows)
        {
            int order = 0;
            foreach (MessageWindow wnd in windows.OrderBy(w => w.Order))
                wnd.Order = order++;
            
            _windows = windows;
            OnFilterChanged(null);
        }
    }
}