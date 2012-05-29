using System;

namespace Gardiner.NUnit.TrxConsole.Core
{
    public class TrxRunner : Runner
    {
        protected override ConsoleUi ConsoleFactory()
        {
            return new TrxConsoleUi();
        }
    }
}