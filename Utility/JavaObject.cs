using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMJA.Utility
{
    public class JavaObject
    {
        public string Name { get; set; }
        public string NodeId { get; set; }
        public string Package { get; set; }
        public List<JavaObject> ObjectsToImport { get; set; } = new List<JavaObject>();
        public List<JavaObject> Implements { get; set; } = new List<JavaObject>();

        public override string ToString()
        {
            StringBuilder objSB = new StringBuilder();
            StringBuilder implSB = new StringBuilder();
            ObjectsToImport.ForEach(x => objSB.Append($"{x.Name}, "));
            Implements.ForEach(x => implSB.Append($"{x.Name}, "));

            return $"Name: {Name} Package: {Package} ObjectsToImport: {objSB.ToString().Trim()} Implements: {implSB.ToString().ToString()}";
        }
    }
}
