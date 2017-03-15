using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestENSolver
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(1, ENSolver.Class1.test1());
        }
    }
}
