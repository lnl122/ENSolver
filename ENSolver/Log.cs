// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;

namespace ENSolver
{
    /// <summary>
    /// реализует работу с лог-файлом
    /// </summary>
    interface ILog
    {
        //void Log(string str);
        void Write(string str);
        void Store(string str);
        void Flush();
    }

    public class Log : ILog
    {
        // имя текущего логгируемого модуля
        private string ModuleName = "";

        // объекты блокировки
        private static object LockWrite = new object();
        private static object LockStore = new object();

        // пути
        private static string PathToPages = "";
        // поток лог-файла, в него будет вестись дозапись лога
        private static System.IO.StreamWriter logfile;
        // прризнак проведенной инициализации
        private static bool isReady = false;
        // объекты для блокировки
        private static object LockInit = new object();
        // индекс (порядковый номер)  сохраняемого файла страниц (.http)
        private static int fileidx = 100;

        /// <summary>
        /// инициализирует лог файл, если нету его - создает. в т.ч. необходимые папки
        /// </summary>
        public Log(string mn = "")
        {
            if (mn != "") { ModuleName = mn + ": "; }
            if (!isReady)
            {
                lock (LockInit)
                {
                    if (!isReady)
                    {
                        FilePath fp = new FilePath();
                        PathToPages = fp.GetPages();
                        logfile = new System.IO.StreamWriter(System.IO.File.AppendText(fp.GetLog()).BaseStream);
                        logfile.AutoFlush = true;
                        isReady = true;
                    }
                }
            }
        }

        /// <summary>
        /// записывает строку текста в лог-файл
        /// </summary>
        /// <param name="str">строка для лог файла</param>
        /// <param name="str2">вторая строка для лог файла</param>
        /// <param name="str3">третья строка для лог файла</param>
        public void Write(string str)
        {
            if (isReady)
            {
                lock (LockWrite)
                {
                    logfile.WriteLine("{0} {1}\t{2}\t{3}",
                        DateTime.Today.ToShortDateString(),
                        DateTime.Now.ToLongTimeString(),
                        ModuleName,
                        str);
                }
            }
        }

        /// <summary>
        /// записывает текст в отдельный файл
        /// </summary>
        /// <param name="modulename">имя файла/модуля</param>
        /// <param name="text">строка текста</param>
        public void Store(string text)
        {
            if (isReady)
            {
                lock (LockStore)
                {
                    fileidx++;
                    var dn = DateTime.Now;
                    string path = PathToPages + ModuleName.Replace(": ","") + "_" + LeadZero(fileidx) + "_" +
                        LeadZero(dn.Year) + LeadZero(dn.Month) + LeadZero(dn.Day) +
                        LeadZero(dn.Hour) + LeadZero(dn.Minute) + LeadZero(dn.Second) + ".http";
                    System.IO.File.WriteAllText(path, text, System.Text.Encoding.UTF8);

                }
            }
        }

        /// <summary>
        /// перевод числа в строку, добавление лидирующих нулей
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private string LeadZero(int num)
        {
            int zeros = 4;
            if(num < 100) { zeros = 2; }
            string str = "0000" + num.ToString();
            return str.Substring(str.Length - zeros);
        }

        /// <summary>
        /// выполняет принудительную запись лога на диск
        /// </summary>
        public void Flush()
        {
            if (isReady)
            {
                logfile.Flush();
            }
        }

        /// <summary>
        /// выполняет принудительную запись лога на диск, убивает объект лог-файла
        /// </summary>
        public void Close()
        {
            if (isReady)
            {
                logfile.Flush();
                logfile.Close();
                logfile = null;
                isReady = false;
            }
        }
    }
}
