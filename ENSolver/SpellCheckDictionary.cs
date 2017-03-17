// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System.Collections.Generic;

namespace ENSolver
{
    interface ISpellCheckDictionary
    {
        List<string> GetDict1();
        List<string> GetDict2();
        void Add(string wrd);
        void Save();
    }

    public class SpellCheckDictionary : ISpellCheckDictionary
    {
        // количество новых слов, через которое будет производиться запись словаря на диск
        private const int NewWords = 10;

        // лог
        private static Log Log = new Log();
        // объекты для блокировки
        private static object ObjLockInit = new object();
        private static object ObjLockSave = new object();
        // словари - загружаемый и создающийся в процессе работы
        private static List<string> dict1 = new List<string>();
        private static List<string> dict2 = new List<string>();
        // путь к словарю
        private static string DictionaryPath = "";
        // первое создание объекта уже было?
        private static bool isObjectReady = false;

        /// <summary>
        /// конструктор
        /// </summary>
        public SpellCheckDictionary()
        {
            if (!isObjectReady)
            {
                lock (ObjLockInit)
                {
                    if (!isObjectReady)
                    {
                        Log.Write("SpellCheckDictionary: Чтение внешнего словаря начато");
                        string DictionaryPath = GetDictPath();
                        if (System.IO.File.Exists(DictionaryPath))
                        {
                            string[] dict = System.IO.File.ReadAllLines(DictionaryPath);
                            foreach (string s1 in dict)
                            {
                                dict1.Add(s1.ToLower());
                            }
                            isObjectReady = true;
                            Log.Write("SpellCheckDictionary: Чтение внешнего словаря завершено");
                        }
                        else
                        {
                            Log.Write("SpellCheckDictionary: ERROR: словаря по указанному пути нет", DictionaryPath);
                        }
                    }
                }
            }
        }

        // временная заглушка. заменить на функцию получения пути и имени
        public string GetDictPath()
        {
            return System.Environment.CurrentDirectory + "\\Data\\SpChDict.dat";
        }

        /// <summary>
        /// добавление нового слова в польовательский словарь
        /// </summary>
        /// <param name="wrd">добавляемое слово</param>
        public void Add(string wrd)
        {
            dict2.Add(wrd.ToLower());
            if(dict2.Count % NewWords == 0)
            {
                Save();
            }
        }

        /// <summary>
        /// возвращает первый словарь
        /// </summary>
        /// <returns>список слов первого словаря</returns>
        public List<string> GetDict1()
        {
            //if (!isObjectReady) { return new List<string>(); }
            return dict1;
        }

        /// <summary>
        /// возвращает второй словарь
        /// </summary>
        /// <returns>список слов второго словаря</returns>
        public List<string> GetDict2()
        {
            //if (!isObjectReady) { return new List<string>(); }
            return dict2;
        }

        /// <summary>
        /// сохраняет польовательский словарь, сформированный в процессе работы на диск
        /// </summary>
        public void Save()
        {
            lock (ObjLockSave)
            {
                Log.Write("SpellCheckDictionary: Запись в файл словаря для Spellchecker'а начата");
                if (!isObjectReady) { return; }
                // объединяем два словаря (без пустых строк) и сохраняем в файл DictionaryPath
                List<string> dict_out = new List<string>();
                dict_out.AddRange(dict1);
                dict_out.AddRange(dict2);
                if (DictionaryPath != "")
                {
                    System.IO.File.WriteAllLines(DictionaryPath, dict_out.ToArray());
                }
                Log.Write("SpellCheckDictionary: Запись в файл словаря для Spellchecker'а завершена");
            }
        }

    }
}
