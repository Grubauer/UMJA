using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMJA.Utility
{
    class JavaInterface : JavaObject
    {
        public List<Method> Methods { get; set; }
        public List<Variable> Variables { get; set; }
        
    }
}
