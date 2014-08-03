using System;
using System.Collections.Generic;
using System.Linq;
using Esthar.Data.Transform;

namespace Esthar
{
    internal sealed class AsmGraphFactory
    {
        private readonly Dictionary<AsmSegment, AsmVertex> _vertices = new Dictionary<AsmSegment, AsmVertex>();
        private readonly Dictionary<Tuple<AsmVertex, AsmVertex>, AsmEdge> _edges = new Dictionary<Tuple<AsmVertex, AsmVertex>, AsmEdge>();
        private readonly Dictionary<AsmVertex, List<AsmEdge>> _edgeSources = new Dictionary<AsmVertex, List<AsmEdge>>();
        private readonly Dictionary<AsmVertex, List<AsmEdge>> _edgeTargets = new Dictionary<AsmVertex, List<AsmEdge>>();

        public AsmVertex CreateVertex(Location location, AsmSegment segment)
        {
            AsmVertex result;
            if (!_vertices.TryGetValue(segment, out result))
            {
                result = new AsmVertex(location, segment);
                _vertices.Add(segment, result);
            }

            return result;
        }

        public AsmEdge CreateEdge(AsmVertex source, AsmVertex target)
        {
            if (source == null || target == null)
                return null;

            Tuple<AsmVertex, AsmVertex> tuple = Tuple.Create(source, target);
            AsmEdge result;
            if (!_edges.TryGetValue(tuple, out result))
            {
                result = new AsmEdge(source, target);
                _edges.Add(tuple, result);

                List<AsmEdge> edges;
                if (!_edgeSources.TryGetValue(source, out edges))
                {
                    edges = new List<AsmEdge>();
                    _edgeSources.Add(source, edges);
                }
                edges.Add(result);

                if (!_edgeTargets.TryGetValue(target, out edges))
                {
                    edges = new List<AsmEdge>();
                    _edgeTargets.Add(target, edges);
                }
                edges.Add(result);
            }

            return result;
        }

        public AsmGraph CreateGraph()
        {
            AsmGraph graph = new AsmGraph(true, _vertices.Count);
            graph.AddVertexRange(_vertices.Values);
            graph.AddEdgeRange(_edges.Values);
            return graph;
        }

        public void FilterVertices()
        {
            List<AsmSegment> toDelete = _vertices.Where(pair => pair.Value.IsEmpty).Select(pair => pair.Key).ToList();
            foreach (AsmSegment segment in toDelete)
            {
                AsmVertex vertex = _vertices[segment];
                _vertices.Remove(segment);

                List<AsmEdge> sourceEdges;
                List<AsmEdge> targetEdges;
                _edgeSources.TryGetValue(vertex, out sourceEdges);
                _edgeTargets.TryGetValue(vertex, out targetEdges);
                if (sourceEdges != null && targetEdges != null)
                {
                    for (int s = sourceEdges.Count - 1; s >= 0; s--)
                    {
                        AsmEdge source = sourceEdges[s];
                        if (source.Target.Segment == vertex.Segment)
                            continue;

                        for (int t = targetEdges.Count - 1; t >= 0; t--)
                        {
                            AsmEdge target = targetEdges[t];
                            if (target.Source.Segment == vertex.Segment)
                                continue;

                            AsmEdge newEdge = CreateEdge(target.Source, source.Target);
                        }
                    }
                }
                if (sourceEdges != null)
                    _edgeSources.Remove(vertex);
                if (targetEdges != null)
                    _edgeTargets.Remove(vertex);
            }

            List<Tuple<AsmVertex, AsmVertex>> toDeleteEdges = new List<Tuple<AsmVertex, AsmVertex>>(1024);
            foreach (KeyValuePair<Tuple<AsmVertex, AsmVertex>, AsmEdge> pair in _edges)
            {
                if (!_vertices.ContainsKey(pair.Value.Source.Segment) || !_vertices.ContainsKey(pair.Value.Target.Segment))
                    toDeleteEdges.Add(pair.Key);
            }
            foreach (var key in toDeleteEdges)
                _edges.Remove(key);
        }

        /// <summary>
        /// <remarks>Не очищает Source\Target Edges</remarks>
        /// </summary>
        public void RemoveOverheadEdges()
        {
            _edgeSources.Clear();
            foreach (AsmEdge edge in _edges.Values)
            {
                List<AsmEdge> edgeSources;
                if (!_edgeSources.TryGetValue(edge.Source, out edgeSources))
                {
                    edgeSources = new List<AsmEdge>();
                    _edgeSources.Add(edge.Source, edgeSources);
                }
                edgeSources.Add(edge);
            }

            HashSet<AsmEdge> toDelete = new HashSet<AsmEdge>();
            foreach (AsmEdge edge in _edges.Values)
            {
                List<AsmEdge> sourceEdges = _edgeSources[edge.Source];
                for (int index = sourceEdges.Count - 1; index >= 0; index--)
                {
                    HashSet<AsmEdge> routedEdges = new HashSet<AsmEdge>();

                    AsmEdge childEdge = sourceEdges[index];
                    if (edge == childEdge)
                        continue;

                    if (EdgeRouteToVertex(childEdge, edge.Target, toDelete, routedEdges))
                    {
                        toDelete.Add(edge);
                        sourceEdges.Remove(edge);
                        break;
                    }
                }
            }
            
            foreach (AsmEdge edge in toDelete)
                _edges.Remove(Tuple.Create(edge.Source, edge.Target));
        }

        private bool EdgeRouteToVertex(AsmEdge edge, AsmVertex target, HashSet<AsmEdge> toDelete, HashSet<AsmEdge> routedEdges, int level = 0)
        {
            if (!routedEdges.Add(edge))
                return false;

            if (toDelete.Contains(edge))
                return false;

            if (level > 0 && edge.Target.Segment == target.Segment)
                return true;

            List<AsmEdge> sourceEdges;
            if (!_edgeSources.TryGetValue(edge.Target, out sourceEdges))
                return false;

            foreach (AsmEdge childEdge in sourceEdges)
            {
                if (edge == childEdge)
                    continue;

                if (EdgeRouteToVertex(childEdge, target, toDelete, routedEdges, level + 1))
                    return true;
            }

            return false;
        }
    }
}