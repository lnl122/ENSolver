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
            SpellCheck.Init();
            SpellCheck.LoadDictionary();
            SpellCheck SC = new SpellCheck();
            List<string> wrds = new List<string>() { "мама", "мыла", "бляхамухаминуспять", "раму", "веселоебали" };
            List<string> res = new List<string>() { "мама", "мыла", "раму" };
            List<string> res2 = SC.CheckBlock(wrds);
            SC.Close();
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
