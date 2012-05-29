using System;
using NUnit.Framework;

namespace NUnitSandbox
{
    [TestFixture]
    public class FailingFixture
    {
        [SetUp]
        public void SetUp()
        {
            // this setup method fails

            throw new InvalidOperationException( "SetUp has had a problem :-(" );
        }

        [Test]
        public void ProbablyWontRun()
        {
        }
    }
}