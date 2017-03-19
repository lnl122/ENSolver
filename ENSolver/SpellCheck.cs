// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ENSolver
{
    // need COM Reference "Microsoft.Word.14.Object.Library"

    /// <summary>
    /// реализует проверку слов на орфографию
    /// </summary>
    interface ISpellCheck
    {
        bool Check(string wrd);
        List<string> Check(List<string> wrds);
        void Close();
        void Dispose();
    }

    public class SpellCheck
    {
        // лог
        private ILog Log = new Log("SpellCheck");
        // словарь
        private ISpellCheckDictionary Dictionary = new SpellCheckDictionary();
        private const string teststring = "мама мыла раму";
        // первое создание объекта уже было?
        private bool isReady = false;
        private static bool isWordPresent = false;
        // ограничение для создания нескольких потоков
        private const int maxCntWords = 1000;

        // внешний объект для экземпляра CpellCheck'а
        private Microsoft.Office.Interop.Word.Application WordApp = null;

        /// <summary>
        /// конструктор
        /// </summary>
        public SpellCheck()
        {
            if (!isWordPresent) { WordInit(); }
            Log.Write("Инициализация объекта SpellChecker");
            if (isWordPresent)
            {
                try
                {
                    WordApp = new Microsoft.Office.Interop.Word.Application();
                    Log.Write("Создание нового объекта спелчекера");
                    isReady = true;
                }
                catch
                {
                    Log.Write("ERROR: Не смогли создать объект Microsoft.Office.Interop.Word.Application()");
                }
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
            Log.Write("Закрыли очередную копию MS Word");
        }
        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// проверяет одно слово в словаре, и, если там нету - то в Word
        /// </summary>
        /// <param name="SingleWord">проверяемое слово</param>
        /// <returns>результат проверки</returns>
        public bool Check(string SingleWord)
        {
            if (!isReady) { return false; }
            // нормализуем входящее слово
            string NormalWord = SingleWord.ToLower().Trim();
            // отсекаем пустые слова
            if (NormalWord == "") { return false; }

            // проверяем в основном и пользовательском словаре
            if (Dictionary.Check(NormalWord)) { return true; }
            // проверяем в MsWord само слово
            if (WordApp.CheckSpelling(NormalWord)) { Dictionary.Add(NormalWord); return true; }
            // проверяем в MsWord капитализированное слово
            string CapitalWord = CapitalizeWord(NormalWord);
            if (WordApp.CheckSpelling(CapitalWord)) { Dictionary.Add(CapitalWord); return true; }

            // если не нашли в ворде
            return false;
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
            if (!isReady) { return res; }

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
        /// инициализация объектов
        /// </summary>
        private void WordInit()
        {
            if (!isReady)
            {
                if (CheckMsWord())
                {
                    isWordPresent = true;
                }
            }
        }

        /// <summary>
        /// проверка существования Word и работы чекера
        /// </summary>
        /// <returns>флаг готовности</returns>
        private bool CheckMsWord()
        {
            Log.Write("Проверяем наличие MS Word");
            try
            {
                var wa = new Microsoft.Office.Interop.Word.Application();
                bool test = wa.CheckSpelling(teststring);
                if (test) { Log.Write("MS Word точно есть, тест проверкой пройден"); }
                else { Log.Write("ERROR: тест проверкой не пройден"); }
                wa.Quit();
                return true;
            }
            catch
            {
                Log.Write("ERROR: не удалось запустить MS Word, проверить слова, или какие-то другие проблемы");
                return false;
            }
        }

        /// <summary>
        /// капитализация слова
        /// </summary>
        /// <param name="NormalWord">входящее слово</param>
        /// <returns>слово, где первый символ - заглавная литера</returns>
        private static string CapitalizeWord(string NormalWord)
        {
            return NormalWord.Substring(0, 1).ToUpper() + NormalWord.Substring(1, NormalWord.Length - 1).ToLower();
        }

        /// <summary>
        /// проверяет список слов (до 1000) за один проход цикла
        /// </summary>
        /// <param name="InnerWordList">список слов</param>
        /// <returns>список слов, прошедших орфографию успешно</returns>
        private List<string> CheckBlock(List<string> InnerWordList)
        {
            //результат
            List<string> res = new List<string>();
            if (!isReady) { return res; }

            // для всех слов, если их меньше тысячи
            foreach (string SingleWord in InnerWordList)
            {
                string wrd = SingleWord.ToLower().Trim();
                if (Check(wrd)) { res.Add(wrd); }
            }
            return res;
        }

    }
}
