using System;
using System.Collections.Generic;
using QuickGraph;
using System.Linq;

namespace AssemblyDependencyGraph
{
    public static class GraphAnalyzer
    {
        public static IEnumerable<string> GetOrderedListOfDependencies(AdjacencyGraph<string, Edge<string>> graph)
        {
            var vertices =  graph.Vertices.ToList();
            var orderedList = new List<string>();

            while (vertices.Count > 0)
            {
                var vertexToRemove = new List<string>();
                var edgesToRemove = new List<Edge<string>>();

                foreach (var vertex in vertices)
                {
                    var outEdges = graph.OutEdges(vertex).ToList();

                    if (outEdges.Count == 0)
                    {
                        orderedList.Add(vertex);

                        string vertex1 = vertex;
                        edgesToRemove.AddRange(graph.Edges.Where(edge => edge.Target == vertex1));
                        vertexToRemove.Add(vertex);
                    }
                }

                if (vertexToRemove.Count == 0)
                    throw new InvalidOperationException("Graph has circular dependencies");

                edgesToRemove.ForEach(edge => graph.RemoveEdge(edge));

                vertexToRemove.ForEach(vertex =>
                                           {
                                               graph.RemoveVertex(vertex);
                                               vertices.Remove(vertex);
                                           });
            }

            return orderedList;
        }
    }
}