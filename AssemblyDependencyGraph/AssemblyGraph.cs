using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using QuickGraph;
using QuickGraph.Serialization;

namespace AssemblyDependencyGraph
{
    public static class AssemblyGraph
    {
        public static void SaveGraphToFile(AdjacencyGraph<string, Edge<string>> graph, string targetPath)
        {
            // Save the graph
            using (var xwriter = XmlWriter.Create(File.CreateText(targetPath)))
                graph.SerializeToGraphML<string, Edge<string>, AdjacencyGraph<string, Edge<string>>>(xwriter);
        }

        public static AdjacencyGraph<string, Edge<string>> LoadGraph(string path)
        {
            // Save the graph
            using (var xReader = XmlReader.Create(File.OpenText(path)))
            {
                var graph = new AdjacencyGraph<string, Edge<string>>();
                graph.DeserializeFromGraphML<string, Edge<string>, AdjacencyGraph<string, Edge<string>>>(xReader,
                    id => id,
                    (source, target, id) => new Edge<string>(source, target));

                return graph;
            }
        }

        public static AdjacencyGraph<string, Edge<string>> GenerateGraph(IEnumerable<AssemblyInfo> assemblies, bool skipGacAssemblies = true)
        {
            // Detect all the GAC assemblies
            var gacAssemblies = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            if (skipGacAssemblies)
            {
                foreach (var info in assemblies)
                {
                    if (info.IsInGac)
                        gacAssemblies.Add(info.FullName);
                }
            }

            var graph = new AdjacencyGraph<string, Edge<string>>();
            var verticesOnGraph = new HashSet<string>();

            foreach (var parent in assemblies)
            {
                string parentName = parent.FullName;

                // Skip GAC assemblies
                if (gacAssemblies.Contains(parentName))
                    continue;

                if (!verticesOnGraph.Contains(parentName))
                {
                    graph.AddVertex(parent.Name);
                    verticesOnGraph.Add(parentName);
                }

                foreach (var child in parent.Children)
                {
                    string childName = child.FullName;
                    if (gacAssemblies.Contains(childName))
                        continue;

                    if (!verticesOnGraph.Contains(childName))
                    {
                        graph.AddVertex(child.Name);

                        verticesOnGraph.Add(childName);
                    }

                    // TODO: reduce two-way edges.  This may happen in a rare case of circular dependency.
                    graph.AddEdge(new Edge<string>(parent.Name, child.Name));
                }
            }
            return graph;
        }
    }
}