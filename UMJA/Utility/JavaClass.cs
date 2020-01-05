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
        public bool HasConstructor { get; set; }
        public bool HasGetter { get; set; }
        public bool HasSetter { get; set; }


    }
}
