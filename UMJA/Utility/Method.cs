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

            if (mString.Contains("<html>"))
            {
                mString = mString.Replace("<html>", "");
                mString = mString.Replace("</html>", "");

                List<Method> methods = new List<Method>();
                if (mString.Contains("<br>"))
                {
                    string[] methodStrings = mString.Split(new string[] { "<br>" }, StringSplitOptions.None);
                    foreach (var methodString in methodStrings)
                    {
                        bool isStatic = methodString.Contains("<u>");
                        string modMethString = methodString;
                        if(isStatic)
                        {
                            modMethString = modMethString.Replace("<u>", "");
                            modMethString = modMethString.Replace("</u>", "");
                            methods.Add(ParseNormalMethod(modMethString.Trim(), true));
                        }
                        else
                        {
                            methods.Add(ParseNormalMethod(modMethString.Trim(), false));
                        }
                    }
                }
                else
                {
                    bool isStatic = mString.Contains("<u>");
                    if (isStatic)
                    {
                        mString = mString.Replace("<u>", "");
                        mString = mString.Replace("</u>", "");
                        methods.Add(ParseNormalMethod(mString.Trim(), true));
                    }
                    else
                    {
                        methods.Add(ParseNormalMethod(mString.Trim(), false));
                    }
                }
                return methods;
            }
            else
            {
                bool staticM = mString.Contains("<u>");



                mString = Regex.Replace(mString, "<.*?>", String.Empty);
                string[] methodStrings = mString.Split(new[] { "\n" }, StringSplitOptions.None);
                List<Method> methods = new List<Method>();
                foreach (var methodString in methodStrings)
                {
                    if (methodString.StartsWith("+") || methodString.StartsWith("-") || methodString.StartsWith("#"))
                    {
                        methods.Add(ParseNormalMethod(methodString, false));
                    }

                   
                }
                return methods;
            }
            
       
            
        }

        private static Method ParseNormalMethod(string methodString, bool isStatic)
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

                return new Method { Name = name, IsPrivate = isPrivate, ReturnObject = returnObject, Parameters = parameters, IsStatic = isStatic};
            

        }
    }
}