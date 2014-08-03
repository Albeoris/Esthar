using System.Collections.Generic;
using System.Windows;
using GraphSharp;
using GraphSharp.Algorithms.Layout.Simple.Hierarchical;
using GraphSharp.Controls;

namespace Esthar
{
    public class AsmGraphLayout : GraphLayout<AsmVertex, AsmEdge, AsmGraph>
    {
        public void SetGraph(AsmGraph graph)
        {
            //IDictionary<AsmVertex, Size> sizes = CalcSizes(graph);
            //IDictionary<AsmVertex, Point> positions = CalcPositions(graph);
            //LayoutAlgorithm = new EfficientSugiyamaLayoutAlgorithm<AsmVertex, AsmEdge, AsmGraph>(graph, new EfficientSugiyamaLayoutParameters(), positions, sizes);
            Graph = graph;
            Relayout();
        }

        private IDictionary<AsmVertex, Size> CalcSizes(AsmGraph graph)
        {
            Dictionary<AsmVertex, Size> result = new Dictionary<AsmVertex, Size>(graph.VertexCount);
            foreach (AsmVertex vertex in graph.Vertices)
                result.Add(vertex, new Size(70, 40));
            return result;
        }

        private IDictionary<AsmVertex, Point> CalcPositions(AsmGraph graph)
        {
            int x = 0;
            int y = 0;
            Dictionary<AsmVertex, Point> result = new Dictionary<AsmVertex, Point>(graph.VertexCount);
            foreach (AsmVertex vertex in graph.Vertices)
                result.Add(vertex, new Point(x = x + 40, y = y + 30));
            return result;
        }
    }
}