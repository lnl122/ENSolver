// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace ENSolver
{

    /// <summary>
    /// методы работы с игровым движком
    /// </summary>
    interface IEngine
    {
        bool Logon(UserInfo user, string domain);
        List<GameInfo> GetGameList();
        void SetGame(GameInfo selected_game);
        string GetPage(string url);// походу он станет приватным
        string GetLevelPage(int level = -1);
        string SendAnswer(string answer, int level = -1);
    }

    /// <summary>
    /// информация об игроке - имя, пароль
    /// </summary>
    public class UserInfo
    {
        private Log Log = new Log("UserInfo");
        public string name { get; private set; }
        public string pass { get; private set; }
        public string id { get; set; }
        public UserInfo(string u, string p)
        {
            name = u;
            pass = p;
            id = "";
        }
        public void SetIdByPage(string page)
        {
            try
            {
                page = page.Substring(page.IndexOf(name.ToLower()));
                page = page.Substring(page.IndexOf("(id"));
                page = page.Substring(page.IndexOf(">") + 1);
                id = page.Substring(0, page.IndexOf("<"));
            }
            catch
            {
                id = "";
                Log.Write("Не получилось определить id пользователя, хотя логон прошел успешно. ник=" + name);
            }
        }
        public void StoreIntoRegistry()
        {
            Registry reg = new Registry();
            reg.SetUserInfo(this);
        }
    }

    public struct GameInfo
    {
        public string domain;
        public string game_id;
        public string gameurl;
        public string name;
        public DateTime start;
        public DateTime end;
        public bool isStorm;
    }

    public class Engine : IEngine
    {
        // лог
        private static Log Log = new Log("Engine");
        // константы
        //private string url_game_en_cx = "http://game.en.cx/Login.aspx";
        private const string DefaultDomainForLogon = "game.en.cx";
        // готовность к получению уровней и отправке ответов
        private static bool isReady = false;
        // корректность учетных данных
        private static bool isLogged = false;
        // игра
        private static string domain;
        //private static string game_id;
        //private static string gameurl;
        private static UserInfo userinfo;
        // куки
        public static string cHead;
        public static CookieContainer cCont;
        // блокировка операций с движком
        private static object LockAction = new object();

        /// <summary>
        /// конструктор
        /// </summary>
        public Engine()
        {
            // не знаю что конструировать, все данные статические

        }

        /// <summary>
        /// выполняет логон в домене. заполняет часть данных класса Engine
        /// </summary>
        /// <param name="user">информация о имени/пароле</param>
        /// <param name="domain2">домен. если не указан, используем game.en.cx</param>
        /// <returns>успешность логона</returns>
        public bool Logon(UserInfo user, string domain2 = "")
        {
            string page = DoLogon(user, domain2);
            if (isLogonSussefully(page))
            {
                user.SetIdByPage(page);
                user.StoreIntoRegistry();
                isLogged = true;
                userinfo = user;
                domain = domain2;
                return true;
            }
            return false;
        }

        /// <summary>
        /// выполняет логон в домене игры, если домен не установлен - то в game.en.cx
        /// заполняет внутренние поля, сохраняет куки, устанавливает флаг логона
        /// </summary>
        /// <param name="user">UserInfo пользователя</param>
        /// <param name="domain2">пароль</param>
        /// <returns>страница с ответом</returns>
        private string DoLogon(UserInfo user, string domain2 = "")
        {
            string pageSource = "";
            lock (LockAction)
            {
                string domain = "";
                if (domain2 == "") { domain = DefaultDomainForLogon; }
                string formParams = string.Format("Login={0}&Password={1}", user.name, user.pass);
                string cookieHeader = "";
                CookieContainer cookies = new CookieContainer();
                cCont = cookies;
                string url1 = "http://" + domain + "/Login.aspx";
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url1);
                req.CookieContainer = cookies;
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(formParams);
                req.ContentLength = bytes.Length;
                using (Stream os = req.GetRequestStream()) { os.Write(bytes, 0, bytes.Length); }
                try
                {
                    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                    cookieHeader = resp.Headers["Set-cookie"];
                    cHead = cookieHeader;
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream())) { pageSource = sr.ReadToEnd(); }
                }
                catch
                {
                    Log.Write("ERROR: не удалось получить ответ на авторизацию " + url1 + " / " + user.name + " / " + user.pass);
                }
            }
            return pageSource;
        }

        /// <summary>
        /// проверяет страницу на предмет поиска строки с logout
        /// </summary>
        /// <param name="page">код страницы</param>
        /// <returns>true - если мы залогонены, false - если нужен логон</returns>
        private bool isLogonSussefully(string page)
        {
            if (page.IndexOf("action=logout") != -1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// проверяет страницу на предмет поиска строки с Login.aspx
        /// </summary>
        /// <param name="page">код страницы</param>
        /// <returns>true - есть необходимость в логоне</returns>
        private bool isNeedLogon(string page)
        {
            if (page.IndexOf("action=\"/Login.aspx") != -1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// выбирает конкретную игру
        /// </summary>
        /// <param name="selected_game">выбранная игра</param>
        public void SetGame(GameInfo selected_game)
        {
            // скопировать в объект данные игры
            // создать строки урл для получения уровней
            // 
            throw new NotImplementedException("устанавливаем параметры игры, реальных гейминфо пока нет");

            if (isLogged) { isReady = true; }
        }

        /// <summary>
        /// получает список игр, на которые подписан пользователь
        /// </summary>
        /// <returns>список структур с описанием игр</returns>
        public List<GameInfo> GetGameList()
        {
            if (!isLogged) { return new List<GameInfo>(); }
            // получить список игр пользователя
            // выбрать неигранные, создать список
            // для дебуга и себя - добавить игры с демо.ен.цх

            throw new NotImplementedException("не можем создать список игр");
        }

        /// <summary>
        /// получает информацию об одной игре
        /// </summary>
        /// <param name="domain">домен игры</param>
        /// <param name="gamenumber">номер игры</param>
        /// <returns>структура данных об игре</returns>
        private GameInfo GetGameInfo(string domain, string gamenumber)
        {
            // прочитать описание игры по ссылке, вычленить параметры игры, сложить в структуру

            throw new NotImplementedException("получили страницу с описанием игры, но создать объект гейминфо не можем.");
        }

        /// <summary>
        /// пробует отправить ответ в игровой движек,
        /// при запросе авторизации - выполняет её
        /// </summary>
        /// <param name="answer">ответ</param>
        /// <param name="level">номер уровня для штурма, или пусто для линейки</param>
        /// <returns>страница, полученная в ответ</returns>
        public string SendAnswer(string answer, int level = -1)
        {
            if (!isReady) { return ""; }

            // если в ответной странице isNeedLogon надо переавторизоваться и, если успешно - повторить отправку

            throw new NotImplementedException("как бы выполнили отправку ответа, но вернуть страницу с результатом мы не можем.");
        }

        /// <summary>
        /// получает страницу, учитывая сохраненные куки
        /// </summary>
        /// <param name="url">урл</param>
        /// <returns>текст страницы</returns>
        public string GetPage(string url)
        {
            string page = GetPageClean(url);
            if (isNeedLogon(page))
            {
                string logon_res = DoLogon(userinfo, domain);
                if (isLogonSussefully(logon_res))
                {
                    page = GetPageClean(url);
                }
            }
            return page;
        }

        /// <summary>
        /// получает страницу, учитывая сохраненные куки
        /// </summary>
        /// <param name="url">урл</param>
        /// <returns>текст страницы</returns>
        private string GetPageClean(string url)
        {
            string page = "";
            lock (LockAction)
            {
                HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(url);
                try
                {
                    getRequest.CookieContainer = cCont;
                    WebResponse getResponse = getRequest.GetResponse();
                    using (StreamReader sr = new StreamReader(getResponse.GetResponseStream()))
                    {
                        page = sr.ReadToEnd();
                    }
                }
                catch
                {
                    Log.Write("ERROR: Не удалось прочитать страницу " + url);
                    page = "";
                }
            }
            return page;
        }

        /// <summary>
        /// получает страницу с уровнем
        /// </summary>
        /// <param name="level">уровень, если есть необходимость в его указании для штурма, или пусто для линейки</param>
        /// <returns>текст страницы</returns>
        public string GetLevelPage(int level = -1)
        {
            if (!isReady) { return ""; }

            // если на странице встретили "<form ID=\"formMain\" method=\"post\" action=\"/Login.aspx?return=%2fgameengines%2fencounter%2fplay%2f24889%2f%3flevel%3d11"
            // надо переавторизоваться и, если успешно - вернуть страницу

            throw new NotImplementedException("запросили страничку с уровнем, но вернуть её мы неможем");
        }

    }
}
