// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System.Collections.Generic;

namespace ENSolver
{
    /// <summary>
    /// получение урл загруженной пикчи из локального пути к файлу с картинкой
    /// при ошибке пробуем разных хостеров.
    /// при невозможности аплоада возвращает пустую строку
    /// (!) новые методы следует добавлять в конструктор
    /// </summary>
    interface IUpload
    {
        string GetUrl(string path);
        List<string> GetUrls(List<string> path);
    }

    public class Upload : IUpload
    {
        // лог
        private static ILog Log = new Log("Upload");
        // текущий механизм аплоада
        private static int hoster_index;
        private static List<IUploadHoster> hosters;
        private static int hosters_count;
        private static IUploadHoster Method;
        // объекты для блокировки
        private static object LockChangeMethod = new object();

        /// <summary>
        /// конструктор
        /// </summary>
        public Upload()
        {
            // лог
            Log.Write("Новый объект");
            // добавляем в список все наши методы
            hosters = new List<IUploadHoster>();
            //hosters.Add(new HosterPixicRu()); // - перестал работать в Казахстане
            hosters.Add(new HosterIpicSu());
            hosters.Add(new HosterIi4Ru());
            //hosters.Add(new HosterJpegshareNet()); // - иногда возникают ошибки EnsureSuccessStatusCode();
            hosters.Add(new HosterPixshockNet());
            hosters.Add(new HosterSaveimgRu());
            hosters.Add(new HosterSavepicRu());
            hosters.Add(new HosterRadikalRu());
            hosters.Add(new HosterFreeimagehostingNet());
            //
            hosters_count = hosters.Count;
            // выбираем начальный метод
            hoster_index = 0;
            Method = hosters[hoster_index];
        }

        /// <summary>
        /// загружает картинку одним из известных нам хостеров изображений
        /// </summary>
        /// <param name="path">путь к загружаемому файлу</param>
        /// <returns>линк на загруженный файл</returns>
        public string GetUrl(string path)
        {
            if (!System.IO.File.Exists(path)) { return ""; }
            // первая попытка
            int attempts = 1;
            string res = Method.GetUrl(path);
            // если ответ не корректный или попытки не закончились - поменяем метод и повторим
            while (!isCorrectUrl(res) || (attempts * 2 == hosters_count))
            {
                lock (LockChangeMethod)
                {
                    hoster_index = (++hoster_index) % hosters_count;
                    Method = hosters[hoster_index];
                }
                attempts++;
                res = Method.GetUrl(path);
            }
            if (!isCorrectUrl(res)) { res = ""; }
            return res;
        }

        /// <summary>
        /// получает ссылки загруженных картинок по списку путей к файлам с ними
        /// </summary>
        /// <param name="paths">список путей файлов изображений</param>
        /// <returns>список урл-ов</returns>
        public List<string> GetUrls(List<string> paths)
        {
            List<string> res = new List<string>();
            foreach(string p in paths)
            {
                res.Add(GetUrl(p));
            }
            return res;
        }

        /// <summary>
        /// проверка корректности полученной ссылки
        /// </summary>
        /// <param name="sd">проверяемая ссылка</param>
        /// <returns>результат проверки</returns>
        private bool isCorrectUrl(string sd)
        {
            if (sd.Length < 5)
            {
                Log.Write("ERROR: вернулась слишком короткая ссылка");
                Log.Write(sd);
                return false;
            }
            if (sd.Substring(0, 4) != "http")
            {
                Log.Write("ERROR: то что вернулось не является ссылкой http");
                Log.Write(sd);
                return false;
            }
            return true;
        }
    }
}
