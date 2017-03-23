using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ENSolver;

namespace UnitTestENSolver
{
    //[TestClass]
    public class SpellCheckDictionaryTests
    {
        [TestMethod]
        public void SpellCheckDictionary_True1()
        {
            SpellCheckDictionary scd = new SpellCheckDictionary();
            Assert.IsTrue(scd.Check("РодИнА"));
        }
        [TestMethod]
        public void SpellCheckDictionary_True2()
        {
            SpellCheckDictionary scd = new SpellCheckDictionary();
            Assert.IsTrue(scd.Check("париЖ"));
        }
        [TestMethod]
        public void SpellCheckDictionary_False()
        {
            SpellCheckDictionary scd = new SpellCheckDictionary();
            Assert.IsFalse(scd.Check("овыраплоырвлапрфщ"));
        }


    }
}
