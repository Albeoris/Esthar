using System;
using System.Windows;
using System.Windows.Media;
using Esthar.Core;

namespace Esthar
{
    internal sealed class UserTagView : DependencyObject
    {
        public UserTagView(UserTag tag)
        {
            Name = tag.Name;
            Priority = tag.Priority;
            Font = (FontDescriptorView)tag.Font;
            Foreground = tag.Foreground;
            Background = tag.Background;
            LocationBindable = tag.LocationBindable;
            MessageBindable = tag.MessageBindable;
        }

        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(UserTagView), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PriorityProperty = DependencyProperty.Register("Priority", typeof(int), typeof(UserTagView), new PropertyMetadata(default(int)));
        public static readonly DependencyProperty FontProperty = DependencyProperty.Register("Font", typeof(FontDescriptorView), typeof(UserTagView), new PropertyMetadata(default(FontDescriptorView)));
        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Color), typeof(UserTagView), new PropertyMetadata(default(Color), OnPropertyChanged));
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Color), typeof(UserTagView), new PropertyMetadata(default(Color), OnPropertyChanged));
        public static readonly DependencyProperty LocationBindableProperty = DependencyProperty.Register("LocationBindable", typeof(bool), typeof(UserTagView), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty MessageBindableProperty = DependencyProperty.Register("MessageBindable", typeof(bool), typeof(UserTagView), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty ForegroundBrushProperty = DependencyProperty.Register("ForegroundBrush", typeof(Brush), typeof(UserTagView), new PropertyMetadata(default(Brush)));
        public static readonly DependencyProperty BackgroundBrushProperty = DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(UserTagView), new PropertyMetadata(default(Brush)));
        
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public int Priority
        {
            get { return (int)GetValue(PriorityProperty); }
            set { SetValue(PriorityProperty, value); }
        }

        public FontDescriptorView Font
        {
            get { return (FontDescriptorView)GetValue(FontProperty); }
            set { SetValue(FontProperty, value); }
        }

        public Color Foreground
        {
            get { return (Color)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public Color Background
        {
            get { return (Color)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public bool LocationBindable
        {
            get { return (bool)GetValue(LocationBindableProperty); }
            set { SetValue(LocationBindableProperty, value); }
        }

        public bool MessageBindable
        {
            get { return (bool)GetValue(MessageBindableProperty); }
            set { SetValue(MessageBindableProperty, value); }
        }

        public Brush ForegroundBrush
        {
            get { return (Brush)GetValue(ForegroundBrushProperty); }
            set { SetValue(ForegroundBrushProperty, value); }
        }

        public Brush BackgroundBrush
        {
            get { return (Brush)GetValue(BackgroundBrushProperty); }
            set { SetValue(BackgroundBrushProperty, value); }
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UserTagView tag = (UserTagView)d;

            if (e.Property == ForegroundProperty)
                tag.ForegroundBrush = new SolidColorBrush((Color)e.NewValue);
            else if (e.Property == BackgroundProperty)
                tag.BackgroundBrush = new SolidColorBrush((Color)e.NewValue);
            else
                throw new NotImplementedException();
        }

        public static explicit operator UserTag(UserTagView self)
        {
            return new UserTag(self.Name)
            {
                Priority = self.Priority,
                Font = (FontDescriptor)self.Font,
                Foreground = self.Foreground,
                Background = self.Background,
                LocationBindable = self.LocationBindable,
                MessageBindable = self.MessageBindable
            };
        }

        public static explicit operator UserTagView(UserTag self)
        {
            return new UserTagView(self);
        }
    }
}