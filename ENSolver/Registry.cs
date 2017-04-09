// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using Microsoft.Win32;

namespace ENSolver
{
    /// <summary>
    /// реализует работу с реестром - получение сведений о версии .NET, get/set user/pass
    /// </summary>
    interface IRegistry
    {
        string GetVersionDotNet();
        UserInfo GetUserInfo();
        void SetUserInfo(UserInfo ui);
    }

    public class Registry : IRegistry, System.IDisposable
    {
        //  лог
        private Log Log = new Log("Registry");
        private bool isReady = false;
        // константы
        private const string HCKU_lev1 = "Software";
        private const string HCKU_lev2 = "lnl122";
        private const string HCKU_lev3 = "ENSolver";
        private const string Key_user = "user";
        private const string Key_pass = "pass";

        /// <summary>
        /// инит ветки реестра, создание папок при необходимости
        /// </summary>
        public Registry()
        {
            if (!isReady)
            {
                Init();
                isReady = true;
            }
        }
        /*
        /// <summary>
        /// инит ветки реестра, создание папок при необходимости, сохранение UserInfo
        /// </summary>
        public Registry(UserInfo ui)
        {
            if (!isReady)
            {
                Init();
                isReady = true;
            }
            SetUserInfo(ui);
        }
        */
        /// <summary>
        /// создаем при необходимости ветки реестра для учетки юзера
        /// </summary>
        private void Init()
        {
            if (!isReady)
            {
                RegistryKey rk = Microsoft.Win32.Registry.CurrentUser;
                RegistryKey rks = rk.OpenSubKey(HCKU_lev1, true); rk.Close();
                RegistryKey rksl = rks.OpenSubKey(HCKU_lev2, true); if (rksl == null) { rksl = rks.CreateSubKey(HCKU_lev2); }
                rks.Close();
                RegistryKey rksls = rksl.OpenSubKey(HCKU_lev3, true); if (rksls == null) { rksls = rksl.CreateSubKey(HCKU_lev3); }
                rksl.Close();
                var r_key_u = rksls.GetValue(Key_user);
                if (r_key_u == null) { rksls.SetValue(Key_user, ""); }
                var r_key_p = rksls.GetValue(Key_pass);
                if (r_key_p == null) { rksls.SetValue(Key_pass, ""); }
                rksls.Close();
            }
        }

        /// <summary>
        /// возвращает строку с указанием версий .net
        /// </summary>
        /// <returns>строка с указанием версий .net</returns>
        public string GetVersionDotNet()
        {
            string res = "";
            using (RegistryKey ndpKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
            {
                foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                {
                    if (versionKeyName.StartsWith("v"))
                    {
                        res = res + versionKeyName + " ";
                    }
                }
            }
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    int releaseKey = (int)ndpKey.GetValue("Release");
                    if (releaseKey >= 393295) { res = res + " v4.6"; }
                    else
                    {
                        if ((releaseKey >= 379893)) { res = res + " v4.5.2"; }
                        else
                        {
                            if ((releaseKey >= 378675)) { res = res + " v4.5.1"; }
                            else
                            {
                                if ((releaseKey >= 378389)) { res = res + " v4.5"; }
                            }
                        }
                    }
                }
            }
            return res.Trim();
        }

        /// <summary>
        /// получает из ветки HKCU/Software/lnl122/ENSolver значение ключа
        /// </summary>
        /// <param name="key">ключ</param>
        /// <returns>значение</returns>
        private string GetEnsolverValue(string key)
        {
            if (!isReady) { return ""; }
            string value = "";
            try
            {
                RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(HCKU_lev1 + @"\" + HCKU_lev2 + @"\" + HCKU_lev3, true);
                value = rk.GetValue(key).ToString();
                rk.Close();
            }
            catch
            {
                Log.Write("ERROR: не удалось прочитать содержимое ключа реестра " + key);
            }
            return value;
        }

        /// <summary>
        /// оберстка для Engine
        /// </summary>
        /// <returns>UserInfo из реестра</returns>
        public UserInfo GetUserInfo()
        {
            return new UserInfo(GetEnsolverValue(Key_user), GetEnsolverValue(Key_pass));
        }

        /// <summary>
        /// сохраняет в реестре ник и пароль
        /// </summary>
        /// <param name="u">ник</param>
        /// <param name="p">пароль</param>
        public void SetUserInfo(UserInfo ui)
        {
            RegistryKey rk2 = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(HCKU_lev1 + @"\" + HCKU_lev2 + @"\" + HCKU_lev3, true);
            rk2.SetValue(Key_user, ui.name);
            rk2.SetValue(Key_pass, ui.pass);
            rk2.Close();
        }

        /// <summary>
        /// деструктор
        /// </summary>
        public void Dispose()
        {
            isReady = false;
        }
    }
}
