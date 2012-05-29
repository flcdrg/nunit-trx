using System;
using System.Diagnostics;
using NUnit.Framework;

namespace NUnitSandbox
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void ATest()
        {
            Console.WriteLine("Console");
            Trace.WriteLine( "Trace" );
            Debug.WriteLine( "<Debug>" );
            // Arrange
            
            // Act

            // Assert
            Assert.IsTrue(true);
        }

        [Test]
        public void BTest()
        {
            Console.WriteLine( "Console B" );
            Trace.WriteLine( "Trace B" );
            Debug.WriteLine( "Debug B" );
            // Arrange

            // Act

            // Assert
            Assert.IsTrue( true );
        }

        [Test]
        [Category("ACategory")]
        public void CategorisedTest()
        {

        }

        [Test]
        [Ignore("Just because")]
        public void IgnoredTest()
        {

        }

        [Test]
        [Explicit("Only run explicitly")]
        public void ExplicitTest()
        {

        }

        [Test]
        public void SlowTest()
        {
            System.Threading.Thread.Sleep(1750);
        }

        [Test]
        [Category("ExcludedCategory")]
        public void ExcludedTestViaCategory()
        {

        }

        [Test]
        public void AssertFails()
        {
            Assert.IsTrue( false, "Surprisingly, this test failed" );
        }

        [Test]
        public void ThrowsUnexpected()
        {
            throw new InvalidOperationException( "An uncaught exception" );
        }
    }
}
