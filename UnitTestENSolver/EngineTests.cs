// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ENSolver;

namespace UnitTestENSolver
{
    //[TestClass]
    public class EngineTests
    {
        [TestMethod]
        public void Registry_SetGet_true()
        {
            Registry r = new Registry();
            UserInfo ui = new UserInfo("user123", "pass789");
            r.SetUserInfo(ui);
            UserInfo ui2 = r.GetUserInfo();
            Assert.AreEqual("user123", ui2.name);
            Assert.AreEqual("pass789", ui2.pass);
        }
        [TestMethod]
        public void Registry_SetGet_false()
        {
            Registry r = new Registry();
            UserInfo ui = new UserInfo("user123", "pass789");
            r.SetUserInfo(ui);
            UserInfo ui2 = r.GetUserInfo();
            Assert.AreNotEqual("us5er123", ui2.name);
            Assert.AreNotEqual("pa5ss789", ui2.pass);
        }
        [TestMethod]
        public void Engine_Logon_true()
        {
            Engine en = new Engine();
            UserInfo ui = new UserInfo("полвторого", "ovs122");
            Assert.IsTrue(en.Logon(ui));
        }
        [TestMethod]
        public void Engine_Logon_false_1()
        {
            Engine en = new Engine();
            UserInfo ui = new UserInfo("полвт442орого", "o123vs12hhf2");
            Assert.IsFalse(en.Logon(ui));
        }
        [TestMethod]
        public void Engine_Logon_false_2()
        {
            Engine en = new Engine();
            UserInfo ui = new UserInfo("полвторого", "ovs123");
            Assert.IsFalse(en.Logon(ui));
        }
    }
}
