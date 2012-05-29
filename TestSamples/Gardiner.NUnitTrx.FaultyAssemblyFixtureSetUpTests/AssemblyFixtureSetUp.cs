using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace Gardiner.NUnitTrx.FaultyAssemblyFixtureSetUpTests
{
    [SetUpFixture]
    [TestClass]
    public class AssemblyFixtureSetUp
    {
        [SetUp]
        public void SetUp()
        {
            throw new InvalidOperationException( "Problem call SetUpFixture" );
        }

        [AssemblyInitialize()]
        public static void AssemblyInit( TestContext context )
        {
            throw new InvalidOperationException( "Problem call AssemblyInitialize" );
        }
    }
}
