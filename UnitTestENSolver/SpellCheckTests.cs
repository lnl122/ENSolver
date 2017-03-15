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
        public void SpellCheck_Init()
        {
            SpellCheck.Init();
            Assert.IsTrue(SpellCheck.isObjectReady);
        }
        [TestMethod]
        public void SpellCheck_WordExist()
        {
            Assert.IsTrue(SpellCheck.CheckMsWord());
        }
        [TestMethod]
        public void SpellCheck_DictExist()
        {
            string fn = Environment.CurrentDirectory + "\\Data\\SpChDict.dat";
            Assert.IsTrue(System.IO.File.Exists(fn));
        }
        [TestMethod]
        public void SpellCheck_LoadDictionary()
        {
            SpellCheck.Init();
            SpellCheck.LoadDictionary();
            Assert.AreNotEqual(0, SpellCheck.dict1.Count);
        }
        [TestMethod]
        public void SpellCheck_CheckOne_True()
        {
            SpellCheck.Init();
            SpellCheck.LoadDictionary();
            SpellCheck SC = new SpellCheck();
            Assert.IsTrue(SC.CheckOne("капуста"));
            SC.Close();
        }
        [TestMethod]
        public void SpellCheck_CheckOne_False()
        {
            SpellCheck.Init();
            SpellCheck.LoadDictionary();
            SpellCheck SC = new SpellCheck();
            Assert.IsFalse(SC.CheckOne("капуsfgsfgста"));
            SC.Close();
        }
        [TestMethod]
        public void SpellCheck_CapitalizeWord_True()
        {
            Assert.AreEqual("Париж", SpellCheck.CapitalizeWord("пАрИж"));
        }
        [TestMethod]
        public void SpellCheck_CapitalizeWord_False()
        {
            Assert.AreNotEqual("париж", SpellCheck.CapitalizeWord("пАрИж"));
        }
        [TestMethod]
        public void SpellCheck_Check_True()
        {
            SpellCheck.Init();
            SpellCheck.LoadDictionary();
            SpellCheck SC = new SpellCheck();
            Assert.IsTrue(SC.CheckOne("капуста"));
            SC.Close();
        }
        [TestMethod]
        public void SpellCheck_Check_False()
        {
            SpellCheck.Init();
            SpellCheck.LoadDictionary();
            SpellCheck SC = new SpellCheck();
            Assert.IsFalse(SC.CheckOne("капуsfgsfgста"));
            SC.Close();
        }
        [TestMethod]
        public void SpellCheck_CheckBlock()
        {
            SpellCheck.Init();
            SpellCheck.LoadDictionary();
            SpellCheck SC = new SpellCheck();
            List<string> wrds = new List<string>() { "мама", "мыла", "бляхамухаминуспять", "раму", "веселоебали" };
            List<string> res = new List<string>() { "мама", "мыла", "раму" };
            List<string> res2 = SC.CheckBlock(wrds);
            Assert.AreEqual(res.Count, res2.Count);
            Assert.AreEqual(res[0], res2[0]);
            Assert.AreEqual(res[1], res2[1]);
            Assert.AreEqual(res[2], res2[2]);
            SC.Close();
        }
        [TestMethod]
        public void SpellCheck_Check1000()
        {
            SpellCheck.Init();
            SpellCheck.LoadDictionary();
            SpellCheck SC = new SpellCheck();
            SpellCheck.maxCntWords = 10;
            List<string> wrds = new List<string>() { "мама", "мыла", "бляхамухаминуспять", "раму", "веселоебали", "мама", "мыла", "бляхамухаминуспять", "раму", "веселоебали", "мама", "мыла", "раму" };
            List<string> res = new List<string>() { "мама", "мыла", "раму", "мама", "мыла", "раму", "мама", "мыла", "раму" };
            List<string> res2 = SC.Check(wrds);
            Assert.AreEqual(res.Count, res2.Count);
            for(int i = 0; i < 9; i++)
            {
                Assert.AreEqual(res[i], res2[i]);
            }
            SpellCheck.maxCntWords = 1000;
            SC.Close();
        }
        [TestMethod]
        public void SpellCheck_Check1000_woDict()
        {
            SpellCheck.Init();
            SpellCheck SC = new SpellCheck();
            SpellCheck.maxCntWords = 10;
            List<string> wrds = new List<string>() { "мама", "мыла", "бляхамухаминуспять", "раму", "веселоебали", "мама", "мыла", "бляхамухаминуспять", "раму", "веселоебали", "мама", "мыла", "раму" };
            List<string> res = new List<string>() { "мама", "мыла", "раму", "мама", "мыла", "раму", "мама", "мыла", "раму" };
            List<string> res2 = SC.Check(wrds);
            Assert.AreEqual(res.Count, res2.Count);
            for (int i = 0; i < 9; i++)
            {
                Assert.AreEqual(res[i], res2[i]);
            }
            SpellCheck.maxCntWords = 1000;
            SC.Close();
        }
        //
    }
}
