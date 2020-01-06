using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMJA;
using UMJA.Utility;
using Xunit;
using Xunit.Abstractions;

namespace TestLibrary {
    public class UMJATest {
        private readonly ITestOutputHelper output;

        public UMJATest(ITestOutputHelper output) {
            this.output = output;
        }

        [Fact]
        public void T01_TestReadFile() {
            string path = "testGraph.graphml";

            var results = Logic.ReadDocument(path);
            var itemClass = (JavaClass)results[0];

            string expectedName = "TestClass";

            int expectedVariableCount = 2;
            string expectedVariableName1 = "name";
            string expectedVariableName2 = "price";

            int expectedMethodCount = 3;
            string expectedMethodName1 = "setName";
            string expectedMethodName2 = "setPrice";
            string expectedMethodName3 = "toString";

            string name = itemClass.Name;
            var variables = itemClass.Variables;
            var methods = itemClass.Methods;

            Assert.Equal(expectedName, name);

            Assert.Equal(expectedVariableCount, variables.Count);
            Assert.True(variables.Select(x => x.Name).Contains(expectedVariableName1));
            Assert.True(variables.Select(x => x.Name).Contains(expectedVariableName2));

            Assert.Equal(expectedMethodCount, methods.Count);
            Assert.True(methods.Select(x => x.Name).Contains(expectedMethodName1));
            Assert.True(methods.Select(x => x.Name).Contains(expectedMethodName2));
            Assert.True(methods.Select(x => x.Name).Contains(expectedMethodName3));

        }

        [Fact]
        public void T02_TestParseMethod_Multiple() {
            string methodString = "+ setName() : void\r\n+ setPrice() : void\r\n+ toString() : void";
            var methods = Method.ParseMethods(methodString);

            int expectedCount = 3;
            string expectedMethodName1 = "setName";
            string expectedMethodName2 = "setPrice";
            string expectedMethodName3 = "toString";

            Assert.Equal(expectedCount, methods.Count);
            Assert.True(methods.Select(x => x.Name).Contains(expectedMethodName1));
            Assert.True(methods.Select(x => x.Name).Contains(expectedMethodName2));
            Assert.True(methods.Select(x => x.Name).Contains(expectedMethodName3));


        }

        [Fact]
        public void T03_TestParseMethod_Parameter() {
            string methodString = "+ setName(name: String) : void";
            var methods = Method.ParseMethods(methodString);

            int expectedCount = 1;
            string expectedMethodName1 = "setName";
            string expectedParameterName = "name";
            string expectedParameterType = "String";

            Assert.Equal(expectedCount, methods.Count);
            Assert.True(methods.Select(x => x.Name).Contains(expectedMethodName1));
            Assert.Equal(expectedParameterName, methods[0].Parameters[0].Name);
            Assert.Equal(expectedParameterType, methods[0].Parameters[0].ObjectType);

        }

    }
}
