// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;

namespace ENSolver
{
    /// <summary>
    /// предоставляем по запросу пути к внешним файловым ресурсам
    /// </summary>
    interface IFilePath
    {
        string GetLog();
        string GetPages();
        string GetPics();
        string GetDictionary();
        string GetAssoc();
        string GetAssocBad();
        string GetBooks();
    }

    class FilePath : IFilePath
    {
        // константы
        private const string LogFolderName = "Log";
        private const string DataFolderName = "Data";
        private const string PagesFolderName = "Pages";
        private const string PicsFolderName = "Pics";
        private const string SpellChekDictFile = "SpChDict.dat";
        private const string AssocDictFile = "AssocDict.dat";
        private const string AssocBadDictFile = "AssocDictBad.dat";
        private const string BooksDictFile = "Books.dat";
        // готовность
        private static bool isReady = false;
        // объекты для блокировки
        private static object LockInit = new object();
        // готовые пути и имена
        private static string Pages;
        private static string Pics;
        private static string Log;
        private static string SpellChekDict;
        private static string AssocDict;
        private static string AssocBadDict;
        private static string BooksDict;

        /// <summary>
        /// контсруктор
        /// </summary>
        public FilePath()
        {
            Init();
        }

        /// <summary>
        /// инициализация путей и имен файлов
        /// </summary>
        private static void Init()
        {
            if (!isReady)
            {
                lock (LockInit)
                {
                    if (!isReady)
                    {
                        Pages = CheckCreateFolder(PagesFolderName);
                        Pics = CheckCreateFolder(PicsFolderName);
                        Log = CheckCreateFolder(LogFolderName) + System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName + ".log";
                        string DataFolder = CheckCreateFolder(DataFolderName);
                        SpellChekDict = DataFolder + SpellChekDictFile;
                        AssocBadDict = DataFolder + AssocBadDictFile;
                        AssocDict = DataFolder + AssocDictFile;
                        BooksDict = DataFolder + BooksDictFile;
                        isReady = true;
                    }
                }
            }
        }

        /// <summary>
        /// если папка есть, или если не было, но удалось создать - возвращает путь к ней, иначе - базовый путь
        /// </summary>
        /// <param name="basepath">базовый путь</param>
        /// <param name="folder">имя папки</param>
        /// <returns>путь к папке</returns>
        private static string CheckCreateFolder(string folder)
        {
            string basepath = Environment.CurrentDirectory;
            string path = basepath + @"\" + folder.Replace("\\","");
            if (!System.IO.Directory.Exists(path))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                catch
                {
                    path = basepath;
                }
            }
            return path + "\\";
        }

        // здесь и ниже - передаем установленные значения путей/файлов
        public string GetLog()
        {
            //if (!isReady) { Init(); }
            return Log;
        }
        public string GetPages()
        {
            //if (!isReady) { Init(); }
            return Pages;
        }
        public string GetPics()
        {
            //if (!isReady) { Init(); }
            return Pics;
        }
        public string GetDictionary()
        {
            //if (!isReady) { Init(); }
            return SpellChekDict;
        }
        public string GetBooks()
        {
            //if (!isReady) { Init(); }
            return BooksDict;
        }
        public string GetAssoc()
        {
            //if (!isReady) { Init(); }
            return AssocDict;
        }
        public string GetAssocBad()
        {
            //if (!isReady) { Init(); }
            return AssocBadDict;
        }

    }
}
