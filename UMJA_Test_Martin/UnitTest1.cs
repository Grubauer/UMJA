using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UMJA;

namespace UMJA_Test_Martin
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestMethod1() //Check if projectforder exists  !!!Wichtig alle Aufrufe der Methode guiLog müssen beim Testen auskommentiert werden!!!
        {
            var javaObject = Logic.ReadDocument(@"E:\4HTL\AUD\UMLS\Test1.graphml");
            Logic.CreateProject(javaObject, @"C:\Users\mgebh\Desktop\", "Test1");

            bool expRes = true;
            bool res = false;
            if (Directory.Exists(@"C:\Users\mgebh\Desktop\Test1")) res = true;

            Assert.AreEqual(expRes, res);
        }

        [TestMethod]
        public void TestMethod2() //Check folders1  
        {
            var javaObject = Logic.ReadDocument(@"E:\4HTL\AUD\UMLS\Test1.graphml");
            Logic.CreateProject(javaObject, @"C:\Users\mgebh\Desktop\", "Test1");


            bool expRes = true;
            bool res = false;
            if (Directory.Exists(@"C:\Users\mgebh\Desktop\Test1\net")) res = true;

            Assert.AreEqual(expRes, res);
        }

        [TestMethod]
        public void TestMethod3() //Check folders2 
        {
            var javaObject = Logic.ReadDocument(@"E:\4HTL\AUD\UMLS\Test1.graphml");
            Logic.CreateProject(javaObject, @"C:\Users\mgebh\Desktop\", "Test1");


            bool expRes = true;
            bool res = false;
            if (Directory.Exists(@"C:\Users\mgebh\Desktop\Test1\net\htlgrieskirchen")) res = true;

            Assert.AreEqual(expRes, res);
        }

        [TestMethod]
        public void TestMethod4() //Check folders3  
        {
            var javaObject = Logic.ReadDocument(@"E:\4HTL\AUD\UMLS\Test1.graphml");
            Logic.CreateProject(javaObject, @"C:\Users\mgebh\Desktop\", "Test1");


            bool expRes = true;
            bool res = false;
            if (Directory.Exists(@"C:\Users\mgebh\Desktop\Test1\net\htlgrieskirchen\test1")) res = true;

            Assert.AreEqual(expRes, res);
        }

        [TestMethod]
        public void TestMethod5() //Check files  
        {
            var javaObject = Logic.ReadDocument(@"E:\4HTL\AUD\UMLS\Test1.graphml");
            Logic.CreateProject(javaObject, @"C:\Users\mgebh\Desktop\", "Test1");


            bool expRes = true;
            bool res = false;
            if (File.Exists(@"C:\Users\mgebh\Desktop\Test1\net\htlgrieskirchen\test1\Student.java") &&
                File.Exists(@"C:\Users\mgebh\Desktop\Test1\net\htlgrieskirchen\test1\School.java")) res = true;

            Assert.AreEqual(expRes, res);
        }

        [TestMethod]
        public void ParseVariables1() //Check variables  
        {
            var variables = UMJA.Utility.Variable.ParseVariables("< y:AttributeLabel > +city : String");

            
            bool result = false;

            foreach (var variable in variables)
            {
                if (variable.IsFinal == false && variable.IsPrivate == false &&
                    variable.IsStatic == false && variable.Name == "city" && variable.ObjectType == "String"
                    && variable.DefinedValue == null) result = true; 
            }

            Assert.AreEqual(true, result);
        }

        public void ParseVariables2() //Check variables  
        {
            var variables = UMJA.Utility.Variable.ParseVariables("< y:AttributeLabel > +city : String");


            bool result = false;

            foreach (var variable in variables)
            {
                if (variable.IsFinal == false && variable.IsPrivate == false &&
                    variable.IsStatic == false && variable.Name == "city" && variable.ObjectType == "String"
                    && variable.DefinedValue == null) result = true;
            }

            Assert.AreEqual(true, result);
        }
    }
}
