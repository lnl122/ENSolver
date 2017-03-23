// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENSolver
{

    /// <summary>
    /// методы работы с игровым движком
    /// </summary>
    interface IEngine
    {
        bool Logon(UserInfo userinfo);
        List<string> GetGameList();
        GameInfo GetGameInfo(string domain, string gamenumber);
    }

    public class GameInfo
    {
        public UserInfo user = new UserInfo("", "");
        public string domain;
        public string number;
        public string url;
    }

    public class UserInfo
    {
        public string username;
        public string password;
        public UserInfo(string u, string p)
        {
            username = u;
            password = p;
        }
    }

    public class Engine : IEngine
    {
        // лог
        private Log Log = new Log("Engine");
        // объект создан уже?
        private bool isReady = false;
        private bool isLogged = false;

        private UserInfo userinfo = new UserInfo("", "");

        public bool Logon(UserInfo userinfo)
        {
            throw new NotImplementedException();
        }

        public List<string> GetGameList()
        {
            throw new NotImplementedException();
        }

        public GameInfo GetGameInfo(string domain, string gamenumber)
        {
            throw new NotImplementedException();
        }
    }
}
