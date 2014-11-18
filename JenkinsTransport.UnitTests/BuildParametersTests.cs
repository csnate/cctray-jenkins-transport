using JenkinsTransport.BuildParameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Xml.Linq;

namespace JenkinsTransport.UnitTests
{
    [TestClass]
    public class BuildParametersTests
    {
        [TestMethod]
        public void TestChoiceParameter()
        {
            var xml = @"<parameterDefinition>
                        <defaultParameterValue>
                        <value>ONE</value>
                        </defaultParameterValue>
                        <description>Select a choice</description>
                        <name>CHOICE.1</name>
                        <type>ChoiceParameterDefinition</type>
                        <choice>ONE</choice>
                        <choice>TWO</choice>
                        <choice>THREE</choice>
                        </parameterDefinition>";

            var xDoc = XDocument.Parse(xml).Descendants().First();
            var choice = new ChoiceParameter(xDoc);

            Assert.AreEqual(choice.Name, "CHOICE.1");
            Assert.AreEqual(choice.Description, "Select a choice");
            Assert.AreEqual(choice.ParameterType, BuildParameterType.ChoiceParameterDefinition);
            Assert.AreEqual(choice.DefaultValue, "ONE");

            CollectionAssert.AllItemsAreNotNull(choice.Options);
            CollectionAssert.AllItemsAreUnique(choice.Options);

            // Check the name/value pairs
            Assert.AreEqual(choice.Options.Length, 3);
            Assert.AreEqual(choice.Options[0].Value, "ONE");
            Assert.AreEqual(choice.Options[0].Name, "ONE");

            Assert.AreEqual(choice.Options[1].Value, "TWO");
            Assert.AreEqual(choice.Options[1].Name, "TWO");

            Assert.AreEqual(choice.Options[2].Value, "THREE");
            Assert.AreEqual(choice.Options[2].Name, "THREE");
        }

        [TestMethod]
        public void TestBooleanParameter()
        {
            var xml = @"<parameterDefinition>
                        <defaultParameterValue>
                        <value>true</value>
                        </defaultParameterValue>
                        <description>Select a checkbox option</description>
                        <name>BOOLEAN.1</name>
                        <type>BooleanParameterDefinition</type>
                        </parameterDefinition>";

            var xDoc = XDocument.Parse(xml).Descendants().First();
            var boolean = new BooleanParameter(xDoc);

            Assert.AreEqual(boolean.Name, "BOOLEAN.1");
            Assert.AreEqual(boolean.Description, "Select a checkbox option");
            Assert.AreEqual(boolean.ParameterType, BuildParameterType.BooleanParameterDefinition);
            Assert.AreEqual(boolean.DefaultValue, "true");

            // Check the ParameterBase values
            var ccNetBooleanParameter = boolean.ToParameterBase();
        }

        [TestMethod]
        public void TestStringParameter()
        {
            var xml = @"<parameterDefinition>
                        <defaultParameterValue>
                        <value>this is a default string</value>
                        </defaultParameterValue>
                        <description>Enter a string</description>
                        <name>STRING.1</name>
                        <type>StringParameterDefinition</type>
                        </parameterDefinition>";

            var xDoc = XDocument.Parse(xml).Descendants().First();
            var str = new StringParameter(xDoc);

            Assert.AreEqual(str.Name, "STRING.1");
            Assert.AreEqual(str.Description, "Enter a string");
            Assert.AreEqual(str.ParameterType, BuildParameterType.StringParameterDefinition);
            Assert.AreEqual(str.DefaultValue, "this is a default string");
        }

        [TestMethod]
        public void TestStringParameterWithEmptyDefault()
        {
            var xml = @"<parameterDefinition>
                        <defaultParameterValue>
                        <value />
                        </defaultParameterValue>
                        <description>Enter a string</description>
                        <name>STRING.1</name>
                        <type>StringParameterDefinition</type>
                        </parameterDefinition>";

            var xDoc = XDocument.Parse(xml).Descendants().First();
            var str = new StringParameter(xDoc);

            Assert.AreEqual(str.Name, "STRING.1");
            Assert.AreEqual(str.Description, "Enter a string");
            Assert.AreEqual(str.ParameterType, BuildParameterType.StringParameterDefinition);
            Assert.AreEqual(str.DefaultValue, String.Empty);
        }
    }
}
