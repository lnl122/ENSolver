using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ENSolver;

namespace UnitTestENSolver
{
    [TestClass]
    public class SysInfoTests
    {
        [TestMethod]
        public void SysInfo_true()
        {
            Assert.AreEqual(true, SysInfo.Check());
        }
    }
}
