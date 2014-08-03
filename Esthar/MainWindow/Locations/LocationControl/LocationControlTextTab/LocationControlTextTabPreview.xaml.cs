using System.Windows;
using Esthar.Data.Transform;
using Esthar.OpenGL;

namespace Esthar
{
    /// <summary>
    /// Логика взаимодействия для LocationControlTextTabPreview.xaml
    /// </summary>
    public partial class LocationControlTextTabPreview
    {
        public LocationControlTextTabPreview()
        {
            InitializeComponent();
            Scroll.Width = 640 + SystemParameters.VerticalScrollBarWidth;

            MessageWindowControl.WindowXControl.ValueChanged += OnWindowXChanged;
            MessageWindowControl.WindowYControl.ValueChanged += OnWindowYChanged;
            MessageWindowControl.WindowFirstAnswerControl.ValueChanged += OnFirstAnswerChanged;
            MessageWindowControl.WindowLastAnswerControl.ValueChanged += OnLastAnswerChanged;
            MessageWindowControl.WindowDefaultAnswerControl.ValueChanged += OnDefaultAnswerChanged;
            MessageWindowControl.WindowCancelAnswerControl.ValueChanged += OnCancelAnswerChanged;
        }

        public LocationTextControlContext Context
        {
            set { MessageWindowControl.Context = value; }
            get { return MessageWindowControl.Context; }
        }

        public void ChangeWindow(MessageWindow messageWindow)
        {
            MessageWindowControl.ChangeWindow(messageWindow);
        }

        private void OnWindowXChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            GLLocationRenderer.WindowX = (int)e.NewValue;
            OnWindowIfnoChanged();
        }

        private void OnWindowYChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            GLLocationRenderer.WindowY = (int)e.NewValue;
            OnWindowIfnoChanged();
        }

        private void OnFirstAnswerChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            GLLocationRenderer.FirstAnswer = (int)e.NewValue;
            OnWindowIfnoChanged();
        }

        private void OnLastAnswerChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            GLLocationRenderer.LastAnswer = (int)e.NewValue;
            OnWindowIfnoChanged();
        }

        private void OnDefaultAnswerChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            GLLocationRenderer.DefaultAnswer = (int)e.NewValue;
            OnWindowIfnoChanged();
        }

        private void OnCancelAnswerChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            GLLocationRenderer.CancelAnswer = (int)e.NewValue;
            OnWindowIfnoChanged();
        }

        private void OnWindowIfnoChanged()
        {
            GLLocationRenderer.DrawEvent.Set();
        }
    }
}