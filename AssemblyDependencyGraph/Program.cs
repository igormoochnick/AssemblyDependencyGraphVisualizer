using System.IO;

namespace AssemblyDependencyGraph
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = args[0];

            var graph = AssemblyGraph.LoadGraph("test.gml");

            GraphAnalyzer.GetOrderedListOfDependencies(graph);

            /*
                        if (File.Exists(path))
                        {
                            var assembliesList = AssemblyLoader.GetAssemblyReferenceGraph(path);

                            var graph = AssemblyGraph.GenerateGraph(assembliesList);

                            AssemblyGraph.SaveGraphToFile(graph, "test.gml");
                        }
                    }
            */
        }
    }
}
