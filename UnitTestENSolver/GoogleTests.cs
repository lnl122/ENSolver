using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ENSolver;

namespace UnitTestENSolver
{
    [TestClass]
    public class GoogleTests
    {
        private static string[,] tags_script = {
                { "<script>"  , "<noscript>"  },
                { "</script>", "</noscript>" }
            };

        [TestMethod]
        public void ParseTags_2()
        {
            string inner = "01<script>dfghdfg</script>2<noscript>giugi</noscript>34567<script>dfghdfg</script>89";
            string outer = ParsePage.ParseTags(inner, tags_script);
            Assert.AreEqual("0123456789", outer);
        }

    }
}
