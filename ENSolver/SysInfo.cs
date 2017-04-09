// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;

namespace ENSolver
{
    public class SysInfo
    {
        /// <summary>
        /// проверяем наличие, настройки и также работу всех необходимых компонент, ведем лог
        /// </summary>
        /// <returns>признак успешной проверки</returns>
        static public bool Check()
        {
            Log Log = new Log("SysInfo");

            // Environment
            Log.Write("________________________________________________________________________________");
            Log.Write("Старт программы..");
            Log.Write("Сборка от " + System.IO.File.GetCreationTime(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName).ToString());
            Log.Write("ПК: " + Environment.MachineName);
            Log.Write(System.Environment.OSVersion.VersionString + ", " + Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") + ", ver:" + Environment.Version.ToString() + ", CPU: " + Environment.ProcessorCount.ToString() + ", 64bit:" + Environment.Is64BitOperatingSystem.ToString());

            // .NET
            string DotNetVersions = (new Registry()).GetVersionDotNet();
            Log.Write("Найденные версии .NET: " + DotNetVersions);
            if (DotNetVersions.IndexOf("v2.0") == -1) { Log.Write("ERROR: Отсутствует .NET v2.0"); return false; }
            if (DotNetVersions.IndexOf("v3.0") == -1) { Log.Write("ERROR: Отсутствует .NET v3.0"); return false; }
            if (DotNetVersions.IndexOf("v4.0") == -1) { Log.Write("ERROR: Отсутствует .NET v4.0"); return false; }
            if ((DotNetVersions.IndexOf("v4.5") == -1) && (DotNetVersions.IndexOf("v4.6") == -1)) { Log.Write("ERROR: Отсутствует .NET v4.5 или v4.6"); return false; }

            // MS Word
            string WordVersion = GetVersionMicrosoftWord();
            if (WordVersion == "") { Log.Write("ERROR: Отсутствует установленный Microsoft Word"); return false; }
            int ii1 = 0;
            if (Int32.TryParse(WordVersion.Substring(0, WordVersion.IndexOf(".")), out ii1))
            {
                if (ii1 <= 11) { Log.Write("ERROR: Версия Microsoft Word ниже 11.0, необходим Microsoft Word 2007 или более новый"); return false; }
            }
            else
            {
                Log.Write("ERROR: Не удалось определить версию Microsoft Word"); return false;
            }
            Log.Write("Найден Microsoft Word версии " + WordVersion);

            // проверка открытия web-ресурсов
            System.Net.WebClient wc1 = null;
            try { wc1 = new System.Net.WebClient(); } catch { Log.Write("ERROR: Не удалось создать объект WebClient"); return false; }
            string re1 = "";
            try { re1 = wc1.DownloadString("http://image.google.com/"); } catch { Log.Write("ERROR: http://image.google.com/ не открывается"); return false; }
            try { re1 = wc1.DownloadString("http://game.en.cx/"); } catch { Log.Write("ERROR: http://game.en.cx/ не открывается"); return false; }
            try { re1 = wc1.DownloadString("http://goldlit.ru/"); } catch { Log.Write("ERROR: http://goldlit.ru/ не открывается"); return false; }
            try { re1 = wc1.DownloadString("http://sociation.org/"); } catch { Log.Write("ERROR: http://sociation.org/ не открывается"); return false; }
            try { re1 = wc1.DownloadString("https://ru.wiktionary.org/"); } catch { Log.Write("ERROR: https://ru.wiktionary.org/ не открывается"); return false; }
            Log.Write("Все необходимые web-ресурсы открываются успешно");

            // все проверки пройдены
            return true;
        }

        /// <summary>
        /// получаем строку с версией MS Word
        /// </summary>
        /// <returns></returns>
        private static string GetVersionMicrosoftWord()
        {
            try
            {
                var WordApp = new Microsoft.Office.Interop.Word.Application();
                string s1 = WordApp.Version;
                WordApp.Quit();
                return s1;
            }
            catch
            {
                return "";
            }
        }

    }
}
