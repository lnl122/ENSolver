using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENSolver
{
    public class Program
    {

        static void Main(string[] args)
        {
            HosterIpicSu up = new HosterIpicSu();
            string path = Environment.CurrentDirectory + @"\..\files\tulips.jpg";
            string res = up.GetUrl(path);
        }

        static public int test2()
        {
            return 2;
        }
    }

    // класс для тестирования методов основной программы
    public class Class1
    {
        static public int test1()
        {
            return 1;
        }
    }
}
