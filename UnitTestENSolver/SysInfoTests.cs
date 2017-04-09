// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

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
