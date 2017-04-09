// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;

namespace ENSolver
{
    public class Program
    {

        static void Main(string[] args)
        {
            // выполняем проверки окружения
            if (!SysInfo.Check())
            {
                System.Windows.Forms.MessageBox.Show("Не все необхдимые компоненты установлены на ПК.\r\nПроверьте лог-файл.");
                return;
            }

            /*
            // инициализируем наши собственные компоненты
            Components.Init();

            // создаем основную форму
            spl.ChangeProgress(70, "Создаём форму приложения");
            AppForm AF = new AppForm();

            // создаём пользовательский таб
            spl.ChangeProgress(70, "Создаём пользовательский уровень");
            Level userlevel = new Level(D.Game, 0);
            D.Lvl.Add(userlevel);
            OneTab OT = new OneTab(D, userlevel);
            D.OneTab.Add(OT);

            // закрываем сплеш
            spl.ChangeProgress(100, "Готово!");
            System.Windows.Forms.Application.Run(D.F);

            // закругляемся
            Components.Close();
            Log.Write("Выход из программы..");
            Log.Close();
            */
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
