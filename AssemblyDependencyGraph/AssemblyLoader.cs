using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace AssemblyDependencyGraph
{
    public static class AssemblyLoader
    {
        static public IEnumerable<AssemblyInfo> GetAssemblyReferenceGraph(string targetAssemblyPath)
        {
            string loadPath = Path.GetDirectoryName(targetAssemblyPath);

            var foundNames = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase); // list of Assembly names to avoid dups and an endless loop
            var results = new List<AssemblyInfo>();     // ArrayList of AssemblyInfo objects
            var stack = new Stack<AssemblyInfo>();      // stack of Assembly AssemblyInfo objects

            stack.Push(new AssemblyInfo(AssemblyName.GetAssemblyName(targetAssemblyPath)) { FilePath =  targetAssemblyPath });

            // do a preorder, non-recursive traversal to store all remaining assemblies);
            while (stack.Count > 0)
            {
                var info = stack.Pop(); // get next assembly AssemblyInfo

                try
                {
                    Assembly assembly = LoadAssembly(ref info, loadPath);
                    if (assembly == null)
                        continue;

                    info.Children = assembly.GetReferencedAssemblies(); 
                    results.Add(info);
                    foundNames.Add(info.FullName);

                    foreach (var assemblyName in info.Children.Reverse()) // "right-to-left" = preorder traversal
                    {
                        if (!foundNames.Contains(assemblyName.FullName))
                        {
                            foundNames.Add(assemblyName.FullName);
                            var subChildInfo = new AssemblyInfo(assemblyName);
                            stack.Push(subChildInfo);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return results;
        }


        static Assembly LoadAssembly(ref AssemblyInfo assemblyInfo, string loadPath)
        {
            string filePath = assemblyInfo.FilePath ?? Path.Combine(loadPath, assemblyInfo.AssemblyName.Name + ".dll");

            try
            {
                assemblyInfo.IsInGac = true;
                return Assembly.ReflectionOnlyLoad(assemblyInfo.AssemblyName.FullName);
            }
            catch
            {
                assemblyInfo.IsInGac = false;
            }

            try
            {
                return Assembly.ReflectionOnlyLoadFrom(filePath);
            }
            catch (Exception ex)
            {
                // Possible unhandles exception: An attempt was made to load a program with an incorrect format.
                Console.WriteLine(ex);
            }

            return null;
        }

    }
}