using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ENSolver;

namespace UnitTestENSolver
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(1, Class1.test1());
        }
        [TestMethod]
        public void TestMethod2()
        {
            Assert.AreEqual(2, Program.test2());
        }
    }
}
