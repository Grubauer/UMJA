using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace UMJA.Utility
{
    public class Variable
    {
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public string ObjectType { get; set; }
        public bool IsStatic { get; set; }
        public bool IsFinal { get; set; }
        public string DefinedValue { get; set; }
        public static List<Variable> ParseVariables(string vString)
        {
            bool staticV = vString.Contains("<u>");

            
            vString = Regex.Replace(vString, "<.*?>", string.Empty);
            string[] variableStrings = vString.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            List<Variable> variables = new List<Variable>();
            foreach (var varString in variableStrings)
            {
               ;
                if (varString.Trim().StartsWith("+") || varString.Trim().StartsWith("-") || varString.Trim().StartsWith("#"))
                {
                    string definedValue = null;
                    string name;
                    if (varString.Contains("="))
                    {
                        definedValue = varString.Split(':')[0].Split('=')[1].Trim();
                        name = varString.Split(':')[0].Split('=')[0].Trim().Remove(0, 1).Trim();
                    }
                    else name = varString.Split(':')[0].Trim().Remove(0,1).Trim();
                        
                    bool isPrivate = varString.StartsWith("-");
                   
                    string objectType = varString.Split(':')[1].Trim();
                    bool isFinal = name.ToUpper().Equals(name);
                    

                    variables.Add(new Variable { Name = name, IsPrivate = isPrivate, IsStatic = staticV, IsFinal = isFinal, ObjectType = objectType , DefinedValue = definedValue});
                }

            }
            
            return variables;
        }
    }
}