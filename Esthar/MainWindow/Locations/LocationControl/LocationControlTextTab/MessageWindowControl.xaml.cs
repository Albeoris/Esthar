using System.Windows;
using Esthar.Data.Transform;

namespace Esthar
{
    public partial class MessageWindowControl
    {
        public LocationTextControlContext Context { get; set; }
        public MessageWindow MessageWindow { get; set; }

        private bool _initialization;

        public MessageWindowControl()
        {
            InitializeComponent();
            Subscribe();
        }

        private void Subscribe()
        {
            WindowXControl.ValueChanged += WindowXControlValueChanged;
            WindowYControl.ValueChanged += WindowYControlValueChanged;
            WindowFirstAnswerControl.ValueChanged += WindowFirstAnswerControlChanged;
            WindowLastAnswerControl.ValueChanged += WindowLastAnswerControlChanged;
            WindowDefaultAnswerControl.ValueChanged += WindowDefaultAnswerControlChanged;
            WindowCancelAnswerControl.ValueChanged += WindowCancelAnswerControlChanged;
        }

        private void WindowXControlValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (MessageWindow == null || _initialization)
                return;

            MessageWindow.X = (int)e.NewValue / 2;
            Context.SaveRequest(LocationProperty.Scripts);
        }

        private void WindowYControlValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (MessageWindow == null || _initialization)
                return;

            MessageWindow.Y = (int)e.NewValue / 2;
            Context.SaveRequest(LocationProperty.Scripts);
        }

        private void WindowFirstAnswerControlChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (MessageWindow == null || _initialization)
                return;

            MessageWindow.FirstAnswerLine = (int)e.NewValue;
            if (e.NewValue == null || e.OldValue == null)
                return;

            Context.UpdateTrigger.Set();
            Context.SaveRequest(LocationProperty.Scripts);
        }

        private void WindowLastAnswerControlChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (MessageWindow == null || _initialization)
                return;

            MessageWindow.LastAnswerLine = (int)e.NewValue;
            if (e.NewValue == null || e.OldValue == null)
                return;

            Context.UpdateTrigger.Set();
            Context.SaveRequest(LocationProperty.Scripts);
        }

        private void WindowDefaultAnswerControlChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (MessageWindow == null || _initialization)
                return;

            MessageWindow.DefaultAnswerLine = (int)e.NewValue;
            if (e.NewValue == null || e.OldValue == null)
                return;

            Context.UpdateTrigger.Set();
            Context.SaveRequest(LocationProperty.Scripts);
        }

        private void WindowCancelAnswerControlChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (MessageWindow == null || _initialization)
                return;

            MessageWindow.CancelAnswerLine = (int)e.NewValue;
            if (e.NewValue == null || e.OldValue == null)
                return;

            Context.UpdateTrigger.Set();
            Context.SaveRequest(LocationProperty.Scripts);
        }

        public void ChangeWindow(MessageWindow messageWindow)
        {
            MessageWindow = null;

            WindowXControl.IsEnabled = false;
            WindowYControl.IsEnabled = false;

            WindowFirstAnswerControl.IsEnabled = false;
            WindowLastAnswerControl.IsEnabled = false;
            WindowDefaultAnswerControl.IsEnabled = false;
            WindowCancelAnswerControl.IsEnabled = false;

            if (messageWindow == null)
                return;

            _initialization = true;

            WindowXControl.IsEnabled = messageWindow.IsMovable && !messageWindow.IsDynamic;
            WindowYControl.IsEnabled = messageWindow.IsMovable && !messageWindow.IsDynamic;
            WindowXControl.Value = messageWindow.X * 2;
            WindowYControl.Value = messageWindow.Y * 2;

            WindowFirstAnswerControl.IsEnabled = messageWindow.IsQuestion && !messageWindow.IsDynamic;
            WindowLastAnswerControl.IsEnabled = messageWindow.IsQuestion && !messageWindow.IsDynamic;
            WindowDefaultAnswerControl.IsEnabled = messageWindow.IsQuestion && !messageWindow.IsDynamic;
            WindowCancelAnswerControl.IsEnabled = messageWindow.IsQuestion && !messageWindow.IsDynamic;
            WindowFirstAnswerControl.Value = messageWindow.FirstAnswerLine;
            WindowLastAnswerControl.Value = messageWindow.LastAnswerLine;
            WindowDefaultAnswerControl.Value = messageWindow.DefaultAnswerLine;
            WindowCancelAnswerControl.Value = messageWindow.CancelAnswerLine;

            MessageWindow = messageWindow;
            _initialization = false;
        }
    }
}