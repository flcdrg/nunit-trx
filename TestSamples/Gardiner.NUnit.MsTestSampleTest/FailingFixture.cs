using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace MsTestSampleTest
{
    [TestFixture]
    [TestClass]
    public class FailingFixture
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

        [TestInitialize]
        [SetUp]
        public void SetUp()
        {
            // this setup method fails
            throw new InvalidOperationException( "SetUp has had a problem :-(" );
        }

        [Test]
        [TestMethod]
        public void ProbablyWontRun()
        {
        }
    }
}