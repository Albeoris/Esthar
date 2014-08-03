using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Esthar.Core;

namespace Esthar.UI
{
    /// <summary>
    /// Логика взаимодействия для FilterControl.xaml
    /// </summary>
    public partial class FilterControl
    {
        public FilterControl()
        {
            DataContext = this;
            InitializeComponent();
        }

        public event Action<string> FilterChanged;
        public event Action<Enum, bool?> PropertyStateChanged;
        public event Action<UserTag, bool?> TagStateChanged;
        public event Action<bool> IsShownTagsChanged;

        #region Properties 

        private readonly ConcurrentDictionary<Enum, CheckBox> _propertiesState = new ConcurrentDictionary<Enum, CheckBox>();

        public IEnumerable<Enum> GetCheckedProperties()
        {
            return _propertiesState.SelectWhere(p => p.Value.IsChecked == true, p => p.Key);
        }

        public IEnumerable<Enum> GetUncheckedProperties()
        {
            return _propertiesState.SelectWhere(p => p.Value.IsChecked == false, p => p.Key);
        }

        public IEnumerable<Enum> GetNotChangedProperties()
        {
            return _propertiesState.SelectWhere(p => p.Value.IsChecked == null, p => p.Key);
        }

        public void SetCheckedProperties(IEnumerable<Enum> properties)
        {
            foreach (Enum value in properties)
                _propertiesState[value].IsChecked = true;
        }

        public void SetUncheckedProperties(IEnumerable<Enum> properties)
        {
            foreach (Enum value in properties)
                _propertiesState[value].IsChecked = false;
        }

        public void SetNotChangedProperties(IEnumerable<Enum> properties)
        {
            foreach (Enum value in properties)
                _propertiesState[value].IsChecked = null;
        }

        #endregion

        #region Tags 

        private readonly ConcurrentDictionary<UserTag, CheckBox> _tagsState = new ConcurrentDictionary<UserTag, CheckBox>();

        public IEnumerable<UserTag> GetCheckedTags()
        {
            return _tagsState.SelectWhere(p => p.Value.IsChecked == true, p => p.Key);
        }

        public IEnumerable<UserTag> GetUncheckedTags()
        {
            return _tagsState.SelectWhere(p => p.Value.IsChecked == false, p => p.Key);
        }

        public IEnumerable<UserTag> GetNotChangedTags()
        {
            return _tagsState.SelectWhere(p => p.Value.IsChecked == null, p => p.Key);
        }

        public void SetCheckedTags(IEnumerable<UserTag> tags)
        {
            foreach (UserTag tag in tags)
                _tagsState[tag].IsChecked = true;
        }

        public void SetUncheckedTags(IEnumerable<UserTag> tags)
        {
            foreach (UserTag tag in tags)
                _tagsState[tag].IsChecked = false;
        }

        public void SetNotChangedTags(IEnumerable<UserTag> tags)
        {
            foreach (UserTag tag in tags)
                _tagsState[tag].IsChecked = null;
        }

        #endregion

        private Grid _innerContentGrid;
        private UniformGrid _propertiesPanel;
        private UniformGrid _tagsPanel;

        private void OnInnerContentGridInitialized(object sender, EventArgs e)
        {
            _innerContentGrid = (Grid)sender;
        }

        private void OnPropertiesPanelInitialized(object sender, EventArgs e)
        {
            _propertiesPanel = (UniformGrid)sender;
        }

        private void OnTagsPanelInitialized(object sender, EventArgs e)
        {
            _tagsPanel = (UniformGrid)sender;
        }

        #region DependencyProperties

