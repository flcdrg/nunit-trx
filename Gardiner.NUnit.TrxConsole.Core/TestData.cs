using System;

namespace Gardiner.NUnit.TrxConsole.Core
{
    internal class TestData
    {
        public TestData( string testId )
        {
            TestId = testId;
        }

        public string TestId { get; private set; }

        public string ExecutionId { get { return TestId; } }
    }
}