using System;
using System.Windows;
using Esthar.Data.Transform;
using Esthar.UI;

namespace Esthar
{
    public partial class LocationControlScriptsTab : IDisposable
    {
        readonly LayoutAnalyzerViewModel analyzerViewModel = new LayoutAnalyzerViewModel();
        private Location _location;

        public LocationControlScriptsTab()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        public void Dispose()
        {
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                _location = (Location)DataContext;
                Redraw();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        private void Redraw()
        {
            if (_location == null)
                return;

            AsmInterpreter interpreter = new AsmInterpreter(_location.Scripts);
            interpreter.BindSegments();

            AsmGraphFactory factory = new AsmGraphFactory();

            foreach (AsmModule module in _location.Scripts.GetOrderedModules())
            {
                foreach (AsmEvent evt in module.GetOrderedEvents())
                {
                    foreach (AsmSegment sourceSegment in evt.Segments)
                    {
                        AsmVertex source = factory.CreateVertex(_location, sourceSegment);
                        foreach (AsmBinding binding in sourceSegment.OutputBindings)
                        {
                            AsmSegment[] targets = binding.ResolveTargets();
                            foreach (AsmSegment targetSegment in targets)
                            {
                                AsmVertex target = factory.CreateVertex(_location, targetSegment);
                                AsmEdge edge = factory.CreateEdge(source, target);
                            }
                        }
                    }
                }
            }

            factory.RemoveOverheadEdges();
            factory.FilterVertices();
            factory.RemoveOverheadEdges();
            graphLayout.Graph = factory.CreateGraph();
        }
    }
}