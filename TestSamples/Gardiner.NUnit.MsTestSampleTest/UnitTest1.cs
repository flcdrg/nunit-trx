using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MsTestSampleTest
{
    /// <summary>
    ///     Summary description for UnitTest1
    /// </summary>
    [TestClass]
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
        public void TestMethod1()
        {
            Console.WriteLine( "<Console>" );
            Trace.WriteLine( "Trace" );
            Debug.WriteLine( "Debug" );

            TestContext.WriteLine( "TestContext" );
        }

        [TestMethod]
        public void WithAssert()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void AssertFails()
        {
            Assert.IsTrue(false, "Surprisingly, this test failed");
        }

        [TestMethod]
        public void ThrowsUnexpected()
        {
            throw new InvalidOperationException("An uncaught exception");
        }

        [TestMethod]
        [Ignore]
        public void IgnoredTest()
        {

        }
    }
}