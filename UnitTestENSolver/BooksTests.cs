using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ENSolver;

namespace UnitTestENSolver
{
    //[TestClass]
    public class BooksTests
    {
        [TestMethod]
        public void Books_Gapo_0()
        {
            BooksDictionary books = new BooksDictionary();
            List<string> res = books.CheckGapoifika("ъъъъъъъ");
            Assert.AreEqual(0, res.Count);
        }
        [TestMethod]
        public void Books_Ledida_0()
        {
            BooksDictionary books = new BooksDictionary();
            List<string> res = books.CheckLedida("ъъъъъъъ");
            Assert.AreEqual(0, res.Count);
        }
        [TestMethod]
        public void Books_Gapo_1()
        {
            BooksDictionary books = new BooksDictionary();
            List<string> res = books.CheckGapoifika("ВоиМи");
            Assert.AreEqual(1, res.Count);
            Assert.IsTrue(res.Contains("Война и мир"));
        }
        [TestMethod]
        public void Books_Ledida_1()
        {
            BooksDictionary books = new BooksDictionary();
            List<string> res = books.CheckLedida("наими");
            Assert.AreEqual(1, res.Count);
            Assert.IsTrue(res.Contains("Война и мир"));
        }
        [TestMethod]
        public void Books_Gapo_2()
        {
            BooksDictionary books = new BooksDictionary();
            List<string> res = books.CheckGapoifika("шк");
            Assert.AreEqual(2, res.Count);
            Assert.IsTrue(res.Contains("Шкура"));
            Assert.IsTrue(res.Contains("Школа"));
        }
        [TestMethod]
        public void Books_Ledida_2()
        {
            BooksDictionary books = new BooksDictionary();
            List<string> res = books.CheckLedida("ийка");
            Assert.AreEqual(4, res.Count);
            Assert.IsTrue(res.Contains("Гарри Поттер и философский камень"));
            Assert.IsTrue(res.Contains("Пятнадцатилетний капитан"));
            Assert.IsTrue(res.Contains("Философский камень"));
            Assert.IsTrue(res.Contains("Флорентийка"));
        }
        // CheckGapoifika CheckLedida
    }
}
