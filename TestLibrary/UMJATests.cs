﻿using UMJA;
using UMJA.Utility;
using Xunit;
using Xunit.Abstractions;
using System.Linq;

namespace TestLibrary {
    public class UMJATests {
        private readonly ITestOutputHelper output;

        public UMJATests(ITestOutputHelper output) {
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

            Assert.Equal(variables.Count, expectedVariableCount);
            Assert.True(variables.Select(x => x.Name).Contains(expectedVariableName1));
            Assert.True(variables.Select(x => x.Name).Contains(expectedVariableName2));

            Assert.Equal(methods.Count, expectedMethodCount);
            Assert.True(methods.Select(x => x.Name).Contains(expectedMethodName1));
            Assert.True(methods.Select(x => x.Name).Contains(expectedMethodName2));
            Assert.True(methods.Select(x => x.Name).Contains(expectedMethodName3));


        }
    }
}
