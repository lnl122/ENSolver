using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ENSolver;

namespace UnitTestENSolver
{
    [TestClass]
    public class GoogleTests
    {
        private static string[,] tags_script = { { "<script>"  , "<noscript>"  }, { "</script>", "</noscript>" } };
        private static string[] bad_tag = { "<script>"  , "<noscript>" };

        [TestMethod]
        public void ParsePage_ParseTags_2()
        {
            string inner = "01<script>dfghdfg</script>2<noscript>giugi</noscript>34567<script>dfghdfg</script>89";
            string outer = ParsePage.ParseTags(inner, tags_script);
            Assert.AreEqual("0123456789", outer);
        }
        [TestMethod]
        public void ParsePage_RemoveBad_2()
        {
            string inner = "0123<script>4<noscript>56789";
            string outer = ParsePage.RemoveBadWords(inner, bad_tag);
            Assert.AreEqual("0123 4 56789", outer);
        }
        [TestMethod]
        public void ParsePage_RemoveMnemonics_3()
        {
            string inner = "0123&nbsp;45&nbsp;6789012&lt;34567&nbsp;89&gt;";
            string outer = ParsePage.RemoveMnemonics(inner).Replace(" ","");
            Assert.AreEqual("01234567890123456789", outer);
        }
        [TestMethod]
        public void Google_TextByImage_1()
        {
            string file = Environment.CurrentDirectory + @"\..\files\buratino.jpg";
            Google goo = new Google();
            string res = goo.GetWordsByImgFile(file);
            int idx1 = res.IndexOf("пьеро");
            int idx2 = res.IndexOf("буратино");
            Assert.AreNotEqual(-1, idx1);
            Assert.AreNotEqual(-1, idx2);
        }
        [TestMethod]
        public void Google_TextByImage_2()
        {
            string file = Environment.CurrentDirectory + @"\..\files\vanga.jpg";
            Google goo = new Google();
            string res = goo.GetWordsByImgFile(file);
            int idx1 = res.IndexOf("ванга");
            Assert.AreNotEqual(-1, idx1);
        }
        [TestMethod]
        public void Google_TextByImage_3()
        {
            string file = Environment.CurrentDirectory + @"\..\files\tango.jpg";
            Google goo = new Google();
            string res = goo.GetWordsByImgFile(file);
            int idx1 = res.IndexOf("танго");
            Assert.AreNotEqual(-1, idx1);
        }
        // goo.GetWordsByImgFile(@"..\files\buratino.jpg");
    }
}
