using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Esthar.Core;
using Esthar.Data.Transform;
using Esthar.UI;

namespace Esthar
{
    public partial class LocationsListControl
    {
        private LocationView[] _content;
        public MainTabLocations ParentTab { get; set; }

        public LocationsListControl()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            Subscribe();

            Filter.AvalibleProperties = Enum.GetValues(typeof(LocationProperty)).Cast<Enum>().Where(f=>(LocationProperty)f != LocationProperty.None);
            OnGlobalTagsChanged();
        }

        private LocationsListView _list;
        
        private void OnLocationsListViewLoaded(object sender, RoutedEventArgs e)
        {
            _list = (LocationsListView)sender;
            _list.ParentListControl = this;

            LoadContent();
        }

        private void Subscribe()
        {
            UiService.TagsChanged += OnGlobalTagsChanged;
            Filter.FilterChanged += OnFilterTextChanged;
            Filter.IsShownTagsChanged += OnFilterIsShownTagsChanged;
        }

        private string _filterText;
        private Enum[] _checkedProperties;
        private Enum[] _uncheckedProperties;
        private UserTag[] _checkedTags;
        private UserTag[] _uncheckedTags;

        private void OnGlobalTagsChanged()
        {
            Filter.AvalibleTags = Options.UserTags.GetAllTags().Where(t => t.LocationBindable);
        }

        private void OnFilterTextChanged(string filterText)
        {
            _filterText = filterText;

            OnFilterChanged();
        }

        private void OnFilterIsShownTagsChanged(bool value)
        {
            if (value)
                return;

            _checkedProperties = Filter.GetCheckedProperties().ToArray();
            _uncheckedProperties = Filter.GetUncheckedProperties().ToArray();
            _checkedTags = Filter.GetCheckedTags().ToArray();
            _uncheckedTags = Filter.GetUncheckedTags().ToArray();

            OnFilterChanged();
        }

        private void OnFilterChanged()
        {
            if (_content == null)
                return;

            IEnumerable<LocationView> locs = _content;
            if (!_checkedProperties.IsNullOrEmpty())
                locs = locs.Where(l => _checkedProperties.Any(f => (l.Location.Importable & (LocationProperty)f) == (LocationProperty)f));
            if (!_uncheckedProperties.IsNullOrEmpty())
                locs = locs.Where(l => _uncheckedProperties.All(f => (l.Location.Importable & (LocationProperty)f) != (LocationProperty)f));
            if (!_checkedTags.IsNullOrEmpty())
                locs = locs.Where(l => l.Tags.Intersect(_checkedTags).Any());
            if (!_uncheckedTags.IsNullOrEmpty())
                locs = locs.Where(l => !l.Tags.Intersect(_uncheckedTags).Any());
            
            if (string.IsNullOrEmpty(_filterText))
            {
                _list.ItemsSource = locs;
                return;
            }

            HashSet<LocationView> locations = new HashSet<LocationView>();
            HashSet<string> filters = new HashSet<string>();

            IEnumerable<LocationView> filterByTitle = locs.Where(l => l.Location.MatchTitleFilter(_filterText));
            foreach (LocationView loc in filterByTitle)
            {
                locations.Add(loc);
                filters.Add(loc.Location.Name.TrimEnd('_', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'));
            }

            IEnumerable<LocationView> filterByName = locs.Where(l => l.Location.MatchNameFilter(_filterText));
            foreach (LocationView loc in filterByName)
                locations.Add(loc);

            foreach (string filter in filters)
            {
                filterByName = locs.Where(l => string.IsNullOrEmpty(l.Location.Title) && l.Location.MatchNameFilter(filter));
                foreach (LocationView loc in filterByName)
                    locations.Add(loc);
            }

            _list.ItemsSource = locations.OrderBy(loc => loc.Location.Name);
        }

        private void LoadContent()
        {
            ProgressWindow pw = new ProgressWindow("Загрузка локаций");
            Task<Location[]> task = Task.Run(() => LoadContentAync(pw));
            pw.ShowDialog();

            Location[] content = task.Result;
            if (content != null)
            {
                _content = content.Select(l => new LocationView(l)).ToArray();
                _tools.SetContent(content);
                _list.ItemsSource = _content;
            }
        }

        private Location[] LoadContentAync(ProgressWindow pw)
        {
            try
            {
                return Locations.GetLocationList(pw.SetTotal, pw.Increment);
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
                return null;
            }
            finally
            {
                Dispatcher.Invoke(pw.Close);
            }
        }
    }
}