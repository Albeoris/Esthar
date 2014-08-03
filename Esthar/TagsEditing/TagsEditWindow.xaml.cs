using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Esthar.Core;
using Esthar.UI;

namespace Esthar
{
    /// <summary>
    /// Логика взаимодействия для TagsEditWindow.xaml
    /// </summary>
    public partial class TagsEditWindow : Window
    {
        private readonly IEnumerable<UserTag> _avalibleTags;
        private readonly IEnumerable<IUserTagsHandler> _taggedObjects;

        public TagsEditWindow(IEnumerable<UserTag> avalibleTags, IEnumerable<IUserTagsHandler> taggedObjects)
        {
            _avalibleTags = avalibleTags;
            _taggedObjects = taggedObjects;
            InitializeComponent();
        }

        private FilterControl _control;

        private void OnFilterLoaded(object sender, RoutedEventArgs e)
        {
            _control = (FilterControl)sender;

            int objectsCount = 0;
            Dictionary<UserTag, int> existsTags = new Dictionary<UserTag, int>();
            foreach (IUserTagsHandler obj in _taggedObjects)
            {
                foreach (UserTag tag in obj.Tags)
                {
                    int count;
                    existsTags.TryGetValue(tag, out count);
                    existsTags[tag] = count + 1;
                }
                objectsCount++;
            }

            _control.AvalibleTags = _avalibleTags;
            _control.SetCheckedTags(existsTags.SelectWhere(p => p.Value == objectsCount, p => p.Key));
            _control.SetUncheckedTags(_avalibleTags.Where(t => !existsTags.ContainsKey(t)));
            _control.IsShownTags = true;
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            IEnumerable<UserTag> checkedTags = _control.GetCheckedTags();
            IEnumerable<UserTag> uncheckedTags = _control.GetUncheckedTags();

            foreach (IUserTagsHandler obj in _taggedObjects)
            {
                obj.Tags.TryAdd(checkedTags);
                obj.Tags.TryRemove(uncheckedTags);
            }

            DialogResult = true;
            Close();
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
