using System;
using NUnit.Framework;

namespace Gardiner.NUnitTrx.FaultyAssemblyFixtureSetUpTests
{
    [SetUpFixture]
    public class AssemblyFixtureSetUp
    {
        [SetUp]
        public void SetUp()
        {
            throw new InvalidOperationException("Problem setting up Assembly Fixture");
        }
    }
}
