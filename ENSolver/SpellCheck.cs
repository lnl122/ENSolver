// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ENSolver
{
    // need COM Reference "Microsoft.Word.14.Object.Library"
    //
    // public Init()
    // public SpellChecker() - constructor
    // public void Close()
    // public void LoadDictionary(string DictPath)
    // public void SaveDictionary()
    // public List<string> Check(List<string> InnerWordList)
    // public bool Check(string SingleWord)
    // public bool CheckOne(string SingleWord)
    //

    public class SpellCheck
    {
        // словари - загружаемый и создающийся в процессе работы
        public static List<string> dict1;
        public static List<string> dict2;
        // путь к словарю
        private static string DictionaryPath = "";
        // первое создание объекта уже было?
        public static bool isObjectReady = false;
        // ограничение для создания нескольких потоков
        public static int maxCntWords = 1000;
        // словарь был ли загружен?
        public static bool isDicionaryLoaded = false;
        // тестовая строка для проверки работоспособности
        private static string teststring = "мама мыла раму";

        // внешний объект для экземпляра CpellCheck'а
        private Microsoft.Office.Interop.Word.Application WordApp = null;

        /// <summary>
        /// конструктор
        /// </summary>
        public SpellCheck()
        {
            //инициализация одного объекта, если ранее не инициализировали
            if (isObjectReady == true)
            {
                WordApp = new Microsoft.Office.Interop.Word.Application();
                Log.Write("SpellCheck: Создание нового объекта спелчекера");
            }
        }

        /// <summary>
        /// деструктор
        /// </summary>
        public void Close()
        {
            //SaveDictionary();
            WordApp.Quit();
            WordApp = null;
            Log.Write("SpellCheck: Закрыли очередную копию MS Word");
        }



        /// <summary>
        /// инициализация объектов
        /// </summary>
        public static void Init()
        {
            Log.Write("SpellCheck: Инициализация объекта SpellChecker");
            if (isObjectReady == false)
            {
                if (CheckMsWord() == true)
                {
                    dict1 = new List<string>();
                    dict2 = new List<string>();
                    isObjectReady = true;
                }
            }
        }
        
        /// <summary>
        /// чтение словаря с диска
        /// </summary>
        /// <param name="DictPath">имя файла словаря</param>
        public static void LoadDictionary(string DictPath2 = "")
        {
            // 
            string DictName = "SpChDict.dat";
            if (DictPath2 != "") { DictName = DictPath2; }
            string DictPath = System.Environment.CurrentDirectory + "\\Data\\" + DictName;

            Log.Write("SpellCheck: Чтение внешнего словаря начато");
            if (isObjectReady == false) { return; }
            // если словарь не загружен
            if (isDicionaryLoaded == false)
            {
                // проверить путь на валидность
                if (System.IO.File.Exists(DictPath) == true)
                {
                    string[] dict; // временный массив
                    dict = System.IO.File.ReadAllLines(DictPath);
                    DictionaryPath = DictPath;
                    // переносим в List
                    foreach (string s1 in dict)
                    {
                        dict1.Add(s1.ToLower());
                    }
                    isDicionaryLoaded = true;
                    Log.Write("SpellCheck: Чтение внешнего словаря завершено");
                }
                else
                {
                    Log.Write("SpellCheck: ERROR: словаря по указанному пути нет", DictPath);
                }
            }
        }

        /// <summary>
        /// сохраняет польовательский словарь, сформированный в процессе работы на диск
        /// </summary>
        public static void SaveDictionary()
        {
            Log.Write("SpellCheck: Запись в файл словаря для Spellchecker'а начата");
            if (isObjectReady == false) { return; }
            // объединяем два словаря (без пустых строк) и сохраняем в файл DictionaryPath
            List<string> dict_out = new List<string>();
            dict_out.AddRange(dict1);
            foreach (string s1 in dict2)
            {
                dict_out.Add(s1.ToLower());
            }
            if (DictionaryPath != "")
            {
                System.IO.File.WriteAllLines(DictionaryPath, dict_out.ToArray());
            }
            Log.Write("SpellCheck: Запись в файл словаря для Spellchecker'а завершена");
        }

        /// <summary>
        /// проверка существования Word и работы чекера
        /// </summary>
        /// <returns>флаг готовности</returns>
        public static bool CheckMsWord()
        {
            Log.Write("SpellCheck: Проверяем наличие MS Word");
            try
            {
                var wa = new Microsoft.Office.Interop.Word.Application();
                wa.CheckSpelling(teststring);
                wa.Quit();
                Log.Write("SpellCheck: MS Word точно есть, тест проверкой пройден");
                return true;
            }
            catch
            {
                Log.Write("SpellCheck: ERROR: MS Word не удалось запустить, проверить слова, или какие-то другие проблемы");
                return false;
            }
        }

        /// <summary>
        /// капитализация слова
        /// </summary>
        /// <param name="NormalWord">входящее слово</param>
        /// <returns>слово, где первый символ - заглавная литера</returns>
        public static string CapitalizeWord(string NormalWord)
        {
            return NormalWord.Substring(0, 1).ToUpper() + NormalWord.Substring(1, NormalWord.Length - 1).ToLower();
        }


        /// <summary>
        /// проверяет произвольное число слов в списке на орфографию.
        /// при количестве слов более maxCntWords - проверка производиться в нескольких потоках
        /// </summary>
        /// <param name="InnerWordList">список слов для проверки</param>
        /// <returns>список корректных слов</returns>
        public List<string> Check(List<string> InnerWordList)
        {
            //результат
            List<string> res = new List<string>();
            if (isObjectReady == false) { return res; }

            if (InnerWordList.Count <= maxCntWords)
            {
                // если слов меньше 1000
                res = CheckBlock(InnerWordList);
            }
            else
            {
                // для слов, если их более тысячи

                // разбиваем список на кусочки по 1000 слов
                List<List<string>> wrd1000 = new List<List<string>>();
                List<string> wl = InnerWordList;
                while (wl.Count > maxCntWords)
                {
                    List<string> qq = new List<string>();
                    for (int i = 0; i < maxCntWords; i++) { qq.Add(wl[i]); }
                    wl.RemoveRange(0, maxCntWords);
                    wrd1000.Add(qq);
                }
                if (wl.Count != 0)
                {
                    wrd1000.Add(wl);
                }
                // формируем таски, передаём им управление
                var Tasks2 = new List<Task<List<string>>>();
                foreach (List<string> t2 in wrd1000)
                {
                    Tasks2.Add(Task<List<string>>.Factory.StartNew(() => CheckBlock(t2)));
                }
                // дождаться выполнения потоков, собрать результаты вместе
                Task.WaitAll(Tasks2.ToArray());
                List<string> r2 = new List<string>();
                foreach (Task<List<string>> t8 in Tasks2)
                {
                    res.AddRange(t8.Result);
                }
            }
            return res;
        }

        /// <summary>
        /// проверяет список слов (до 1000) за один проход цикла
        /// </summary>
        /// <param name="InnerWordList">список слов</param>
        /// <returns>список слов, прошедших орфографию успешно</returns>
        public List<string> CheckBlock(List<string> InnerWordList)
        {
            //результат
            List<string> res = new List<string>();
            if (isObjectReady == false) { return res; }

            // для всех слов, если их меньше тысячи
            foreach (string SingleWord in InnerWordList)
            {
                string wrd = SingleWord.ToLower().Trim();
                if (Check(wrd)) { res.Add(wrd); }
            }
            return res;
        }

        /// <summary>
        /// проверяет одно слово в словаре, и, если там нету - то в Word
        /// </summary>
        /// <param name="SingleWord">проверяемое слово</param>
        /// <returns>результат проверки</returns>
        public bool Check(string SingleWord)
        {
            if (isObjectReady == false) { return false; }
            // нормализуем входящее слово
            string NormalWord = SingleWord.ToLower().Trim();
            // отсекаем пустые слова
            if (NormalWord == "") { return false; }

            if (isDicionaryLoaded)
            {
                // проверяем в основном и пользовательском словаре
                if (dict1.Contains(NormalWord)) { return true; }
                if (dict2.Contains(NormalWord)) { return true; }
            }
            // проверяем в MsWord само слово
            if (WordApp.CheckSpelling(NormalWord)) { dict2.Add(NormalWord); return true; }
            // проверяем в MsWord капитализированное слово
            string CapitalWord = CapitalizeWord(NormalWord);
            if (WordApp.CheckSpelling(CapitalWord)) { dict2.Add(CapitalWord); return true; }

            // если не нашли в ворде
            return false;
        }

        /// <summary>
        /// проверка слова только в Word. слово не нормализуется, не проверяется по словарю
        /// </summary>
        /// <param name="SingleWord">слово для проверки</param>
        /// <returns>ответ</returns>
        public bool CheckOne(string SingleWord)
        {
            if (isObjectReady == false)
            {
                return false;
            }
            // отсекаем пустые слова
            if (SingleWord == "")
            {
                return false;
            }
            return WordApp.CheckSpelling(SingleWord);
        }

    }
}
