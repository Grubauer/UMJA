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


            if (vString.Contains("<html>"))
            {
                vString = vString.Replace("<html>", "");
                vString = vString.Replace("</html>", "");

                List<Variable> variables = new List<Variable>();
                if (vString.Contains("<br>"))
                {
                    string[] variableStrings = vString.Split(new string[] { "<br>" }, StringSplitOptions.None);
                    foreach (var variableString in variableStrings)
                    {
                        bool isStatic = variableString.Contains("<u>");
                        string modVarString = variableString;
                        if (isStatic)
                        {
                            modVarString = modVarString.Replace("<u>", "");
                            modVarString = modVarString.Replace("</u>", "");
                            variables.Add(ParseNormalVariable(modVarString.Trim(), true));
                        }
                        else
                        {
                            variables.Add(ParseNormalVariable(modVarString.Trim(), false));
                        }
                    }
                }
                else
                {
                    bool isStatic = vString.Contains("<u>");
                    if (isStatic)
                    {
                        vString = vString.Replace("<u>", "");
                        vString = vString.Replace("</u>", "");
                        variables.Add(ParseNormalVariable(vString.Trim(), true));
                    }
                    else
                    {
                        variables.Add(ParseNormalVariable(vString.Trim(), false));
                    }
                }
                return variables;
            }
            else
            {
                string[] variableStrings = vString.Split(new[] { "\n" }, StringSplitOptions.None);
                List<Variable> variables = new List<Variable>();
                foreach (var varString in variableStrings)
                {
                    
                    if (varString.Trim().StartsWith("+") || varString.Trim().StartsWith("-") || varString.Trim().StartsWith("#"))
                    {
                        variables.Add(ParseNormalVariable(varString, false));
                    }

                }

                return variables;
            }



        
            
        }

        private static Variable ParseNormalVariable(string varString, bool isStatc)
        {
            string definedValue = null;
            string name;
            if (varString.Contains("="))
            {
                definedValue = varString.Split(':')[0].Split('=')[1].Trim();
                name = varString.Split(':')[0].Split('=')[0].Trim().Remove(0, 1).Trim();
            }
            else name = varString.Split(':')[0].Trim().Remove(0, 1).Trim();

            bool isPrivate = varString.StartsWith("-");

            string objectType = varString.Split(':')[1].Trim();
            bool isFinal = name.ToUpper().Equals(name);


            return new Variable { Name = name, IsPrivate = isPrivate, IsStatic = isStatc, IsFinal = isFinal, ObjectType = objectType, DefinedValue = definedValue };

        }

    }
}