        public static readonly DependencyProperty InnerContentProperty = DependencyProperty.Register("InnerContent", typeof(UIElement), typeof(FilterControl), new PropertyMetadata(null, OnPropertyChanged));
        public static readonly DependencyProperty IsShownTagsProperty = DependencyProperty.Register("IsShownTags", typeof(bool?), typeof(FilterControl), new PropertyMetadata(false, OnPropertyChanged));
        public static readonly DependencyProperty TagsPanelVisibilityProperty = DependencyProperty.Register("TagsPanelVisibility", typeof(Visibility), typeof(FilterControl), new PropertyMetadata(Visibility.Hidden));
        public static readonly DependencyProperty ContentPresenterVisibilityProperty = DependencyProperty.Register("ContentPresenterVisibility", typeof(Visibility), typeof(FilterControl), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty AvaliblePropertiesProperty = DependencyProperty.Register("AvalibleProperties", typeof(IEnumerable<Enum>), typeof(FilterControl), new PropertyMetadata(null, OnPropertyChanged));
        public static readonly DependencyProperty AvalibleTagsProperty = DependencyProperty.Register("AvalibleTags", typeof(IEnumerable<UserTag>), typeof(FilterControl), new PropertyMetadata(null, OnPropertyChanged));
        public static readonly DependencyProperty FilterWatermarkProperty = DependencyProperty.Register("FilterWatermark", typeof(string), typeof(FilterControl), new PropertyMetadata("Фильтр..."));
        public static readonly DependencyProperty FilterTextProperty = DependencyProperty.Register("FilterText", typeof(string), typeof(FilterControl), new PropertyMetadata(default(string), OnPropertyChanged));
        public static readonly DependencyProperty InternalFilterTextProperty = DependencyProperty.Register("InternalFilterText", typeof(string), typeof(FilterControl), new PropertyMetadata(default(string), OnPropertyChanged));
        public static readonly DependencyProperty FilterControlHeightProperty = DependencyProperty.Register("FilterControlHeight", typeof(double), typeof(FilterControl), new PropertyMetadata(default(double), OnPropertyChanged));
        public static readonly DependencyProperty InternalFilterControlHeightProperty = DependencyProperty.Register("InternalFilterControlHeight", typeof(double), typeof(FilterControl), new PropertyMetadata(default(double), OnPropertyChanged));
        public static readonly DependencyProperty ToggleButtonHeightProperty = DependencyProperty.Register("ToggleButtonHeight", typeof(double), typeof(FilterControl), new PropertyMetadata(default(double)));
        public static readonly DependencyProperty ToggleButtonVisibilityProperty = DependencyProperty.Register("ToggleButtonVisibility", typeof(Visibility), typeof(FilterControl), new PropertyMetadata(default(Visibility)));
        public static readonly DependencyProperty InternalFilterControlColumnSpanProperty = DependencyProperty.Register("InternalFilterControlColumnSpan", typeof(int), typeof(FilterControl), new PropertyMetadata(default(int)));

        public UIElement InnerContent
        {
            get { return (UIElement)GetValue(InnerContentProperty); }
            set { SetValue(InnerContentProperty, value); }
        }

        public bool IsShownTags
        {
            get { return (bool)GetValue(IsShownTagsProperty); }
            set { SetValue(IsShownTagsProperty, value); }
        }

        public Visibility TagsPanelVisibility
        {
            get { return (Visibility)GetValue(TagsPanelVisibilityProperty); }
            set { SetValue(TagsPanelVisibilityProperty, value); }
        }

        public Visibility ContentPresenterVisibility
        {
            get { return (Visibility)GetValue(ContentPresenterVisibilityProperty); }
            set { SetValue(ContentPresenterVisibilityProperty, value); }
        }

        public IEnumerable<Enum> AvalibleProperties
        {
            get { return (IEnumerable<Enum>)GetValue(AvaliblePropertiesProperty); }
            set { SetValue(AvaliblePropertiesProperty, value); }
        }

        public IEnumerable<UserTag> AvalibleTags
        {
            get { return (IEnumerable<UserTag>)GetValue(AvalibleTagsProperty); }
            set { SetValue(AvalibleTagsProperty, value); }
        }

        public string FilterWatermark
        {
            get { return (string)GetValue(FilterWatermarkProperty); }
            set { SetValue(FilterWatermarkProperty, value); }
        }

        public string FilterText
        {
            get { return (string)GetValue(FilterTextProperty); }
            set { SetValue(FilterTextProperty, value); }
        }

        public double ToggleButtonHeight
        {
            get { return (double)GetValue(ToggleButtonHeightProperty); }
            set { SetValue(ToggleButtonHeightProperty, value); }
        }

        public Visibility ToggleButtonVisibility
        {
            get { return (Visibility)GetValue(ToggleButtonVisibilityProperty); }
            set { SetValue(ToggleButtonVisibilityProperty, value); }
        }

        public int InternalFilterControlColumnSpan
        {
            get { return (int)GetValue(InternalFilterControlColumnSpanProperty); }
            set { SetValue(InternalFilterControlColumnSpanProperty, value); }
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FilterControl control = (FilterControl)d;

            if (e.Property == InnerContentProperty)
                OnInnerContentChanged(control, (UIElement)e.NewValue);
            else if (e.Property == IsShownTagsProperty)
                OnIsShownTagsChanged(control, (bool)e.NewValue);
            else if (e.Property == AvaliblePropertiesProperty)
                OnAvaliblePropertiesChanged(control, (IEnumerable<Enum>)e.NewValue);
            else if (e.Property == AvalibleTagsProperty)
                OnAvalibleTagsChanged(control, (IEnumerable<UserTag>)e.NewValue);
            else if (e.Property == FilterTextProperty)
                OnFilterTextChanged(control, (string)e.NewValue);
            else if (e.Property == InternalFilterTextProperty)
                OnInternalFilterTextChanged(control, (string)e.NewValue);
            else if (e.Property == FilterControlHeightProperty || e.Property == InternalFilterControlHeightProperty)
                OnFilterControlHeightChanged(control, (double)e.NewValue);
            else
                throw new NotImplementedException();
        }

        private static void OnInnerContentChanged(FilterControl control, UIElement newValue)
        {
            control._innerContentGrid.Children.Clear();
            control._innerContentGrid.Children.Add(newValue);
            control.UpdateToggleButtonVisibility();
        }

        private static void OnIsShownTagsChanged(FilterControl control, bool isShown)
        {
            if (isShown)
            {
                control.TagsPanelVisibility = Visibility.Visible;
                control.ContentPresenterVisibility = Visibility.Hidden;
            }
            else
            {
                control.TagsPanelVisibility = Visibility.Hidden;
                control.ContentPresenterVisibility = Visibility.Visible;
            }

            control.IsShownTagsChanged.NullSafeInvoke(isShown);
        }

        private static void OnAvaliblePropertiesChanged(FilterControl control, IEnumerable<Enum> newValue)
        {
            ConcurrentDictionary<Enum, CheckBox> states = control._propertiesState;
            UIElementCollection propertiesPanel = control._propertiesPanel.Children;
            states.Clear();
            propertiesPanel.Clear();

            foreach (Enum value in newValue)
            {
                CheckBox checkBox = new CheckBox
                {
                    Content = value,
                    IsChecked = null,
                    Margin = new Thickness(3),
                    Tag = value,
                    IsThreeState = true,
                };

                states.TryAdd(value, checkBox);
                checkBox.Checked += control.OnPropertyCheckedChanged;
                propertiesPanel.Add(checkBox);
            }

            control.UpdateToggleButtonVisibility();
        }

        private void OnPropertyCheckedChanged(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            Enum value = (Enum)checkBox.Tag;

            PropertyStateChanged.NullSafeInvoke(value, checkBox.IsChecked);
        }

        private static void OnAvalibleTagsChanged(FilterControl control, IEnumerable<UserTag> newValue)
        {
            ConcurrentDictionary<UserTag, CheckBox> states = control._tagsState;
            UIElementCollection tagsPanel = control._tagsPanel.Children;
            states.Clear();
            tagsPanel.Clear();

            foreach (UserTag userTag in newValue)
            {
                SolidColorBrush foreground = new SolidColorBrush(userTag.Foreground);
                SolidColorBrush background = new SolidColorBrush(userTag.Background);
                foreground.Freeze();
                background.Freeze();

                CheckBox checkBox = new CheckBox
                {
                    Content = userTag.Name,
                    Background = background,
                    IsChecked = null,
                    Margin = new Thickness(3),
                    Tag = userTag,
                    IsThreeState = true,
                };

                states.TryAdd(userTag, checkBox);
                checkBox.Checked += control.OnTagCheckedChanged;
                tagsPanel.Add(checkBox);
            }

            control.UpdateToggleButtonVisibility();
        }

        private void OnTagCheckedChanged(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            UserTag userTag = (UserTag)checkBox.Tag;

            TagStateChanged.NullSafeInvoke(userTag, checkBox.IsChecked);
        }

        private void UpdateToggleButtonVisibility()
        {
            if (_innerContentGrid.Children.Count == 0 || (_propertiesPanel.Children.Count == 0 && _tagsPanel.Children.Count == 0))
            {
                ToggleButtonVisibility = Visibility.Collapsed;
                InternalFilterControlColumnSpan = 2;
            }
            else
            {
                ToggleButtonVisibility = Visibility.Visible;
                InternalFilterControlColumnSpan = 1;
            }
        }

        private static void OnFilterTextChanged(FilterControl control, string filter)
        {
            control.FilterChanged.NullSafeInvoke(filter);
        }

        private static void OnInternalFilterTextChanged(FilterControl control, string filter)
        {
            UIElementCollection propertiesPanel = control._propertiesPanel.Children;
            foreach (CheckBox element in propertiesPanel)
            {
                if (string.IsNullOrEmpty((filter)))
                {
                    element.Visibility = Visibility.Visible;
                    continue;
                }

                element.Visibility = CultureInfo.CurrentCulture.CompareInfo.IndexOf(element.Tag.ToString(), filter, CompareOptions.IgnoreCase) >= 0 ? Visibility.Visible : Visibility.Collapsed;
            }

            UIElementCollection tagsPanel = control._tagsPanel.Children;
            foreach (CheckBox element in tagsPanel)
            {
                if (string.IsNullOrEmpty((filter)))
                {
                    element.Visibility = Visibility.Visible;
                    continue;
                }

                element.Visibility = CultureInfo.CurrentCulture.CompareInfo.IndexOf(((UserTag)element.Tag).Name, filter, CompareOptions.IgnoreCase) >= 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private static void OnFilterControlHeightChanged(FilterControl control, double newValue)
        {
            if (control.Visibility == Visibility.Visible)
                control.ToggleButtonHeight = newValue;
        }

        #endregion
    }
}