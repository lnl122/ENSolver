// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

namespace ENSolver
{
    public class ParsePage
    {
        /// <summary>
        /// вырезает между указанными тегами
        /// </summary>
        /// <param name="g">текст страницы</param>
        /// <param name="tags">массив тегов</param>
        /// <returns></returns>
        public static string ParseTags(string g, string[,] tags)
        {
            if (g.Length < 1) { return ""; }
            int tags_len = tags.Length / 2;
            bool fl = true;
            for (int i = 0; i < tags_len; i++)
            {
                fl = true;
                while (fl)
                {
                    fl = false;
                    int i1 = g.IndexOf(tags[0, i]);
                    if (i1 != -1)
                    {
                        string g2 = g.Substring(i1 + tags[0, i].Length);
                        int i2 = g2.IndexOf(tags[1, i]);
                        g = g.Substring(0, i1) + g2.Substring(i2 + tags[1, i].Length);
                        fl = true;
                    }
                }
            }
            return g;
        }

        /// <summary>
        /// убирает плохие слова из строки
        /// </summary>
        /// <param name="g">строка</param>
        /// <param name="bad">массив плохих слов</param>
        /// <returns>строка без неугодных слов</returns>
        public static string RemoveBadWords(string g, string[] bad)
        {
            if (g.Length < 1) { return ""; }
            for(int i = 0; i < bad.Length; i++)
            {
                if(bad[i] == "") { continue; }
                g = g.Replace(bad[i], " ");
            }
            return g;
        }

        /// <summary>
        /// убирает мнемоники вроде &nbsp; &gt; &lt; из строки
        /// </summary>
        /// <param name="g">строка</param>
        /// <returns>строка без мнемоник</returns>
        public static string RemoveMnemonics(string g)
        {
            System.Collections.Generic.List<string> mnem = new System.Collections.Generic.List<string>();
            string[] amp = g.Split('&');
            for (int i = 0; i < amp.Length; i++)
            {
                int l = amp[i].IndexOf(';');
                if ((l <= 6) && (l >= 2))
                {
                    string s = amp[i].Substring(0, l);
                    bool fl = true;
                    for (int j = 0; j < s.Length; j++)
                    {
                        if ((s[j] < 'a') || (s[j] > 'z')) { fl = false; }
                    }
                    if (fl)
                    {
                        if (!mnem.Contains(s)) { mnem.Add(s); }
                    }
                }
            }
            foreach (string s in mnem)
            {
                g = g.Replace("&" + s + ";", " ");
            }
            return g;
        }        
        
        /// <summary>
        /// убирает короткие слова
        /// </summary>
        /// <param name="g">строка</param>
        /// <returns>строка без коротышей</returns>
        public static string RemoveShortWords(string g)
        {
            System.Text.StringBuilder res = new System.Text.StringBuilder();
            string[] wrds = g.Split(' ');
            for(int i = 0; i < wrds.Length; i++)
            {
                string q = wrds[i];
                int l = q.Length;
                if(l >= 3)
                {
                    if ( ((q[0] > '9') || (q[0] < '0')) && ((q[l-1] > '9') || (q[l-1] < '0')))
                    {
                        res.Append(q);
                        res.Append(' ');
                    }
                }
            }
            return res.ToString();
        }
    }
}
