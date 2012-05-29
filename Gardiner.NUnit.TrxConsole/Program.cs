using System;
using Gardiner.NUnit.TrxConsole.Core;

namespace Gardiner.NUnit.ConsoleTrx
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static int Main(string[] args)
        {
            return new TrxRunner().Main(args);
        }
    }
}
