// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ENSolver;

namespace UnitTestENSolver
{
    //[TestClass]
    public class RegistryTests
    {
        [TestMethod]
        public void Registry_SetGet_1()
        {
            Registry reg = new Registry();
            UserInfo ui = new UserInfo("username1", "password1");
            reg.SetUserInfo(ui);
            UserInfo res_ui = reg.GetUserInfo();
            Assert.AreEqual("username1", res_ui.name);
            Assert.AreEqual("password1", res_ui.pass);
        }
        [TestMethod]
        public void Registry_SetGet_2()
        {
            Registry reg = new Registry(new UserInfo("username2", "password2"));
            Assert.AreEqual("username2", reg.GetUserInfo().name);
            Assert.AreEqual("password2", reg.GetUserInfo().pass);
        }
    }
}
