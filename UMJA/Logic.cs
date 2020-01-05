using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using UMJA.Utility;

namespace UMJA
{
    public class Logic
    {
        public static void ReadDocument(string path)
        {

            // (Application.Current.Windows[0] as MainWindow).lsbLogConsole.Items.Add("Sth changed"); 


            XmlDocument doc = new XmlDocument();
            doc.Load("C:/Users/TH24/Desktop/HTL/Aud/4.Klasse/Umja/UMJA/uml.xml");
            List<GroupNode> groupNodes = new List<GroupNode>(); 

            var genericGroupNodes = doc.GetElementsByTagName("y:GenericGroupNode");
            foreach (XmlNode groupNode in genericGroupNodes)
            {
                if (!groupNode.InnerText.Equals(""))
                {
                    string idOfGroup = groupNode.ParentNode.ParentNode.ParentNode.ParentNode.Attributes["id"].Value;
                    string nameOfGroup = groupNode.InnerText;
                    groupNodes.Add(new GroupNode { Id = idOfGroup, Name = nameOfGroup });
                }
            }

            var classNodeLabels = doc.GetElementsByTagName("y:UMLClassNode");

            List<JavaObject> objects = new List<JavaObject>();

            foreach (XmlNode node in classNodeLabels)
            {
                string className = "";
                string stereotype = "";
                string variableString = "";
                string methodString = "";
                string enumValues = "";
                string nodeId = node.ParentNode.ParentNode.Attributes["id"].Value;


                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.Name.Equals("y:NodeLabel")) className = childNode.InnerText;
                    else if (childNode.Name.Equals("y:UML"))
                    {
                        stereotype = childNode.Attributes["stereotype"].Value;
                        if (stereotype.Equals("enumeration"))
                        {
                            foreach (XmlNode labelChildNode in childNode.ChildNodes)
                            {
                                if (labelChildNode.Name.Equals("y:AttributeLabel")) enumValues = labelChildNode.InnerText;
                            }
                        }
                        else
                        {
                            foreach (XmlNode labelChildNode in childNode.ChildNodes)
                            {
                                if (labelChildNode.Name.Equals("y:AttributeLabel")) variableString = labelChildNode.InnerText;
                                if (labelChildNode.Name.Equals("y:MethodLabel")) methodString = labelChildNode.InnerText;
                            }
                        }
                    }


                }
                if (stereotype.Equals("enumeration"))
                {
                    objects.Add(new JavaEnumeration { Name = className, Values = enumValues, NodeId = nodeId });
                }
                else
                {
                    List<Method> methods = new List<Method>();
                    if (methodString != null && !methodString.Equals(string.Empty)) methods = Method.ParseMethods(methodString);
                    List<Variable> variables = new List<Variable>();
                    if (variableString != null && !variableString.Equals(string.Empty)) variables = Variable.ParseVariables(variableString);

                    if (stereotype.Equals("interface")) 
                        objects.Add(new JavaInterface { Name = className, Methods = methods, Variables = variables, NodeId = nodeId });
                    else 
                        objects.Add(new JavaClass { Name = className, Methods = methods, Variables = variables, NodeId = nodeId });
                }


            }




            objects.ForEach(x => x.Package = groupNodes.Where(g => g.Id.Equals(x.NodeId.Split(':')[0])).First().Name);


            var edges = doc.GetElementsByTagName("edge");

            foreach (XmlNode edgeNode in edges)
            {
                string source = edgeNode.Attributes["source"].Value;
                string target = edgeNode.Attributes["target"].Value;

                string sourceGroup = source.Split(':')[0];
                string sourceNode = source; 

                string targetGroup = target.Split(':')[0];
                string targetNode = target;

                bool sourceHasToImportTarget = !source.Equals(target);

                string lineType = "";
                string sourceArrowType = "";
                string targetArrowType = "";
                foreach (XmlNode data in edgeNode.ChildNodes)
                {
                    foreach (XmlNode polyLineEdge in data.ChildNodes)
                    {
                        foreach (XmlNode polyLineEdgeChild in polyLineEdge.ChildNodes)
                        {
                            
                            if(polyLineEdgeChild.Name.Equals("y:Arrows"))
                            {
                               

                                if(polyLineEdgeChild.Attributes["source"] != null)
                                    sourceArrowType = polyLineEdgeChild.Attributes["source"].Value;
                                if (polyLineEdgeChild.Attributes["target"] != null)
                                    targetArrowType = polyLineEdgeChild.Attributes["target"].Value;
                            }
                            else if (polyLineEdgeChild.Name.Equals("y:LineStyle") && polyLineEdgeChild.Attributes["type"]!= null)
                            {
                                lineType = polyLineEdgeChild.Attributes["type"].Value;
                            }
                        }
                    }
                }

                //implements:
                if(lineType.Equals("dashed")&&targetArrowType.Equals("white_delta"))
                {
                   objects
                        .Where(x => x.NodeId.Equals(source))
                        .Select(x => (JavaClass)x)
                        .First()
                        .Implements
                        .Add(objects.Where(x => x.NodeId.Equals(target))
                        .First());
                }
                
            }


