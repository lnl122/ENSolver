using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ENSolver;

namespace UnitTestENSolver
{
    [TestClass]
    public class LogTests
    {
        
        [TestMethod]
        public void Log_Exists()
        {
            Log l = new Log("UnitTest");
            string fn = Environment.CurrentDirectory + "\\Log\\" + System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName + ".log";
            Assert.IsTrue(System.IO.File.Exists(fn));
            l.Close();
        }
        [TestMethod]
        public void Log_Write()
        {
            Log l = new Log("UnitTest");
            string fn = Environment.CurrentDirectory + "\\Log\\" + System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName + ".log";
            long size1 = new System.IO.FileInfo(fn).Length;
            l.Write("тестовая строка");

            long size2 = new System.IO.FileInfo(fn).Length;
            Assert.AreNotEqual(size1, size2);
            l.Close();
        }
    }
}
