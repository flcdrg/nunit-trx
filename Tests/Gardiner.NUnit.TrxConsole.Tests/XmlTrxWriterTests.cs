using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Gardiner.NUnit.TrxConsole.Core;
using NUnit.Core;
using NUnit.Framework;

using NUnitCore = NUnit.Core;

namespace Gardiner.NUnit.TrxConsole.Tests
{
    [TestFixture]
    public class XmlTrxWriterTests
    {
        [Test]
        public void Create_Object()
        {
            var writer = new StringWriter();
            var sut = new XmlTrxWriter( writer );

            Assert.IsNotNull( sut );
        }

        [Test]
        public void Simple_Document()
        {
            var writer = new StringWriter();
            var sut = new XmlTrxWriter( writer );
            var assembly = new TestAssembly( "c:\\fred.dll" );
            var fixture = new TestFixture( GetType() );
            assembly.Add(fixture);

            var testName = new TestName();
            testName.Name = "Haha";

            fixture.Add( testName );

            var result = new TestResult( assembly );
            //var package = new TestPackage( "Tests", new List<Assembly>() { Assembly.GetExecutingAssembly()});
            var testOutput = new Dictionary<string, Output>();
            sut.SaveTestResult( result, testOutput );

            var output = writer.ToString();

            Trace.WriteLine( output );


        }
    }
}