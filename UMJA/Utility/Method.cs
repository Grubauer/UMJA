using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UMJA.Utility
{
    public class Method
    {
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public string ReturnObject { get; set; }
        public List<Variable> Parameters { get; set; }
        public bool IsStatic { get; set; }
        public bool Override { get; set; }


        public static List<Method> ParseMethods(string mString)
        {
       
            bool staticM = mString.Contains("<u>");
           
            mString = Regex.Replace(mString, "<.*?>", String.Empty);
            string[] methodStrings = mString.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            List<Method> methods = new List<Method>();
            foreach (var methodString in methodStrings)
            {
                if (methodString.StartsWith("+") || methodString.StartsWith("-") || methodString.StartsWith("#"))
                {
                    bool isPrivate = methodString.StartsWith("-");
                    string name = methodString.Split(' ')[1].Split('(')[0].Replace("(", "");
                    List<Variable> parameters = new List<Variable>();
                    string parameterString = methodString.Split('(')[1].Split(')')[0];
                    if (!parameterString.Trim().Equals(string.Empty))
                        if (parameterString.Contains(","))
                            parameterString.Split(',').ToList().ForEach(x => parameters.Add(new Variable { IsPrivate = false, Name = x.Split(':')[0].Trim(), ObjectType = x.Split(':')[1].Trim() }));
                        else
                            parameters.Add(new Variable { IsPrivate = false, Name = parameterString.Split(':')[0].Trim(), ObjectType = parameterString.Split(':')[1].Trim() });

                    string returnObject = string.Empty;
                    if (methodString.Split(')')[1].Contains(":")) returnObject = methodString.Split(':')[methodString.Split(':').Count() - 1];

                    methods.Add(new Method { Name = name, IsPrivate = isPrivate, ReturnObject = returnObject, Parameters = parameters, IsStatic= staticM });
                }

            }

            return methods;
        }
    }
}