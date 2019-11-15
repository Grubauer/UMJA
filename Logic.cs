using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UMJA
{
    public class Logic
    {
        public static void ReadDocument(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("C:\Users\max\source\repos\UMJA\UMJA\uml.xml");

            var x = doc.ChildNodes;

        }
    }
}
