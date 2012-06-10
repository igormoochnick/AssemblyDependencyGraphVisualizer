using System.Collections.Generic;
using System.Reflection;

namespace AssemblyDependencyGraph
{
    public class AssemblyInfo
    {
        public AssemblyInfo(AssemblyName assemblyName)
        {
            this.AssemblyName = assemblyName;
        }

        public string FullName { get { return AssemblyName.FullName; } }

        public string Name { get { return AssemblyName.Name; } }

        public bool IsInGac { get; set; }

        public IEnumerable<AssemblyName> Children { get; set; }

        public string FilePath { get; set; }

        public AssemblyName AssemblyName { get; private set; }
    }
}