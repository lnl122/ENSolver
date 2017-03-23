using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ENSolver;

namespace UnitTestENSolver
{
    //[TestClass]
    public class UploadTests
    {
        [TestMethod]
        public void Upload()
        {
            Upload up = new Upload();
            string path = Environment.CurrentDirectory + @"\..\files\tulips.jpg";
            string res = up.GetUrl(path);
            Assert.AreEqual("http://", res.Substring(0, 7));
            Assert.AreEqual(".jpg", res.Substring(res.Length - 4));
        }
        [TestMethod]
        public void Upload_IpicSu()
        {
            HosterIpicSu up = new HosterIpicSu();
            string path = Environment.CurrentDirectory + @"\..\files\tulips.jpg";
            string res = up.GetUrl(path);
            Assert.AreEqual("http://ipic.su/img", res.Substring(0, 18));
            Assert.AreEqual(".jpg", res.Substring(res.Length - 4));
        }
        [TestMethod]
        public void Upload_Ii4Ru()
        {
            HosterIi4Ru up = new HosterIi4Ru();
            string path = Environment.CurrentDirectory + @"\..\files\tulips.jpg";
            string res = up.GetUrl(path);
            Assert.AreEqual("http://img.ii4.ru/images", res.Substring(0, 24));
            Assert.AreEqual(".jpg", res.Substring(res.Length - 4));
        }
        //[TestMethod]
        public void Upload_JpegshareNet()
        {
            HosterJpegshareNet up = new HosterJpegshareNet();
            string path = Environment.CurrentDirectory + @"\..\files\tulips.jpg";
            string res = up.GetUrl(path);
            Assert.AreEqual("http://jpegshare.net/images", res.Substring(0, 27));
            Assert.AreEqual(".jpg", res.Substring(res.Length - 4));
        }
        [TestMethod]
        public void Upload_PixshockNet()
        {
            HosterPixshockNet up = new HosterPixshockNet();
            string path = Environment.CurrentDirectory + @"\..\files\tulips.jpg";
            string res = up.GetUrl(path);
            Assert.AreEqual("http://www.pixshock.net/pic_b", res.Substring(0, 29));
            Assert.AreEqual(".jpg", res.Substring(res.Length - 4));
        }
        [TestMethod]
        public void Upload_SaveimgRu()
        {
            HosterSaveimgRu up = new HosterSaveimgRu();
            string path = Environment.CurrentDirectory + @"\..\files\tulips.jpg";
            string res = up.GetUrl(path);
            Assert.AreEqual("http://saveimg.ru/pictures", res.Substring(0, 26));
            Assert.AreEqual(".jpg", res.Substring(res.Length - 4));
        }
        [TestMethod]
        public void Upload_SavepicRu()
        {
            HosterSavepicRu up = new HosterSavepicRu();
            string path = Environment.CurrentDirectory + @"\..\files\tulips.jpg";
            string res = up.GetUrl(path);
            Assert.AreEqual("http://savepic.ru/", res.Substring(0, 18));
            Assert.AreEqual(".jpg", res.Substring(res.Length - 4));
        }
        [TestMethod]
        public void Upload_RadikalRu()
        {
            HosterRadikalRu up = new HosterRadikalRu();
            string path = Environment.CurrentDirectory + @"\..\files\tulips.jpg";
            string res = up.GetUrl(path);
            Assert.AreEqual("http://", res.Substring(0, 7));
            Assert.AreNotEqual(-1, res.IndexOf(".radikal.ru"));
            Assert.AreEqual(".jpg", res.Substring(res.Length - 4));
        }
        [TestMethod]
        public void Upload_FreeimagehostingNet()
        {
            HosterFreeimagehostingNet up = new HosterFreeimagehostingNet();
            string path = Environment.CurrentDirectory + @"\..\files\tulips.jpg";
            string res = up.GetUrl(path);
            Assert.AreEqual("http://", res.Substring(0, 7));
            Assert.AreEqual(".jpg", res.Substring(res.Length - 4));
        }
        //[TestMethod]
        public void Upload_PixicRu()
        {
            HosterPixicRu up = new HosterPixicRu();
            string path = Environment.CurrentDirectory + @"\..\files\tulips.jpg";
            string res = up.GetUrl(path);
            Assert.AreEqual(path, res);
        }

    }
}
