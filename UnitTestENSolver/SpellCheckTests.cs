using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ENSolver;

namespace UnitTestENSolver
{
    [TestClass]
    public class SpellCheckTests
    {
        [TestMethod]
        public void SpellCheck_CheckOne_True()
        {
            SpellCheck SC = new SpellCheck();
            Assert.IsTrue(SC.Check("капуста"));
            SC.Close();
        }
        [TestMethod]
        public void SpellCheck_CheckOne_False()
        {
            SpellCheck SC = new SpellCheck();
            Assert.IsFalse(SC.Check("копосто"));
            SC.Close();
        }
        [TestMethod]
        public void SpellCheck_Check_5_3()
        {
            SpellCheck SC = new SpellCheck();
            List<string> wrds = new List<string>() { "мама", "мыла", "бляхамухаминуспять", "раму", "веселоебали" };
            List<string> res = new List<string>() { "мама", "мыла", "раму" };
            List<string> res2 = SC.Check(wrds);
            Assert.AreEqual(res.Count, res2.Count);
            Assert.AreEqual(res[0], res2[0]);
            Assert.AreEqual(res[1], res2[1]);
            Assert.AreEqual(res[2], res2[2]);
            SC.Close();
        }
        [TestMethod]
        public void SpellCheck_Check_13_9()
        {
            SpellCheck SC = new SpellCheck();
            List<string> wrds = new List<string>() { "мама", "мыла", "бляхамухаминуспять", "раму", "веселоебали", "мама", "мыла", "бляхамухаминуспять", "раму", "веселоебали", "мама", "мыла", "раму" };
            List<string> res = new List<string>() { "мама", "мыла", "раму", "мама", "мыла", "раму", "мама", "мыла", "раму" };
            List<string> res2 = SC.Check(wrds);
            Assert.AreEqual(res.Count, res2.Count);
            for(int i = 0; i < 9; i++)
            {
                Assert.AreEqual(res[i], res2[i]);
            }
            SC.Close();
        }

    }
}