            //var allNodes = doc.GetElementsByTagName("node");


            //foreach (XmlNode node in allNodes)
            //{

            //    if(node.Attributes["yfiles.foldertype"] != null && node.Attributes["yfiles.foldertype"].Value.Equals("group"))
            //    {
            //        var x = node.ChildNodes;

            //        foreach (XmlNode childNode in node.ChildNodes)
            //        {
            //            var z = childNode.Name;
            //        }

            //        foreach (XmlNode genericGroupNode in node.SelectSingleNode("data").SelectSingleNode("y:ProxyAutoBoundsNode").SelectSingleNode("y:Realizers").ChildNodes)
            //        {
            //            var y = genericGroupNode.InnerText;
            //        }


            //    }
            //}





            //var classNodeLabels = doc.GetElementsByTagName("y:UMLClassNode");

            //List<JavaObject> objects = new List<JavaObject>();

            //foreach (XmlNode node in classNodeLabels)
            //{
            //    string className = "";
            //    string stereotype = "";
            //    string variableString = "";
            //    string methodString = "";
            //    string enumValues = "";


            //    foreach (XmlNode childNode in node.ChildNodes)
            //    {
            //        if (childNode.Name.Equals("y:NodeLabel")) className = childNode.InnerText;
            //        else if (childNode.Name.Equals("y:UML"))
            //        {
            //            stereotype = childNode.Attributes["stereotype"].Value;
            //            if(stereotype.Equals("enumeration"))
            //            {
            //                foreach (XmlNode labelChildNode in childNode.ChildNodes)
            //                {
            //                    if (labelChildNode.Name.Equals("y:AttributeLabel")) enumValues = labelChildNode.InnerText;

            //                }
            //            }
            //            else
            //            {
            //                foreach (XmlNode labelChildNode in childNode.ChildNodes)
            //                {
            //                    if (labelChildNode.Name.Equals("y:AttributeLabel")) variableString = labelChildNode.InnerText;
            //                    if (labelChildNode.Name.Equals("y:MethodLabel")) methodString = labelChildNode.InnerText;
            //                }
            //            }
            //        }


            //    }
            //    if(stereotype.Equals("enumeration"))
            //    {
            //        objects.Add(new JavaEnumeration { Name = className, Values = enumValues });
            //    }
            //    else
            //    {
            //        List<Method> methods = new List<Method>();
            //        if (methodString != null && !methodString.Equals(string.Empty)) methods = Method.ParseMethods(methodString);
            //        List<Variable> variables = new List<Variable>();
            //        if (variableString != null && !variableString.Equals(string.Empty)) variables = Variable.ParseVariables(variableString);

            //        objects.Add(new JavaClass { Name = className, Methods = methods, Variables = variables});
            //    }


            //}






            //var nodelabels = doc.GetElementsByTagName("y:NodeLabel");

            //foreach (XmlNode nodelabel in nodelabels)
            //{
            //    Console.WriteLine(nodelabel.InnerText);
            //    foreach (XmlNode item in nodelabel.ChildNodes)
            //    {
            //        if (item.Name.Equals("y:LabelModel"))
            //        {
            //            Console.WriteLine("no class");
            //        }
            //    }

            //}




        }

        private static JavaObject GetJavaObjectOfId(string id, List<JavaObject> javaObjects)
        {
            return javaObjects.Where(x => x.NodeId.Equals(id)).First();
        }
    }
}
