using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UMJA.Utility;

namespace UMJA
{
    public class Logic
    {
        public static List<JavaObject> ReadDocument(string path)
        {
            if (!path.EndsWith("graphml"))
            {
                //TODO fehler an GUI (am besten mit event)
                return null;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(path);
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

                var sourceClass = objects
                        .Where(x => x.NodeId.Equals(source))
                        .First().Name;

                var targetClass = objects
                        .Where(x => x.NodeId.Equals(target))
                        .First().Name;

                bool sourceHasToImportTarget = !sourceGroup.Equals(targetGroup);

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
                   objects
                        .Where(x => x.NodeId.Equals(source))
                        .First()
                        .Implements
                        .Add(objects.Where(x => x.NodeId.Equals(target))
                        .First());

                if (sourceHasToImportTarget)
                {
                    objects
                        .Where(x => x.NodeId.Equals(source))
                        .First()
                        .ObjectsToImport
                        .Add(objects.Where(x => x.NodeId.Equals(target))
                        .First());
                }

            }
            // TODO 
            // objects.ForEach(x => gui.log($"Klasse erstellt: {x.ToString()}"));
            //if(objects.Count == 0) gui.log("Es wurden keine Klassen gefunden!");
            return objects;
        }

      
    }
}
