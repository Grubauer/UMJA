using System;
using System.Collections.Generic;
using System.IO;
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
        public static List<JavaObject> ReadDocument(string path)
        {

            if (!path.EndsWith("graphml"))
            {
                guiLog("Die angegebene Datei ist keine graphml Datei!");
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
                    bool hasConstructor = methodString.ToLower().Contains("konstruktor");
                    bool hasSetter = methodString.ToLower().Contains("setter");
                    bool hasGetter = methodString.ToLower().Contains("getter");

                    List<Variable> variables = new List<Variable>();
                    if (variableString != null && !variableString.Equals(string.Empty)) variables = Variable.ParseVariables(variableString);

                    if (stereotype.Equals("interface"))
                        objects.Add(new JavaInterface { Name = className, Methods = methods, Variables = variables, NodeId = nodeId });
                    else
                        objects.Add(new JavaClass { Name = className, Methods = methods, Variables = variables, NodeId = nodeId, HasConstructor = hasConstructor, HasGetter = hasGetter, HasSetter = hasSetter });
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

                            if (polyLineEdgeChild.Name.Equals("y:Arrows"))
                            {


                                if (polyLineEdgeChild.Attributes["source"] != null)
                                    sourceArrowType = polyLineEdgeChild.Attributes["source"].Value;
                                if (polyLineEdgeChild.Attributes["target"] != null)
                                    targetArrowType = polyLineEdgeChild.Attributes["target"].Value;
                            }
                            else if (polyLineEdgeChild.Name.Equals("y:LineStyle") && polyLineEdgeChild.Attributes["type"] != null)
                            {
                                lineType = polyLineEdgeChild.Attributes["type"].Value;
                            }
                        }
                    }
                }

                //implements:
                if (lineType.Equals("dashed") && targetArrowType.Equals("white_delta"))
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

            objects.ForEach(x => guiLog($"Klasse erstellt: {x.ToString()}"));
            if (objects.Count == 0) guiLog("Es wurden keine Klassen gefunden!");
            //objects.ForEach(x => Console.WriteLine($"Klasse erstellt: {x.ToString()}"));
            return objects;
        }

        private static void guiLog(string msg)
        {
            (Application.Current.Windows[0] as MainWindow).lsbLogConsole.Items.Add(msg);
        }

        public static void CreateProject(List<JavaObject> javaObjects, string targetpath, string folderName)
        {

            //string subdir = $"C:/Users/sofia/Favorites/FolderTest";
            string subdir = $"{targetpath}/{folderName}";
            // If directory does not exist, create it.
            if (!Directory.Exists(subdir))
            {
                Directory.CreateDirectory(subdir);
            }

            foreach (var item in javaObjects)
            {        // TODO
                     // einzelne Unterpunkte!;
                     //string pathPackages = @"C:/Users/sofia/Favorites/FolderTest";
                string pathPackages = subdir;

                var splitPath = item.Package.Split('.');
                foreach (var p in splitPath)
                {
                    pathPackages = pathPackages + "/" + p;
                    if (!Directory.Exists(pathPackages))
                    {
                        Directory.CreateDirectory(pathPackages);
                    }
                }

            }
            foreach (var item in javaObjects)
            {
                string pathPackages = "";
                var splitPath = item.Package.Split('.');
                foreach (var p in splitPath)
                {
                    pathPackages = pathPackages + "/" + p;
                    if (!Directory.Exists(pathPackages))
                    {
                        Directory.CreateDirectory(pathPackages);
                    }
                }
                string path = $"{subdir}/{pathPackages}/{item.Name}.java";
                //string path = @"C:/Users/sofia/Favorites/FolderTest/" + pathPackages + "/" + item.Name + ".java";
                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine($"package {item.Package};");
                        sw.WriteLine();
                        foreach (var impl in item.ObjectsToImport)
                        {
                            sw.WriteLine("import " + impl.Package + "." + impl.Name + ";");
                        }
                        if (item.GetType().ToString().Equals("UMJA.Utility.JavaClass"))
                        {
                            

                            JavaClass javaClass = (JavaClass)item;
                            string imp = (javaClass.Implements.Count == 0) ? " " : " implements";


                            foreach (var i in javaClass.Implements)
                            {
                                imp = imp + " " + i.Name;
                            }
                            sw.WriteLine();
                            sw.WriteLine("public class " + item.Name + imp + " {");
                            foreach (var vari in javaClass.Variables)
                            {
                                var final = (vari.IsFinal) ? "final" : "";
                                var priv = (vari.IsPrivate) ? "private" : "public";
                                var stat = (vari.IsStatic) ? "static" : "";

                                sw.WriteLine(priv + " " + stat + " " + final + " " + vari.ObjectType + " " + vari.Name + ";");
                            }
                            foreach (var method in javaClass.Methods)
                            {

                                var priv = (method.IsPrivate) ? "private" : "public";
                                var stat = (method.IsStatic) ? "static" : "";

                                string parameter = "";
                                foreach (var variable in method.Parameters)
                                {
                                    parameter += variable.ObjectType + " " + variable.Name + ", ";
                                }
                                if (parameter.Length > 0)
                                {
                                    parameter = parameter.Substring(0, parameter.Length - 2);
                                }

                                if (Object.Equals(method.Name, "toString"))
                                {
                                    sw.WriteLine(Environment.NewLine + "@Override");
                                    sw.WriteLine(priv + " " + stat + " " + " " + method.ReturnObject + " " + method.Name + "(" + parameter + ")" + " {" + Environment.NewLine + "   return \"\";" + Environment.NewLine + "}");
                                }
                                else if(method.Name.StartsWith("get") && javaClass.Variables.FindAll(x=> method.Name.Contains(x.Name.ToLower())).Count != 0)
                                {
                                    sw.WriteLine(Environment.NewLine + priv + " " + stat + " " + " " + method.ReturnObject + " " + method.Name + "() {");
                                    sw.WriteLine($"return {javaClass.Variables.FindAll(x => method.Name.Contains(x.Name.ToLower())).First().Name};" );
                                    sw.WriteLine("}");
                                }
                                else if (method.Name.StartsWith("set") && javaClass.Variables.FindAll(x => method.Name.ToLower().Contains(x.Name.ToLower())).Count > 0)
                                {
                                    sw.WriteLine(Environment.NewLine + priv + " " + stat + " " + " " + method.ReturnObject + " " + method.Name + "(" + parameter + ")" + " {");
                                    sw.WriteLine($"this.{method.Parameters[0].Name} = {method.Parameters[0].Name};");
                                    sw.WriteLine("}");
                                }
                                else
                                    sw.WriteLine(Environment.NewLine + priv + " " + stat + " " + " " + method.ReturnObject + " " + method.Name + "(" + parameter + ")" + " {" + Environment.NewLine + Environment.NewLine + "}");
                            }

                            if (javaClass.HasConstructor)
                            {
                                var variablesForConstSB = new StringBuilder();
                                javaClass.Variables.FindAll(x => !x.IsStatic).ToList().ForEach(x => variablesForConstSB.Append($"{x.ObjectType} {x.Name}, "));

                                sw.WriteLine($"public {javaClass.Name}({variablesForConstSB.ToString().Remove(variablesForConstSB.ToString().Length - 2, 2)}) " + "{");
                                javaClass.Variables.ForEach(x => sw.WriteLine($"this.{x.Name} = {x.Name};"));
                                sw.WriteLine("}");
                                sw.WriteLine("");
                            }

                            if (javaClass.HasGetter)
                                javaClass.Variables.ForEach(x => sw.WriteLine($"public {x.ObjectType} get{x.Name.ToCharArray()[0].ToString().ToUpper()}{x.Name.Remove(0, 1)}() " + "{" + Environment.NewLine + $"    return {x.Name};" + Environment.NewLine + "}"));

                            if (javaClass.HasSetter)
                                javaClass.Variables.ForEach(x => sw.WriteLine($"public {x.ObjectType} set{x.Name.ToCharArray()[0].ToString().ToUpper()}{x.Name.Remove(0, 1)}({x.ObjectType} {x.Name}) "
                                    + "{"
                                    + Environment.NewLine
                                    + $"    this.{x.Name} =  {x.Name};"
                                    + Environment.NewLine
                                    + "}"));


                            sw.WriteLine("}");
                        }

                        else if (item.GetType().ToString().Equals("UMJA.Utility.JavaEnumeration"))
                        {
                            JavaEnumeration javaEnumeration = (JavaEnumeration)item;
                            sw.WriteLine("public enum " + item.Name + "{");
                            sw.WriteLine(javaEnumeration.Values);
                            sw.WriteLine("}");
                        }

                        else if (item.GetType().ToString().Equals("UMJA.Utility.JavaInterface"))
                        {
                            JavaInterface javaClass = (JavaInterface)item;
                            sw.WriteLine("public interface " + item.Name + "{");
                            foreach (var vari in javaClass.Variables)
                            {
                                var final = (vari.IsFinal) ? "final" : "";
                                var priv = (vari.IsPrivate) ? "private" : "public";
                                var stat = (vari.IsStatic) ? "static" : "";

                                sw.WriteLine(priv + " " + stat + " " + final + " " + vari.ObjectType + " " + vari.Name);

                            }
                            foreach (var method in javaClass.Methods)
                            {

                                var priv = (method.IsPrivate) ? "private" : "public";
                                var stat = (method.IsStatic) ? "static" : "";

                                string parameter = "";
                                foreach (var variable in method.Parameters)
                                {
                                    parameter += parameter + variable.ObjectType + " " + variable.Name + ", ";
                                }
                                if (parameter.Length > 0)
                                {
                                    parameter = parameter.Substring(0, parameter.Length - 2);
                                }


                                sw.WriteLine(priv + " " + stat + " " + " " + method.ReturnObject + " " + method.Name + " (" + parameter + ");");
                            }


                            sw.WriteLine("}");
                        }






                    }
                }
            }
        }





    }
}
