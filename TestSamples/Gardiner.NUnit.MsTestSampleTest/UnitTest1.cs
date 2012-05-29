using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace MsTestSampleTest
{
    /// <summary>
    ///     Summary description for UnitTest1
    /// </summary>
    [TestClass]
    [TestFixture]
    public class UnitTest1
    {
        ///<summary>
        ///    Gets or sets the test context which provides information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        [TestMethod]
        [Test]
        public void TestMethod1()
        {
            Console.WriteLine( "<Console>" );
            Trace.WriteLine( "Trace" );
            Debug.WriteLine( "Debug" );

            if ( TestContext != null )
                TestContext.WriteLine( "TestContext" );
        }

        [TestMethod]
        [Test]
        public void WithAssert()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        [Test]
        public void AssertFails()
        {
            Assert.IsTrue(false, "Surprisingly, this test failed");
        }

        [TestMethod]
        [Test]
        public void ThrowsUnexpected()
        {
            throw new InvalidOperationException("An uncaught exception");
        }

        [TestMethod]
        [Test]
        [Category( "ACategory" )]
        [Category( "BCategory" )]
        [TestCategory("ACategory")]
        [TestCategory("BCategory")]
        public void CategorisedTest()
        {

        }

        [TestMethod]
        [Test]
        [NUnit.Framework.Ignore( "Just because" )]
        public void IgnoredTest()
        {

        }

        [TestMethod]
        [Test]
        [Explicit( "Only run explicitly" )]
        public void ExplicitTest()
        {

        }

        [TestMethod]
        [Test]
        public void SlowTest()
        {
            System.Threading.Thread.Sleep( 1750 );
        }

        [TestMethod]
        [Test]
        [Category( "ExcludedCategory" )]
        [TestCategory( "ExcludedCategory" )]
        public void ExcludedTestViaCategory()
        {

        }

    }
}