using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMJA.Utility
{
    public class JavaClass : JavaObject
    {
        
        public List<Method> Methods { get; set; }
        public List<Variable> Variables { get; set; }
        public List<JavaObject> ObjectsToImport { get; set; }
        public List<JavaObject> Implements { get; set; } = new List<JavaObject>();
    }
}
