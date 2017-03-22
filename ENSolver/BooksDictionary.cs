// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System.Collections.Generic;

namespace ENSolver
{
    /// <summary>
    /// реализует работу с собственным словарем названий книг
    /// </summary>
    interface IBooksDictionary
    {
        List<string> CheckGapoifika(string wrd);
        List<string> CheckLedida(string wrd);
    }

    public class BooksDictionary : IBooksDictionary
    {
        // лог
        private static ILog Log = new Log("BooksDict");
        // объекты для блокировки
        private static object LockInit = new object();
        // словарь
        private static List<string> dict;
        private static List<string> plain;
        private static List<string> gapo;
        private static int Count;
        // первое создание объекта уже было?
        private static bool isReady = false;

        /// <summary>
        /// конструктор
        /// </summary>
        public BooksDictionary()
        {
            Log.Write("Новый словарь книг");
            if (!isReady)
            {
                lock (LockInit)
                {
                    if (!isReady)
                    {
                        Log.Write("Чтение внешнего словаря начато");
                        dict = new List<string>();
                        gapo = new List<string>();
                        plain = new List<string>();
                        string DictionaryPath = (new FilePath()).GetBooks();
                        if (System.IO.File.Exists(DictionaryPath))
                        {
                            string[] filedict = System.IO.File.ReadAllLines(DictionaryPath);
                            foreach (string s1 in filedict)
                            {
                                string s = ClearBookName(s1);

                                if (s != "")
                                {
                                    if (dict.Contains(s)) { continue; } // дубликаты названий книг не включаются в словарь
                                    dict.Add(s);
                                    plain.Add(s.Replace(" ", "").ToLower());
                                    string[] ar1 = s.Split(' ');
                                    string res = "";
                                    foreach (string w in ar1)
                                    {
                                        if (w.Length == 1)
                                        {
                                            res = res + w.ToUpper();
                                        }
                                        else
                                        {
                                            res = res + w.Substring(0, 1).ToUpper();
                                            res = res + w.Substring(1, 1).ToLower();
                                        }
                                    }
                                    gapo.Add(res);
                                }
                            }
                            Count = dict.Count;

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
        /// поиск названий под ГаПоИФиКу
        /// </summary>
        /// <param name="wrd">проверяемое слово</param>
        /// <returns>результат</returns>
        public List<string> CheckGapoifika(string wrd)
        {
            string w = wrd.ToLower();
            List<string> res = new List<string>();
            for (int i = 0; i < Count; i++)
            {
                if (gapo[i].ToLower() == w)
                {
                    res.Add(dict[i]);
                }
            }
            return res;
        }

        /// <summary>
        /// поиск названий под Ледида
        /// </summary>
        /// <param name="wrd">проверяемое слово</param>
        /// <returns>результат</returns>
        public List<string> CheckLedida(string wrd)
        {
            string w = wrd.ToLower();
            List<string> res = new List<string>();
            for(int i = 0; i < Count; i++)
            {
                if (plain[i].Contains(w))
                {
                    res.Add(dict[i]);
                }
            }
            return res;
        }

        /// <summary>
        /// очищает название книги от паразитных символов, не являющиеся символами для слов
        /// </summary>
        /// <param name="s1">входящее название</param>
        /// <returns>очищенное название</returns>
        private static string ClearBookName(string s1)
        {
            string s = s1;
            s = s.Replace("ё", "е").Replace("Ё", "Е");
            s = s.Replace(".", " ").Replace(",", " ").Replace("-", " ").Replace("\"", " ").Replace("!", " ").Replace("?", " ").Replace("#", " ");
            s = s.Replace(":", " ").Replace(";", " ").Replace("%", " ").Replace("(" , " ").Replace(")", " ").Replace("+", " ").Replace("/", " ").Replace("\\", " ");
            s = s.Replace("«", " ").Replace("»", " ");
            s = s.Trim().Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
            return s;
        }

    }
}

