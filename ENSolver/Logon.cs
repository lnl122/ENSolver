// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using Microsoft.Win32;
using System.Windows.Forms;

namespace ENSolver
{
    interface ILogon {
        bool GetResult();
    }

    /// <summary>
    /// выполняет экранную форму логона
    /// </summary>
    public class Logon : ILogon
    {
        private Log Log = new Log("Logon");
        private int border = 5; // расстояния между элементами форм, константа
        private TextBox tu;
        private TextBox tp;
        private bool isSuccessful = false;

        /// <summary>
        /// возвращает успешность логона
        /// </summary>
        /// <returns>флаг успешностти логона</returns>
        public bool GetResult()
        {
            return isSuccessful;
        }

        /// <summary>
        /// выполняет логон
        /// </summary>
        public Logon()
        {
            Registry reg = new Registry();
            UserInfo ui = reg.GetUserInfo();
            Engine engine = new Engine();

            // форма для ввода данных
            Form Login = CreateForm(ui.name, ui.pass);

            // предложим ввести юзера и пароль, дефолтные значения - то, что было в реестре, или же пусто
            bool fl = true;
            while (fl)
            {
                if (Login.ShowDialog() == DialogResult.OK)
                {
                    // попробуем авторизоваться на гейм.ен.цх с указанной УЗ
                    ui.name = tu.Text;
                    ui.pass = tp.Text;
                    Log.Write("Пробуем выполнить вход на сайт для пользвоателя " + ui.name);

                    if (engine.Logon(ui))
                    {
                        reg.SetUserInfo(ui);
                        fl = false;
                        isSuccessful = true;
                        Log.Write("Имя и пароль пользователя проверены, успешный логон для пользвоателя " + ui.name);
                    }
                    else
                    {
                        // если не успешно - вернемся в вводу пользователя
                        isSuccessful = false;
                        Log.Write("Неверные логин/пароль");
                        MessageBox.Show("Неверные логин/пароль");
                    }
                }
                else
                {
                    // если отказались вводить имя/пасс - выходим
                    fl = false;
                }
            } // выход только если fl = false -- это или отказ пользователя в диалоге, или если нажато ОК - корректная УЗ
        }

        /// <summary>
        /// создаем экранную форму логона
        /// </summary>
        /// <param name="user">ник</param>
        /// <param name="pass">пароль</param>
        /// <returns>форма</returns>
        private Form CreateForm(string user, string pass)
        {
            Form Login = new Form();
            Login.Text = "Авторизация..";
            Login.StartPosition = FormStartPosition.CenterScreen;
            Login.Width = 35 * border;
            Login.Height = 25 * border;
            Login.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Login.AutoSize = true;
            //Login.Icon = Properties.Resources.icon2;
            Label lu = new Label();
            lu.Text = "ник:";
            lu.Top = 2 * border;
            lu.Left = border;
            lu.Width = 10 * border;
            Login.Controls.Add(lu);
            Label lp = new Label();
            lp.Text = "пароль:";
            lp.Top = lu.Bottom + border;
            lp.Left = border;
            lp.Width = lu.Width;
            Login.Controls.Add(lp);
            tu = new TextBox();
            tu.Text = user;
            tu.Top = lu.Top;
            tu.Left = lu.Right + border;
            tu.Width = 3 * lu.Width;
            Login.Controls.Add(tu);
            tp = new TextBox();
            tp.Text = pass;
            tp.Top = lp.Top;
            tp.Left = tu.Left;
            tp.Width = tu.Width;
            Login.Controls.Add(tp);
            Button blok = new Button();
            blok.Text = "выполнить вход";
            blok.Top = lp.Bottom + 2 * border;
            blok.Left = lu.Left;
            blok.Width = tu.Right - 1 * border;
            blok.DialogResult = DialogResult.OK;
            Login.AcceptButton = blok;
            Login.Controls.Add(blok);
            return Login;
        }
    }
}
