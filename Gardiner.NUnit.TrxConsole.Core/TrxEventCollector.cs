using System;
using System.Collections.Generic;
using System.IO;
using NUnit.ConsoleRunner;
using NUnit.Core;

namespace Gardiner.NUnit.TrxConsole.Core
{
    public class TrxEventCollector : EventCollector
    {
        private readonly Dictionary<string, Output> _testOutput;

        public TrxEventCollector
            ( ConsoleOptions options, TextWriter outWriter, TextWriter errorWriter,
              Dictionary<string, Output> testOutput ) : base( options, outWriter, errorWriter )
        {
            _testOutput = testOutput;
        }

        public override void TestOutput( TestOutput output )
        {
            base.TestOutput( output );

            Output item;
            if ( !_testOutput.ContainsKey( currentTestName ) )
            {
                item = new Output();
                _testOutput.Add( currentTestName, item );
            }
            else
                item = _testOutput[ currentTestName ];

            string text = output.Text;

            switch ( output.Type )
            {
                case TestOutputType.Error:
                    item.Error += text;
                    break;
                case TestOutputType.Out:
                    item.Out += text;
                    break;
                case TestOutputType.Log:
                    item.Log += text;
                    break;
                case TestOutputType.Trace:
                    item.Trace += text;
                    break;
            }
        }
    }
}