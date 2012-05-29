using System;

namespace Gardiner.NUnit.TrxConsole.Core
{
    public class Output
    {
        // these correspond to the NUnit.TestOutputType enums
        public string Error { get; set; }
        public string Log { get; set; }
        public string Out { get; set; }
        public string Trace { get; set; }

        public Output()
        {
            Error = string.Empty;
            Log = string.Empty;
            Out = string.Empty;
            Trace = string.Empty;
        }
    }
}