using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NUnit.Framework;

using Assert = NUnit.Framework.Assert;

namespace MsTestSampleTest
{
    [TestClass]
    [TestFixture]
    public class GenericTest : GenericBase<string>
    {

        [TestMethod]
        [Test]
        public void TestMethod1()
        {
            Assert.IsTrue(true);
        }
    }

    [TestClass]
    [TestFixture]
    public class GenericBase<T>
    {

        [TestMethod]
        [Test]
        public void TestMethod2()
        {
            Assert.IsTrue(true);
        }

    }
}
