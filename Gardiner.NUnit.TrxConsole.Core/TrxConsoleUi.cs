using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.ConsoleRunner;
using NUnit.Core;

namespace Gardiner.NUnit.TrxConsole.Core
{
    public class TrxConsoleUi : ConsoleUi
    {
        private readonly Dictionary<string, Output> _testOutput = new Dictionary<string, Output>();

        protected override EventCollector CollectorFactory
            ( ConsoleOptions options, TextWriter errorWriter, TextWriter outWriter )
        {
            return new TrxEventCollector( options, errorWriter, outWriter, _testOutput );
        }

        protected override string CreateXmlOutput( TestResult result )
        {
            var builder = new StringBuilder();
            new XmlTrxWriter( new StringWriter( builder ) ).SaveTestResult( result, _testOutput );

            return builder.ToString();
        }
    }
}