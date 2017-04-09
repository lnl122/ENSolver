// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using System.Net;

namespace ENSolver
{
    /// <summary>
    /// методы работы с google.com
    /// </summary>
    interface IGoogle
    {
        string GetWordsByImgUrl(string imgurl);
        string GetWordsByImgFile(string imgpath);
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
        private static int MaxTryToReadPage = 5;
        // на сколько миллисекунд засыпать при неудачном одном чтении
        private static int TimeToSleepMs = 2000;
        // UserAgent
        //private static string UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1";
        private static string UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36";
        // Изображение слишком велико либо его нельзя загрузить из-за низкой скорости интернет-подключения.
        private static string LowSpeed1 = "Изображение слишком велико либо его нельзя загрузить из-за низкой скорости интернет-подключения.";
        //private static string LowSpeed2 = "отключен из-за низкой скорости интернет-подключения.";

        private static string[,] tags_script = {
            { "<script>" , "<noscript>" , "<style>" , "//<![CDATA[", "<kno-share-button>"  },
            { "</script>", "</noscript>", "</style>", "//]]>"      , "</kno-share-button>" }
        };
        private static string[,] tags_2 = {
            { "<svg"  , "<g-img"  , "<cite"  , ">{"     , "<g-section-with-header>" , "<g-text-field"  , "<g-menu"  , "<g-dropdown-menu"  , "<g-popup"  ,  "<g-dialog"  , "<image-viewer-group>" },
            { "</svg>", "</g-img>", "</cite>", "}</div" , "</g-section-with-header>", "</g-text-field>", "</g-menu>", "</g-dropdown-menu>", "</g-popup>",  "</g-dialog>", "</image-viewer-group>" }
        };
        private static string[,] tags_param = {
            { "=\"" },
            { "\""  }
        };
        private static string[] bad_words_1 = {
            "onmousedown", "value", "data_arrtid", "data_hveid", "data-jiis", "data-ved", "aria-label", "jsl", "data-jibp", "role", "jsaction", "onload", "alt", "title",
            "marginwidth", "marginheight", "width", "height", "data-deferred", "aria-haspopup", "aria-expanded", "<input", "tabindex", "tag", "aria-selected", "name", "type",
            "action", "method", "autocomplete", "aria-expanded", "aria-grabbed", "data-bucket", "aria-level", "aria-hidden", "aria-dropeffect", "topmargin" , "margin",
            "data-async-context", "valign", "data-async-context", "unselectable", "style" , "class" , "border" , "cellspacing" , "cellpadding" , "target" , "colspan" ,
            "onclick" , "align" , "color" , "nowrap" , "vspace" , "href" , "src", "data-pid", "data-rtid", "data-hveid", "data-attrid", "data-md", "eid", "onsubmit", "id",
            "data-async-trigger", "data-async-", "-required", "data-du", "data-fi", "data-lhe", "data-lve", "data-oslg", "data-t", "data-d", "data-eca", "data-h", "data-m", "data-nr"
        };
        private static string[] bad_tags_1 = {
            "<div>", "<div >", "</div>", "<span >", "<span>", "</span>", "<a >", "<a>", "</a>", "<table>", "<table >", "</table>", "<tr>", "<tr >", "</tr>", "<td>", "<td >",
            "</td>", "<li>", "<li >", "</li>", "<ol>", "<ol >", "</ol>", "<h1>", "<h1 >", "</h1>", "<h2>", "<h2 >", "</h2>", "<h3>", "<h3 >", "</h3>", "<em>", "</em>",
            "&nbsp;", "&times;", "<!--n-->", "<!--m-->", "<!--z-->", "—"
        };
        private static string[] bad_words_2 = {
            "результаты поиска", "сохраненная копия", "похожие", "страницы с подходящими изображениями", "смотреть онлайн", "сообщений", "оставить отзыв", "отправить отзыв",
            "подробнее справка", "подробнее", "конфиденциальность", "условия", "следующая", "<wbr>", "youtube", "facebook", "vkontakte"
            , " янв. ", " фев. ", " мар. ", " апр. ", "май. ", " мая ", " июн. ", " июл. ", " авг. ", " сен. ", " окт. ", " ноя. ", " дек. ",
            " png ", " jpg ", " jpeg ", " svg ", " bmp ", " gif ", " avi ", " mp3 ", " mp4 ", " flv "
            , " сайт ", " сайта ", "powered by wikia", "все серии подряд", " fandom ", " lurkmore ", " png ", " фото ", " оставить отзыв ", " youtu.be ", " youtube ", " youtu ",
            " http ", "перейти к разделу" , " янв ", " фев ", " мар ", " апр ", " июн ", " июл ", " авг ", " сен ", " окт ", " ноя ", " дек ", " клипарт ", " мои альбомы ",
            " сент " , " блог ", " обои ", " скачать ", "для рабочего стола", " картинки ", " dot ", " просмотреть "
            , " about "
        };
        private static string[] bad_words_3 = {
            "10 справка", "перевести эту страницу", " википедия ", " ещё ", " запросы ", " видео ", " февр ", "режим работы", " что ", " кто ", " где ", " был ", " для ", "риа новости",
            " риа ", " новости ", " во ", " яндекс ", " yandex ", " google ", " com ", "библиотека изображений", " это ", " wbr ", "1 2 3 4 5 6 7 8 9",
            " на ", " для ", " из ", " по ", " как ", " не ", " от ", " что ", " это ", " или ", " вконтакте ", " review ", " png ", " the ",
            " за ", " вы ", " все ", " википедия ", " во ", " год ", " paradise ", " том ", " эту ", " of ", " размер ", " руб ", " бесплатно ", " его ", " клипарт ",
            " описание ", " есть ", " картинки ", " фотографии ", " их ", " for ", " to ", " можно ", " мы ", " назад ", " но ", " так ", " ми ", " они ", " он ",
            " если ", " москве ", " продажа ", " сайт ", " то ", " только ", " цене ", " чтобы ", " and ", " при ", " чем ", " free ", " без ", " где ", " очень ",
            " со ", " by ", " toys ", " two ", " вас ", " всех ", " кто ", " многие ", " может ", " чему ", " яндекс ", " вот ", " нет ", " сша ", " характеристики ",
            " ценам ", " же ", " ли ", " можете ", " нас ", " обзор ", " про ", " современные ", " того ", " уже ", " фоне ", " &amp ", " body ", " какой ", " под ",
            " сайте ", " сравнить ", " ооо ", " себя ", " этой ", " является ", " in ", " mb ", " бы ", " вам ", " об ", " также ", " liveinternet ", " заказать ",
            " здесь ", " какие ", " лучшие ", " vk ", " http ", " https ", " ru ", " com ", " net ", " org ", " youtube ", " vkontakte ", " facebook ", " фото ",
            " видео ", " смотреть ", " купить ", " куплю ", " продам ", " продать ", " онлайн ", " обои ", " цена ", " цены ", " найти ", " самые ", " самых ",
            " самый ", " самая ", " фильм ", " отзывы ", " фильма ", " фильм ", " фильму ", " разрешение ", " разрешении ", " скидка ", " скидки ", " выбрать ",
            " закачка ", " закачки ", " новости ", " скачать ", " форматы ", " хорошем ", " качестве ", " свойства ", " смотреть ", " страницу ", " бесплатные ",
            " программы ", " перевести ", " td ", " td ", " is ", " i ", " < ", " > ", " design ", " data ", " material ", " div ", " wikipedia ", " with ", " был ",
            " лет ", " g ", " on ", " that ", " быть ", " интересные ", " new ", " stars ", " this ", " from ", " google ", " была ", " всё ", " еще ", " i ", " jpg ",
            " online ", " or ", " png ", " jpeg ", " главная ", " доставкой ", " изготовление ", " no ", " over ", " web ", " янв ", " фев ", " мар ", " апр ", " май ",
            " июн ", " июл ", " авг ", " сен ", " окт ", " ноя ", " дек ", " пн ", " вт ", " ср ", " чт ", " пт ", " сб ", " вс "
        };
        private static string[] bad_words_4 = {
            " года ", " родился ", " через ", " фильмы ", " родился ", " через ", " фильмы ", " соответствии ", " интернет ", " могли ", " некоторые ", " биография ", " году ",
            " местным ", " удалены ", " этот ", " нояб ", " части ", " мин ", " фильмография ", " кинопоиск ", " название ", " премьера ", " такое ", " sub ", " англ ", " более ",
            " которая ", " рисунок ", " about ", " fhm ", " panther ", " когда ", " самое ", " файл ", " фильмов ", " more ", " pikabu ", " start ", " аксессуары ", " другие ",
            " информация ", " которые ", " который ", " логотип ", " моё ", " она ", " особенности ", " этом ", " было ", " даже ", " изображения ", " однако ", " основу ",
            " откуда ", " после ", " последние ", " рейтинг ", " сериалы ", " are ", " edition ", " image ", " images ", " ipad ", " olx ", " select ", " trade ", " биографических ",
            " всегда ", " жанре ", " значение ", " именно ", " картинка ", " которого ", " которое ", " мобильный ", " названы ", " народный ", " настоящее ", " начале ", " несколько ",
            " определение ", " первая ", " первого ", " различия ", " различных ", " самой ", " себе ", " сколько ", " содержит ", " тем ", " чьи ", " buy ", " file ", " food ",
            " home ", " information ", " iphone ", " java ", " loading ", " man ", " mpg ", " near ", " news ", " oil ", " page ", " see ", " shop ", " span ", " steam ", " stock ",
            " today ", " was ", " your ", " бывшая ", " ваш ", " ведь ", " всего ", " выборе ", " главной ", " другое ", " игры ", " известен ", " изображение ", " имеет ", " креатив ",
            " лучший ", " мне ", " мой ", " наш ", " ответы ", " первых ", " подробная ", " полезные ", " почти ", " предлагаем ", " предпросмотр ", " рублей ", " состоит ",
            " специальные ", " такой ", " тот ", " часто ", " этого ", " connect ", " event ", " find ", " full ", " international ", " logo ", " mail ", " max ", " now ", " sale ",
            " special ", " store ", " twitter ", " большая ", " будущего ", " виды ", " вполне ", " всем ", " говорить ", " данной ", " данный ", " дают ", " довольно ", " другим ",
            " других ", " ежедневно ", " ему ", " используется ", " каждого ", " каждый ", " картинку ", " картинок ", " кинопоиске ", " компьютерная ", " красивая ", " лишь ",
            " любой ", " между ", " меня ", " много ", " мужские ", " мужскую ", " называют ", " нам ", " начнется ", " наша ", " нашей ", " нашем ", " некоторых ", " обоев ",
            " общественный ", " понимают ", " потому ", " почему ", " прежде ", " разных ", " сейчас ", " следующих ", " смотрите ", " совсем ", " содержание ", " состоялась ",
            " средняя ", " станете ", " такие ", " там ", " твой ", " qualified ", " administration ", " all ", " app ", " around ", " automatic ", " boxnews ", " can ", " common ",
            " dir ", " direction ", " drive ", " engine ", " events ", " files ", " finder ", " forums ", " foto ", " get ", " good ", " gsm ", " has ", " help ", " illustration ",
            " info ", " into ", " itunes ", " market ", " mini ", " price ", " request ", " results ", " search ", " share ", " site ", " test ", " tool ", " views "
        };

