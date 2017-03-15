using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestENSolver
{
    [TestClass]
    public class LogTests
    {
        [TestMethod]
        public void Log_Exists()
        {
            ENSolver.Log.Init();
            string fn = Environment.CurrentDirectory + "\\Log\\" + System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName + ".log";
            Assert.IsTrue(System.IO.File.Exists(fn));
            ENSolver.Log.Close();
        }
        [TestMethod]
        public void Log_Write()
        {
            ENSolver.Log.Init();
            string fn = Environment.CurrentDirectory + "\\Log\\" + System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName + ".log";
            long size1 = new System.IO.FileInfo(fn).Length;
            ENSolver.Log.Write("1st string", "2nd string", "3th string");
            ENSolver.Log.Close();
            long size2 = new System.IO.FileInfo(fn).Length;
            Assert.AreNotEqual(size1, size2);
        }
    }
}
