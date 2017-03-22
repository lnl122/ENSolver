// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ENSolver
{
    /// <summary>
    /// методы работы с google.com
    /// </summary>
    interface IGoogle
    {
        string GetPageByImageUrl(string imgurl);
        string ParsePage(string page);
    }

    public class Google : IGoogle
    {
        // лог
        private static ILog Log = new Log("Google");
        // пути
        private static string googleRU = "https://www.google.ru/searchbyimage?&hl=ru-ru&lr=lang_ru&image_url=";
        // максимальное количество попыток чтения
        private static int MaxTryToReadPage = 3;
        // на сколько миллисекунд засыпать при неудачном одном чтении
        private static int TimeToSleepMs = 1000;
        // UserAgent
        public static string UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1";

        private static string[,] tags_script = {
                { "<script>" , "<noscript>" , "<style>" , "href=\"" },
                { "</script>", "</noscript>", "</style>", "\""      }
            };
        private static string[,] tags_1 = {
                { "onmousedown=\"", "value=\"", "data_arrtid=\"", "data_hveid=\"", "data-jiis=\"", "data-ved=\"", "aria-label=\"", "jsl=\"", "data-jibp=\"", "role=\"", "jsaction=\"", "onload=\"", "alt=\"", "title=\"", "marginwidth=\"", "marginheight=\"", "width=\"", "height=\"", "data-deferred=\"", "aria-haspopup=\"", "aria-expanded=\"", "<input", "tabindex=\"", "tag=\"", "aria-selected=\"", "name=\"", "type=\"", "action=\"", "method=\"", "autocomplete=\"", "aria-expanded=\"", "aria-grabbed=\"", "data-bucket=\"", "aria-level=\"", "aria-hidden=\"", "aria-dropeffect=\"", "topmargin=\"" , "margin=\"", "data-async-context=\"", "valign=\"", "data-async-context=\"", "unselectable=\"", "style=\"" , "class=\"" , "//<![CDATA[" , "border=\"" , "cellspacing=\"" , "cellpadding=\"" , "target=\"" , "colspan=\"" , "onclick=\"" , "align=\"" , "color=\"" , "nowrap=\"" , "vspace=\"" , "href=\"" , "src=\"" },
                { "\""            , "\""      , "\""            , "\""           , "\""          , "\""         , "\""           , "\""    , "\""          , "\""     , "\""         , "\""       , "\""    , "\""      , "\""            , "\""             , "\""      , "\""       , "\""              , "\""              , "\""              , ">"     , "\""         , "\""    , "\""              , "\""     , "\""     , "\""       , "\""       , "\""             , "\""              , "\""             , "\""            , "\""           , "\""            , "\""                , "\""           , "\""       , "\""                   , "\""       , "\""                   , "\""             , "\""       , "\""       , "//]]>"       , "\""        , "\""             , "\""             , "\""        , "\""         , "\""         , "\""       , "\""       , "\""        , "\""        , "\""      , "\""     }
            };


        public string GetPageByImageUrl(string imgurl)
        {
            string gurl = googleRU + imgurl;
            WebClient wc = new WebClient();
            wc.Encoding = System.Text.Encoding.UTF8;
            wc.Headers.Add("User-Agent", UserAgent);
            wc.Headers.Add("Accept-Language", "ru-ru");
            wc.Headers.Add("Content-Language", "ru-ru");
            string page = "";
            bool isNeedReadPage = true;
            int CountTry = 0;
            while (isNeedReadPage)
            {
                try
                {
                    page = wc.DownloadString(gurl);
                    isNeedReadPage = false;
                }
                catch
                {
                    CountTry++;
                    if (CountTry == MaxTryToReadPage)
                    {
                        Log.Write("ERROR: не удалось получить страницу гугля для изображение по ссылке ");
                        Log.Write(imgurl);
                        Log.Store(page);
                        page = "";
                        isNeedReadPage = false;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(TimeToSleepMs);
                    }
                }
            }
            wc.Dispose();
            wc = null;

            if (page.Length == 0)
            {
                Log.Write("ERROR: длина строки нулевая");
                return "";
            }
            page = page.ToLower().Replace("\t", " ").Replace("\n", " ");
            int body1 = page.IndexOf("<body");
            int body2 = page.IndexOf("</body>");
            if ((body1 == -1) || (body2 == -1))
            {
                Log.Write("ERROR: нет тегов <body> у страницы");
                Log.Store(page);
                return "";
            }
            page = page.Substring(body1 + 5, body2 - body1 - 5);
            int tag_a = page.IndexOf("<!--a-->");
            if (tag_a != -1) { page = page.Substring(tag_a + 8); }
            page = ParsePage(page);
            Log.Store(page);
            return page;
        }

        public string ParsePage(string page)
        {
            page = ENSolver.ParsePage.ParseTags(page, tags_script);
            page = ENSolver.ParsePage.ParseTags(page, tags_1);
            page = ENSolver.ParsePage.ParseTags(page, tags_2);
            page = ENSolver.ParsePage.ParseTags(page, tags_3);
            return page;
        }
        private static string[,] tags_2 = {
                { "<svg"  , "<g-img"  , "<cite"  , "data-pid=\"", "data-rtid=\"", "data-hveid=\"", "data-attrid=\"", "data-md=\"", "<div >{", "<div>{"  , "<g-text-field"  , "<g-menu"  , "<g-dropdown-menu"  , "<g-popup"  , "eid=\"", "onsubmit=\"", "<g-dialog"   },
                { "</svg>", "</g-img>", "</cite>", "\"",          "\""          ,"\""            , "\""            , "\""        , "}</div>", "}</div>" , "</g-text-field>", "</g-menu>", "</g-dropdown-menu>", "</g-popup>", "\""    , "\""         , "</g-dialog>" }
            };
        private static string[,] tags_3 = {
                { "id=\""  },
                { "\"",    }
            };

    }
}
