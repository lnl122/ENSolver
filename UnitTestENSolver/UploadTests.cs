using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ENSolver;

namespace UnitTestENSolver
{
    [TestClass]
    public class UploadTests
    {
        [TestMethod]
        public void Upload_IpicSu()
        {
            HosterIpicSu up = new HosterIpicSu();
            string path = Environment.CurrentDirectory + @"\..\files\tulips.jpg";
            string res = up.GetUrl(path);
            Assert.AreEqual("http://ipic.su/img", res.Substring(0, 18));
            Assert.AreEqual(".jpg", res.Substring(res.Length - 4));
        }
        /*
        [TestMethod]
        public void Upload_PixicRu()
        {
            HosterPixicRu up = new HosterPixicRu();
            string path = Environment.CurrentDirectory + @"\..\files\tulips.jpg";
            string res = up.GetUrl(path);
            Assert.AreEqual(path, res);
        }
        */

    }
}
