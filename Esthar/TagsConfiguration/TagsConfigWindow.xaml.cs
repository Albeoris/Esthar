using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Esthar.Core;
using Esthar.UI;

namespace Esthar
{
    /// <summary>
    /// Логика взаимодействия для TagsConfigWindow.xaml
    /// </summary>
    public partial class TagsConfigWindow
    {
        public TagsConfigWindow()
        {
            DataContext = this;
            InitializeComponent();
            Loaded += OnLoaded;
            TagsListView.SizeChanged += OnTagsListViewSizeChanged;
            TagsListView.Loaded += OnTagsListViewLoaded;
        }

        private void OnTagsListViewLoaded(object sender, RoutedEventArgs e)
        {
            SetTagsListViewColumnWidth();
        }

        private void OnTagsListViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetTagsListViewColumnWidth();
        }

        private void SetTagsListViewColumnWidth()
        {
            GridView view = (GridView)TagsListView.View;
            double width = TagsListView.ActualWidth - 10;
            for (int i = 1; i < view.Columns.Count; i++)
                width -= view.Columns[i].ActualWidth;
            if (!double.IsNaN(width) && width > 0)
                view.Columns[0].Width = width;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            IEnumerable<UserTagView> items = Options.UserTags.GetAllTags().Select(t => (UserTagView)t);
            TagsCollection = new ObservableCollection<UserTagView>(items);
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                List<int> emptyIndices = new List<int>();
                HashSet<string> original = new HashSet<string>();
                HashSet<string> duplicates = new HashSet<string>();
                int index = 0;
                foreach (UserTagView view in TagsCollection)
                {
                    if (string.IsNullOrEmpty(view.Name))
                        emptyIndices.Add(index);
                    else if (!original.Add(view.Name))
                        duplicates.Add(view.Name);
                    index++;
                }

                StringBuilder sb = new StringBuilder();
                if (emptyIndices.Count > 0)
                    sb.Append("Вы не указали имена для тегов: ").AppendLine(string.Join(", ", emptyIndices));
                if (duplicates.Count > 0)
                {
                    sb.AppendLine("В таблице присутствуют теги с одинаковыми именами:");
                    foreach (string name in duplicates)
                        sb.AppendLine(name);
                }

                if (sb.Length > 0)
                {
                    MessageBox.Show(sb.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                int priority = 0;
                IEnumerable<UserTag> items = TagsCollection.Select(view =>
                {
                    UserTag tag = (UserTag)view;
                    tag.Priority = priority++;
                    return tag;
                });

                Options.UserTags.TrySync(items);
                Options.Save();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ObservableCollection<UserTagView> collection = TagsCollection;
            if (collection == null)
                return;
            sw.Stop();

            Debug.WriteLine(sw.ElapsedMilliseconds);

            int index = SelectedTagIndex + 1;
            if (index > collection.Count)
                index = collection.Count;

            collection.Insert(index, (UserTagView)(new UserTag("Tag " + (collection.Count + 1))));
        }

        private void OnRemoveButtonClick(object sender, RoutedEventArgs e)
        {
            ObservableCollection<UserTagView> collection = TagsCollection;
            int index = SelectedTagIndex;

            if (collection == null || index < 0 || index >= collection.Count)
                return;

            collection.RemoveAt(index);
        }

        private void OnUpButtonClick(object sender, RoutedEventArgs e)
        {
            ObservableCollection<UserTagView> collection = TagsCollection;
            int index = SelectedTagIndex;

            if (collection == null || index < 1 || index >= collection.Count)
                return;

            collection.Move(index, index - 1);
        }

        private void OnDownButtonClick(object sender, RoutedEventArgs e)
        {
            ObservableCollection<UserTagView> collection = TagsCollection;
            int index = SelectedTagIndex;

            if (collection == null || index < 0 || index >= collection.Count - 1)
                return;

            collection.Move(index, index + 1);
        }

        private static readonly DependencyProperty TagsCollectionProperty = DependencyProperty.Register("TagsCollection", typeof(ObservableCollection<UserTagView>), typeof(TagsConfigWindow), new PropertyMetadata(default(ObservableCollection<UserTagView>)));
        public static readonly DependencyProperty SelectedTagIndexProperty = DependencyProperty.Register("SelectedTagIndex", typeof(int), typeof(TagsConfigWindow), new PropertyMetadata(default(int)));

        private ObservableCollection<UserTagView> TagsCollection
        {
            get { return (ObservableCollection<UserTagView>)GetValue(TagsCollectionProperty); }
            set { SetValue(TagsCollectionProperty, value); }
        }

        private int SelectedTagIndex
        {
            get { return (int)GetValue(SelectedTagIndexProperty); }
            set { SetValue(SelectedTagIndexProperty, value); }
        }
    }
}