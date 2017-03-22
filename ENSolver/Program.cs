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
            Google goo = new Google();
            string res = goo.GetPageByImageUrl("http://www.obnovi.com/uploads/posts/2011-12/thumbs/1322828778_1.jpg");
            int i = 0;
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
