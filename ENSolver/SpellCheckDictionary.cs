// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System.Collections.Generic;

namespace ENSolver
{
    /// <summary>
    /// реализует работу с собственным орфографическим словарем 
    /// </summary>
    interface ISpellCheckDictionary
    {
        bool Check(string wrd);
        void Add(string wrd);
        void Save();
    }

    public class SpellCheckDictionary : ISpellCheckDictionary
    {
        // количество новых слов, через которое будет производиться запись словаря на диск
        private const int NewWords = 10;

        // лог
        private static Log Log = new Log("SpellCheckDict");
        // объекты для блокировки
        private static object LockInit = new object();
        private static object LockSave = new object();
        // словарь
        private static List<string> dict;
        // путь к словарю
        private static string DictionaryPath = "";
        // первое создание объекта уже было?
        private static bool isReady = false;

        /// <summary>
        /// конструктор
        /// </summary>
        public SpellCheckDictionary()
        {
            Log.Write("Новый словарь орфографии");
            if (!isReady)
            {
                lock (LockInit)
                {
                    if (!isReady)
                    {
                        Log.Write("Чтение внешнего словаря начато");
                        dict = new List<string>();
                        string DictionaryPath = (new FilePath()).GetDictionary();
                        if (System.IO.File.Exists(DictionaryPath))
                        {
                            string[] filedict = System.IO.File.ReadAllLines(DictionaryPath);
                            foreach (string s1 in filedict)
                            {
                                dict.Add(s1.ToLower());
                            }
                            isReady = true;
                            Log.Write("Чтение внешнего словаря завершено");
                        }
                        else
                        {
                            Log.Write("ERROR: словаря по указанному пути нет");
                            Log.Write(DictionaryPath);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// проверка вхождения слова в словарь
        /// </summary>
        /// <param name="wrd">проверяемое слово</param>
        /// <returns>результат</returns>
        public bool Check(string wrd)
        {
            return dict.Contains(wrd.ToLower());
        }

        /// <summary>
        /// добавление нового слова в польовательский словарь
        /// </summary>
        /// <param name="wrd">добавляемое слово</param>
        public void Add(string wrd)
        {
            dict.Add(wrd.ToLower());
            if(dict.Count % NewWords == 0)
            {
                Save();
            }
        }

        /// <summary>
        /// сохраняет польовательский словарь, сформированный в процессе работы на диск
        /// </summary>
        public void Save()
        {
            lock (LockSave)
            {
                Log.Write("Запись в файл словаря для Spellchecker'а начата");
                if (!isReady) { return; }
                if (DictionaryPath != "")
                {
                    System.IO.File.WriteAllLines(DictionaryPath, dict.ToArray());
                }
                Log.Write("Запись в файл словаря для Spellchecker'а завершена");
            }
        }

    }
}
