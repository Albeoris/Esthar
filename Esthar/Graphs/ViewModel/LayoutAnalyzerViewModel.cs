//using System;
//using WPFExtensions.ViewModel.Commanding;
//using System.Collections.ObjectModel;
//using System.Windows.Input;
//using System.ComponentModel;
//using System.Windows.Forms;
//using System.IO;

//namespace Esthar
//{
//    public partial class LayoutAnalyzerViewModel : CommandSink, INotifyPropertyChanged
//    {
//        #region Commands
//        public static readonly RoutedCommand AddLayoutCommand = new RoutedCommand( "AddLayout", typeof( LayoutAnalyzerViewModel ) );
//        public static readonly RoutedCommand RemoveLayoutCommand = new RoutedCommand( "RemoveLayout", typeof( LayoutAnalyzerViewModel ) );
//        public static readonly RoutedCommand RelayoutCommand = new RoutedCommand( "Relayout", typeof( LayoutAnalyzerViewModel ) );
//        public static readonly RoutedCommand ContinueLayoutCommand = new RoutedCommand( "ContinueLayout", typeof( LayoutAnalyzerViewModel ) );
//        public static readonly RoutedCommand OpenGraphsCommand = new RoutedCommand( "OpenGraphs", typeof( LayoutAnalyzerViewModel ) );
//        #endregion

//        private GraphModel selectedGraphModel;

//        public ObservableCollection<GraphModel> GraphModels { get; private set; }
//        public GraphModel SelectedGraphModel
//        {
//            get { return selectedGraphModel; }
//            set
//            {
//                if ( selectedGraphModel != value )
//                {
//                    selectedGraphModel = value;
//                    SelectedGraphChanged();
//                    NotifyChanged( "SelectedGraphModel" );
//                }
//            }
//        }

//        private void SelectedGraphChanged()
//        {
//            foreach ( var glvm in AnalyzedLayouts )
//                glvm.Graph = selectedGraphModel.Graph;
//        }

//        public ObservableCollection<GraphLayoutViewModel> AnalyzedLayouts { get; private set; }

//        public LayoutAnalyzerViewModel()
//        {
//            AnalyzedLayouts = new ObservableCollection<GraphLayoutViewModel>();
//            GraphModels = new ObservableCollection<GraphModel>();

//            RegisterCommand( AddLayoutCommand,
//                             param => true,
//                             param => AddLayout() );

//            RegisterCommand( RemoveLayoutCommand,
//                             param => CanRemoveLayout( param as GraphLayoutViewModel ),
//                             param => RemoveLayout( param as GraphLayoutViewModel ) );

//            RegisterCommand( ContinueLayoutCommand,
//                             param => AnalyzedLayouts.Count > 0,
//                             param => ContinueLayout() );

//            RegisterCommand( RelayoutCommand,
//                             param => AnalyzedLayouts.Count > 0,
//                             param => Relayout() );

//            RegisterCommand( OpenGraphsCommand,
//                             param => true,
//                             param => OpenGraphs(null) );
//        }

//        public void AddLayout()
//        {
//            var glvm = new GraphLayoutViewModel
//                        {
//                            LayoutAlgorithmType = string.Empty,
//                            IsSelected = true,
//                            Graph = ( selectedGraphModel == null ? null : selectedGraphModel.Graph )
//                        };
//            AnalyzedLayouts.Add( glvm );
//        }

//        public bool CanRemoveLayout( GraphLayoutViewModel glvm )
//        {
//            return ( glvm != null && AnalyzedLayouts.Contains( glvm ) );
//        }

//        public void RemoveLayout( GraphLayoutViewModel layoutViewModel )
//        {
//            AnalyzedLayouts.Remove( layoutViewModel );
//        }

//        public void ContinueLayout()
//        {
//            LayoutManager.Instance.ContinueLayout();
//        }

//        public void Relayout()
//        {
//            LayoutManager.Instance.Relayout();
//        }

//        public void OpenGraphs(AsmGraph graph)
//        {
//            GraphModels.Add(new GraphModel(Guid.NewGuid().ToString(), graph));
//        }


//        private void NotifyChanged( string propertyName )
//        {
//            if ( PropertyChanged != null )
//                PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
//        }

//        public event PropertyChangedEventHandler PropertyChanged;
//    }
//}