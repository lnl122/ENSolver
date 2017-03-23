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
            Log log = new ENSolver.Log("Main");
            DateTime dt = DateTime.Now;
            string folder = @"C:\Users\Антон\Source\Repos\Solver2\Solver2\Solver2\bin\Debug\Pics";
            string[] dirs = System.IO.Directory.GetFiles(folder, "*.jpg");
            Google goo = new Google();
            foreach(string q in dirs)
            {
                string re = goo.GetWordsByImgFile(q);
            }
            log.Write("Main time for 163 pics: " + (Math.Floor((DateTime.Now - dt).TotalMilliseconds) / 1000).ToString());


            //Google goo = new Google();
            //string res1 = goo.GetPageByImageUrl("http://www.obnovi.com/uploads/posts/2011-12/thumbs/1322828778_1.jpg"); // чебурашка
            //string res2 = goo.GetPageByImageUrl("http://png2.ru/media/k2/items/cache/23da450944f0818162562a06dc761501_L.jpg"); // лосяш
            //string res3 = goo.GetPageByImageUrl("http://foodandhealth.ru/wp-content/uploads/2016/10/kofe-e1475678835457-300x300.jpg"); // кофе
            //string res4 = goo.GetPageByImageUrl("https://news.tj/sites/default/files/articles/231739/914204081.jpg"); // караул у мавзолея
            //string res5 = goo.GetPageByImageUrl("http://rabotastudentu.ru/wp-content/uploads/2013/04/a50878f50680a3e082bfb3238f084bb1-220x300.jpg"); // диплом
            //string res01 = goo.GetWordsByImgFile(@"..\files\buratino.jpg");
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