        /// <summary>
        /// получает гугл страницу поиска по картинке
        /// </summary>
        /// <param name="imgurl">урл изображения</param>
        /// <returns>текст страницы</returns>
        public string GetPageByImageUrl(string imgurl)
        {
            DateTime dt = DateTime.Now;
            string gurl = googleRU + imgurl;
            WebClient wc = new WebClient();
            
            wc.Encoding = System.Text.Encoding.UTF8;
            wc.Headers.Add("User-Agent", UserAgent);
            wc.Headers.Add("Accept-Language", "ru-ru");
            wc.Headers.Add("Content-Language", "ru-ru");

            ////wc.Headers.Add("Host", "www.google.ru");
            ////wc.Headers.Add("Connection", "keep-alive");
            //wc.Headers.Add("User-Agent", UserAgent);
            //wc.Headers.Add("Upgrade-Insecure-Requests", "1");
            ////wc.Headers.Add("X-Client-Data", "CI22yQEIo7bJAQjEtskBCPqcygEIqZ3KAQ==");
            //wc.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            //wc.Headers.Add("Accept-Encoding", "gzip, deflate, sdch, br");
            //wc.Headers.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
            ////wc.Headers.Add("Cookie", "OGPC=89860096-1:; SID=OQRZ8u6TUiAbiNlbEQVyPhhL52B_ws35WZgMHu_rzQvuvvM4vqYDRf0iclsDiLLIeyMcWA.; HSID=AOLz67J9w3Mv-Ygep; SSID=AVYI68Y4HVB01stBz; APISID=BYxz5Vc3HxPmlB1d/APIlCFElGjEfOOpQy; SAPISID=FypqHQFwvb5cPXvp/AELyspmq4__h70l7Z; NID=100=JT7s2fvtu7AkFUJICA8IdhA7tCbsIsFOWYeehmX63RnGC8jVuRZ4nWTMxnw0RgiLBasC253gJehuFPi_pSEtVrGuKDvnVV4_-pzdZKLp91vucJIHiT8Y-6LFeo9eMH7y5-am5H-Dj_-hzqHRV98szsJ5yFTULlaNdkyZZFedoGWD8u1noIgAErmqau9wLa8_GpYbaWhK3vraPDq9t44yFJA");
            //wc.Headers.Add("Content-Language", "ru-ru");

            string page = "";
            bool isNeedReadPage = true;
            int CountTry = 0;
            while (isNeedReadPage)
            {
                try
                {
                    page = wc.DownloadString(gurl);
                    //if ((page.IndexOf(LowSpeed1) == -1) && (page.IndexOf(LowSpeed2) == -1))
                    if (page.IndexOf(LowSpeed1) == -1)
                    {
                            isNeedReadPage = false;
                        //page = "";
                    }
                    else
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

            //page = ParsePage(page);
            Log.Store(page);
            Log.Write("Dowload time: " + (Math.Floor((DateTime.Now - dt).TotalMilliseconds) / 1000).ToString());
            return page;
        }

        /// <summary>
        /// парсит страницу поиска гугля
        /// </summary>
        /// <param name="page">текст страницы</param>
        /// <returns>найденные слова</returns>
        public string ParsePage(string page)
        {
            DateTime dt = DateTime.Now;
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
            if (tag_a != -1)
            {
                page = page.Substring(tag_a + 8);
            }

            page = ENSolver.ParsePage.ParseTags(page, tags_script);
            page = ENSolver.ParsePage.ParseTags(page, tags_2);
            page = ENSolver.ParsePage.ParseTags(page, tags_param);
            page = page.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
            page = ENSolver.ParsePage.RemoveBadWords(page, bad_words_1);
            page = page.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
            page = ENSolver.ParsePage.RemoveBadWords(page, bad_tags_1);
            page = ENSolver.ParsePage.RemoveMnemonics(page);

            page = page.Replace("!", " ").Replace("@", " ").Replace("#", " ").Replace("$", " ").Replace("%", " ").Replace("^", " ").Replace("&", " ").Replace("*", " ");
            page = page.Replace("(", " ").Replace(")", " ").Replace("_", " ").Replace("+", " ").Replace("-", " ").Replace("=", " ").Replace("`", " ").Replace("~", " ");
            page = page.Replace("\"", " ").Replace("№", " ").Replace(";", " ").Replace(":", " ").Replace("?", " ").Replace("[", " ").Replace("]", " ").Replace("{", " ");
            page = page.Replace("}", " ").Replace("\\", " ").Replace("|", " ").Replace("'", " ").Replace(",", " ").Replace(".", " ").Replace("<", " ").Replace(">", " ");
            page = page.Replace("«", " ").Replace("»", " ").Replace("…", " ").Replace("‧", " ").Replace("/", " ");
            page = page.Replace("¡", " ").Replace("“", " ").Replace("”", " ");

            page = page.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
            page = ENSolver.ParsePage.RemoveBadWords(page, bad_words_2);
            page = ENSolver.ParsePage.RemoveBadWords(page, bad_words_3);
            page = ENSolver.ParsePage.RemoveBadWords(page, bad_words_4);
            page = ENSolver.ParsePage.RemoveShortWords(page);
            page = page.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
            page = page.Trim();
            Log.Write("Parse time: "+(Math.Floor((DateTime.Now - dt).TotalMilliseconds)/1000).ToString());
            Log.Store(page);
            return page;
        }

        /// <summary>
        /// получает слова с гугл страницы поиска по картинке, урл которой указан
        /// </summary>
        /// <param name="imgpath">путь к изображению</param>
        /// <returns>слова строкой</returns>
        public string GetWordsByImgUrl(string imgurl)
        {
            string page = GetPageByImageUrl(imgurl);
            if (page == "") { return ""; }
            string wrds = ParsePage(page);
            return wrds;
        }

        /// <summary>
        /// получает слова с гугл страницы поиска по картинке, путь к которой указан
        /// </summary>
        /// <param name="imgpath">путь к изображению</param>
        /// <returns>слова строкой</returns>
        public string GetWordsByImgFile(string imgpath)
        {
            Upload upl = new Upload();
            string imgurl = upl.GetUrl(imgpath);
            if (imgurl == "") { return ""; }
            string page = GetPageByImageUrl(imgurl);
            if (page == "") { return ""; }
            string wrds = ParsePage(page);
            return wrds;
        }
    }
}
