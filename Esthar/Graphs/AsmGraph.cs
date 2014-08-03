using QuickGraph;

namespace Esthar
{
	public class AsmGraph : BidirectionalGraph<AsmVertex, AsmEdge>
	{
		public AsmGraph() { }

		public AsmGraph(bool allowParallelEdges)
			: base(allowParallelEdges) { }

		public AsmGraph(bool allowParallelEdges, int vertexCapacity)
			: base(allowParallelEdges, vertexCapacity) { }
	}
}