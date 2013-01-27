using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace JenkinsTransport.Tests
{
    [TestFixture]
    public class JenkinsProjectManagerTests
    {
        [Test]
        public void TestAssemblyName()
        {
            Type t = typeof(ThoughtWorks.CruiseControl.CCTrayLib.Configuration.ICCTrayMultiConfiguration);
            string s = t.Assembly.FullName.ToString();
            Console.WriteLine("The fully qualified assembly name is {0}.", s);
        }
    }
}
