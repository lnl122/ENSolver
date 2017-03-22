// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
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

    }
}
