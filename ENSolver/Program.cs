// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;

namespace ENSolver
{
    public class Program
    {
        private static Log Log = new Log("Main program");

        public static void Main(string[] args)
        {
            Log.Write("Выполняем проверки окружения");
            if (!SysInfo.Check())
            {
                Log.Write("ERROR: Не все необхдимые компоненты установлены.");
                System.Windows.Forms.MessageBox.Show("Не все необхдимые компоненты установлены на ПК.\r\nПроверьте лог-файл.");
                return;
            }
#if !DEBUG
            Log.Write("Выполняем вход в игровой движок");
            if (!(new Logon()).GetResult())
            {
                Log.Write("ERROR: Не удалось войти/отказ от логона/неверные логин-пароль");
                System.Windows.Forms.MessageBox.Show("Собственно ваше дело, играть или не играть...\r\nНо логин/пароль помнить надо. Плохая память - не наш конек.");
                return;
            }
#endif
#if DEBUG
            Engine engine = new Engine();
            if (!engine.Logon(new UserInfo("полвторого", "ovs122")))
            {
                System.Windows.Forms.MessageBox.Show("полвторого неудачный логон");
            }
#endif
            System.Collections.Generic.List<GameInfo> res = engine.GetGameList();
            // получение списка игр
            // выбор в офрме конкретной игры
            // чтение игры
            // отрисофка основной формы

            // после закрытия основной формы - выходим
            Log.Write("Сохраняем сформированный список корректных слов в локальный словарь");
            (new SpellCheckDictionary()).Save();
            Log.Write("Завершаем работу с лог-файлом");
            Log.Flush();
            Log.Close();
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
