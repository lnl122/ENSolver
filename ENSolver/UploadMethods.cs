// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ENSolver
{
    /// <summary>
    /// методы аплоада картинок для различных хостеров
    /// 
    /// (!) все вновь созданные методы необходимо включить в конструктор класса Upload (!)
    /// </summary>
    interface IUploadHoster
    {
        string GetUrl(string path);
    }

    /// <summary>
    /// хостинг -                       * реаизован, или причина не реализации
    /// http://ipic.su/                 + реализован
    /// http://pixic.ru/                ? перестал работать в Казахстане
    /// http://ii4.ru/                  + реализован
    /// http://jpegshare.net/           ? реализован - иногда ошибки EnsureSuccessStatusCode();
    /// http://pixshock.net/            + реализован
    /// http://saveimg.ru/              + реализован
    /// http://savepic.ru/              + реализован
    /// http://radikal.ru/              + реализован
    /// http://freeimagehosting.net/    + реализован
    /// 
    /// http://adslclub.ru/             - не нашел ссылок для аплоада
    /// http://habreffect.ru/           - не нашел ссылок для аплоада
    /// http://img.flashtux.org/        - не нашел ссылок для аплоада
    /// http://itrash.ru/               - не нашел ссылок для аплоада
    /// http://ixdrive.co.uk/           - не нашел ссылок для аплоада
    /// http://picamatic.com/           - не нашел ссылок для аплоада
    /// http://pikucha.ru/              - не нашел ссылок для аплоада
    /// http://sharepix.ru/             - не нашел ссылок для аплоада
    /// 
    /// https://hostingkartinok.com/    ? 2do - php, 2 шага, долго. в ответе будет страница, сорержащая ссылку на страчку с ссылкой на картинку
    /// http://fanstudio.ru             - вообще это редактор. нужно два шага - аплоад + получить ссылку
    /// http://imageshost.ru/           - 2do - много скриптов, очень медленный
    /// http://www.fotolink.su/         - полноразмерные картинки не доступны по полученной ссылке для гугля
    /// http://imgdepo.com/             - нужна регистрация или поглубже копать текст скриптов. 3 шага.
    /// http://imgur.com/               - 2do - сложно сделано, несколько шагов (капча, создать альбом, загрузить)
    /// http://Keep4u.Ru/               - 2do - медленный, явно нет формы отправки, нужен токен
    /// http://photoload.ru/            - временно не работает, март 2017
    /// http://picshare.ru/             - 2do - очень медленный
    /// http://simplest-image-hosting.net/- мутный сервис, не отдает прямую ссылку
    /// http://funkyimg.com/            - 2do - токены, ответная страница скриптами
    /// http://vfl.ru/                  - 2do - требуется заранее читать магическое число со страницы, прямую ссылку получать по хэшу этого числа после загрузки отдельно
    /// http://fastpic.ru/              - 2do - есть pop-up реклама, ошибки возврата страницы
    /// http://rghost.ru/               - 2do - нет прямой ссылкиб токены, магикнумберы, (http://polariton.ad-l.ink/6Nl6ljVD7/image.png / http://polariton.ad-l.ink/6Nl6ljVD7/image.png )
    /// http://thumbsnap.com/           - не дает точной прямой ссылки
    /// http://turboimagehost.com/      - медленный, нет прямой ссылки
    /// 
    /// http://10pix.ru/                - уже не хостинг
    /// http://fotometka.ru/            - не открылся
    /// http://tinypic.com/             - нужна капча, много рекламы
    /// http://piccy.info/              - нужна капча
    /// http://ephotobay.com/           - нужна регистрация или капча
    /// http://host.fotki.com/          - нужна регистрация
    /// http://imagehost.spark-media.ru/- нужна регистрация
    /// http://imageshack.us/           - нужна регистрация
    /// http://itmages.ru/              - нужна регистрация
    /// http://imagebar.net/            - умер
    /// http://imagepros.us/            - умер
    /// http://mirfoto.ru/              - умер
    /// http://omploader.org/           - умер
    /// http://picbite.com/             - умер
    /// http://picsa.ru/                - умер
    /// http://picsafe.ru/              - умер
    /// http://pict.com/                - умер
    /// http://theimghost.com/          - умер
    /// http://uaimage.com/             - умер
    /// http://uploadingit.compublic/   - умер
    /// </summary>

    // simplest-image-hosting.net
    public class HosterSimplestImageHostingNet : IUploadHoster
    {
        private Log Log = new Log("SimplestImageHostingNet");
        public string GetUrl(string filepath)
        {
            Log.Write("начали загрузку");
            string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
            string uriaction = "http://simplest-image-hosting.net/";
            HttpClient httpClient = new HttpClient();

            //System.Net.ServicePointManager.Expect100Continue = false;

            MultipartFormDataContent form = new MultipartFormDataContent();

            var a1 = new StringContent(filename); a1.Headers.ContentType = null; form.Add(a1, "\"Filename\"");
            var a2 = new StringContent("file"); a2.Headers.ContentType = null; form.Add(a2, "\"fileSource\"");

            var streamContent2 = new StreamContent(File.Open(filepath, FileMode.Open));
            streamContent2.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            form.Add(streamContent2, "\"fileName\"", filename);

            var a3 = new StringContent("Submit Query"); a3.Headers.ContentType = null; form.Add(a3, "\"Upload\"");

            string sd = "";
            try
            {
                Task<HttpResponseMessage> response = httpClient.PostAsync(uriaction, form);
                HttpResponseMessage res2 = response.Result;
                res2.EnsureSuccessStatusCode();
                HttpContent Cont = res2.Content;
                httpClient.Dispose();
                sd = res2.Content.ReadAsStringAsync().Result;
                Log.Store(sd);
                sd = sd.Substring(sd.IndexOf("Прямая ссылка на изображение"));
                sd = sd.Substring(sd.IndexOf("value=\"") + 7);
                sd = sd.Substring(0, sd.IndexOf("\""));
            }
            catch
            {
                Log.Write("ERROR: не удалось выполнить аплоад картинки ");
                Log.Write(uriaction);
                Log.Write(filepath);
                return "";
            }
            Log.Write("закончили загрузку");
            return sd;
        }

    }

    // imgdepo.com
    public class HosterImgdepoCom : IUploadHoster
    {
        private Log Log = new Log("HosterImgdepoCom");
        public string GetUrl(string filepath)
        {
            Log.Write("начали загрузку");
            string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
            string uriaction = "http://imgdepo.com/upload.php";

            WebClient wc = new WebClient();
            string page_code = wc.DownloadString("http://imgdepo.com");
            wc.Dispose();
            string page_sess_id = page_code;
            string page_upload_session = page_code;
            page_code = page_code.Substring(page_code.IndexOf("name=\"code\""));
            page_code = page_code.Substring(page_code.IndexOf("value=\"") + 7);
            page_code = page_code.Substring(0, page_code.IndexOf("\""));
            page_sess_id = page_sess_id.Substring(page_sess_id.IndexOf("name=\"sess_id\""));
            page_sess_id = page_sess_id.Substring(page_sess_id.IndexOf("value=\"") + 7);
            page_sess_id = page_sess_id.Substring(0, page_sess_id.IndexOf("\""));
            page_upload_session = page_upload_session.Substring(page_upload_session.IndexOf("name=\"sess_id\""));
            page_upload_session = page_upload_session.Substring(page_upload_session.IndexOf("value=\"") + 7);
            page_upload_session = page_upload_session.Substring(0, page_upload_session.IndexOf("\""));

            HttpClient httpClient = new HttpClient();

            //System.Net.ServicePointManager.Expect100Continue = false;

            MultipartFormDataContent form = new MultipartFormDataContent();

            var a1 = new StringContent("1"); a1.Headers.ContentType = null; form.Add(a1, "\"commonupl\"");
            var a2 = new StringContent(page_code); a2.Headers.ContentType = null; form.Add(a2, "\"code\"");
            var a3 = new StringContent("0"); a3.Headers.ContentType = null; form.Add(a3, "\"upload_mode\"");
            var a4 = new StringContent(page_sess_id); a4.Headers.ContentType = null; form.Add(a4, "\"sess_id\"");
            var a5 = new StringContent(page_upload_session); a5.Headers.ContentType = null; form.Add(a5, "\"upload_session\"");
            var a6 = new StringContent("0"); a6.Headers.ContentType = null; form.Add(a6, "\"zipfile\"");
            
            var streamContent2 = new StreamContent(File.Open(filepath, FileMode.Open));
            streamContent2.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            form.Add(streamContent2, "\"uploads_0\"", filename);

            var streamContent3 = new StreamContent(new MemoryStream());
            streamContent2.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            form.Add(streamContent3, "\"uploads_attachId1\"", "");

            string sd = "";
            try
            {
                Task<HttpResponseMessage> response = httpClient.PostAsync(uriaction, form);
                HttpResponseMessage res2 = response.Result;
                res2.EnsureSuccessStatusCode();
                HttpContent Cont = res2.Content;
                httpClient.Dispose();
                sd = res2.Content.ReadAsStringAsync().Result;
                Log.Store(sd);
                sd = sd.Substring(sd.IndexOf("location.href='") + 15);
                sd = sd.Substring(0, sd.IndexOf("'"));

                WebClient wc2 = new WebClient();
                string sd2 = wc2.DownloadString(sd);
                wc2.Dispose();
                sd2 = sd2.Substring(sd.IndexOf("Прямая ссылка"));
                sd2 = sd2.Substring(sd.IndexOf("http://img"));
                sd2 = sd2.Substring(0, sd2.IndexOf("<"));
            }
            catch
            {
                Log.Write("ERROR: не удалось выполнить аплоад картинки ");
                Log.Write(uriaction);
                Log.Write(filepath);
                return "";
            }
            Log.Write("закончили загрузку");
            return sd;
        }

    }

    // freeimagehosting.net
    public class HosterFreeimagehostingNet : IUploadHoster
    {
        private Log Log = new Log("HosterFreeimagehostingNet");
        public string GetUrl(string filepath)
        {
            Log.Write("начали загрузку");
            string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
            string uriaction = "http://www.freeimagehosting.net/upl.php";
            HttpClient httpClient = new HttpClient();

            //System.Net.ServicePointManager.Expect100Continue = false;

            MultipartFormDataContent form = new MultipartFormDataContent();

            var streamContent2 = new StreamContent(File.Open(filepath, FileMode.Open));
            streamContent2.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            form.Add(streamContent2, "\"file\"", filename);

            string sd = "";
            try
            {
                Task<HttpResponseMessage> response = httpClient.PostAsync(uriaction, form);
                HttpResponseMessage res2 = response.Result;
                res2.EnsureSuccessStatusCode();
                HttpContent Cont = res2.Content;
                httpClient.Dispose();
                sd = res2.Content.ReadAsStringAsync().Result;
                Log.Store(sd);
                sd = sd.Substring(sd.IndexOf("Image Code:"));
                sd = sd.Substring(sd.IndexOf("src='") + 5);
                sd = sd.Substring(0, sd.IndexOf("'"));
            }
            catch
            {
                Log.Write("ERROR: не удалось выполнить аплоад картинки ");
                Log.Write(uriaction);
                Log.Write(filepath);
                return "";
            }
            Log.Write("закончили загрузку");
            return sd;
        }

    }

    // radikal.ru
    public class HosterRadikalRu : IUploadHoster
    {
        private Log Log = new Log("HosterRadikalRu");
        public string GetUrl(string filepath)
        {
            Log.Write("начали загрузку");
            string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
            string uriaction = "http://radikal.ru/Img/SaveImg2";
            HttpClient httpClient = new HttpClient();

            //System.Net.ServicePointManager.Expect100Continue = false;

            MultipartFormDataContent form = new MultipartFormDataContent();

            var a1 = new StringContent(filename); a1.Headers.ContentType = null; form.Add(a1, "\"OriginalFileName\"");
            var a2 = new StringContent("960"); a2.Headers.ContentType = null; form.Add(a2, "\"MaxSize\"");
            var a3 = new StringContent("360"); a3.Headers.ContentType = null; form.Add(a3, "\"PrevMaxSize\"");
            var a4 = new StringContent("false"); a4.Headers.ContentType = null; form.Add(a4, "\"IsPublic\"");
            var a5 = new StringContent("false"); a5.Headers.ContentType = null; form.Add(a5, "\"NeedResize\"");
            var a6 = new StringContent("0"); a6.Headers.ContentType = null; form.Add(a6, "\"Rotate\"");
            var a7 = new StringContent("false"); a7.Headers.ContentType = null; form.Add(a7, "\"RotateMetadataRelative\"");

            var streamContent2 = new StreamContent(File.Open(filepath, FileMode.Open));
            streamContent2.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            form.Add(streamContent2, "\"File\"", filename);

            string sd = "";
            try
            {
                Task<HttpResponseMessage> response = httpClient.PostAsync(uriaction, form);
                HttpResponseMessage res2 = response.Result;
                res2.EnsureSuccessStatusCode();
                HttpContent Cont = res2.Content;
                httpClient.Dispose();
                sd = res2.Content.ReadAsStringAsync().Result;
                Log.Store(sd);
                sd = sd.Substring(sd.IndexOf(",\"Url\":\"") + 8);
                sd = sd.Substring(0, sd.IndexOf("\""));
            }
            catch
            {
                Log.Write("ERROR: не удалось выполнить аплоад картинки ");
                Log.Write(uriaction);
                Log.Write(filepath);
                return "";
            }
            Log.Write("закончили загрузку");
            return sd;
        }

    }

    // pixshock.net
    public class HosterPixshockNet : IUploadHoster
    {
        private Log Log = new Log("HosterPixshockNet");
        public string GetUrl(string filepath)
        {
            Log.Write("начали загрузку");
            string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
            string uriaction = "http://www.pixshock.net/upfileim.html";
            HttpClient httpClient = new HttpClient();

            System.Net.ServicePointManager.Expect100Continue = false;

            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new StringContent("10000000", System.Text.Encoding.UTF8, null), "MAX_FILE_SIZE");

            var streamContent2 = new StreamContent(File.Open(filepath, FileMode.Open));
            streamContent2.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            form.Add(streamContent2, "apic1", filename);

            form.Add(new StringContent("", System.Text.Encoding.UTF8, null), "title1");
            form.Add(new StringContent("640", System.Text.Encoding.UTF8, null), "smallsize1");
            form.Add(new StringContent("150", System.Text.Encoding.UTF8, null), "size1");

            string sd = "";
            try
            {
                Task<HttpResponseMessage> response = httpClient.PostAsync(uriaction, form);
                HttpResponseMessage res2 = response.Result;
                res2.EnsureSuccessStatusCode();
                HttpContent Cont = res2.Content;
                httpClient.Dispose();
                sd = res2.Content.ReadAsStringAsync().Result;
                Log.Store(sd);
                sd = sd.Substring(sd.IndexOf("URL "));
                sd = sd.Substring(sd.IndexOf("value='") + 7);
                sd = sd.Substring(0, sd.IndexOf("'"));
            }
            catch
            {
                Log.Write("ERROR: не удалось выполнить аплоад картинки ");
                Log.Write(uriaction);
                Log.Write(filepath);
                return "";
            }
            Log.Write("закончили загрузку");
            return sd;
        }

    }

    // hostingkartinok.com
    public class HosterHostingkartinokCom : IUploadHoster
    {
        private Log Log = new Log("HosterHostingkartinokCom");
        public string GetUrl(string filepath)
        {
            Log.Write("начали загрузку");
            string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
            string uriaction = "http://hostingkartinok.com/process.php";
            HttpClient httpClient = new HttpClient();

            //System.Net.ServicePointManager.Expect100Continue = false;

            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new StringContent("0"), "watermark");
            form.Add(new StringContent("0"), "tojpeg");
            form.Add(new StringContent("70"), "jpeg_quality"); // ?? 0/1 yes/no
            form.Add(new StringContent("500"), "resize_to");
            form.Add(new StringContent("standard"), "upload_type");

            var streamContent2 = new StreamContent(File.Open(filepath, FileMode.Open));
            form.Add(streamContent2, "image_1", filename);
            string sd = "";
            try
            {
                Task<HttpResponseMessage> response = httpClient.PostAsync(uriaction, form);
                HttpResponseMessage res2 = response.Result;
                res2.EnsureSuccessStatusCode();
                HttpContent Cont = res2.Content;
                httpClient.Dispose();
                sd = res2.Content.ReadAsStringAsync().Result;
                Log.Store(sd);
                sd = sd.Substring(sd.IndexOf("[img]") + 5);
                sd = sd.Substring(0, sd.IndexOf("[/img]"));
            }
            catch
            {
                Log.Write("ERROR: не удалось выполнить аплоад картинки ");
                Log.Write(uriaction);
                Log.Write(filepath);
                return "";
            }
            Log.Write("закончили загрузку");
            return sd;
        }

    }

    // fastpic.ru
    public class HosterFastpicRu : IUploadHoster
    {
        private Log Log = new Log("HosterFastpicRu");
        public string GetUrl(string filepath)
        {
            Log.Write("начали загрузку");
            string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
            string uriaction = "http://fastpic.ru/uploadmulti/";
            HttpClient httpClient = new HttpClient();

            //System.Net.ServicePointManager.Expect100Continue = false;

            MultipartFormDataContent form = new MultipartFormDataContent();

            var streamContent2 = new StreamContent(File.Open(filepath, FileMode.Open));
            streamContent2.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            form.Add(streamContent2, "\"file[]\"", filename);
            
            var a1 = new StringContent("1"); a1.Headers.ContentType = null; form.Add(a1, "\"thumb_text\"");
            var a2 = new StringContent("no"); a2.Headers.ContentType = null; form.Add(a2, "\"check_thumb\"");
            var a3 = new StringContent("170"); a3.Headers.ContentType = null; form.Add(a3, "\"thumb_size\"");
            var a4 = new StringContent("500"); a4.Headers.ContentType = null; form.Add(a4, "\"res_select\"");
            var a5 = new StringContent("500"); a5.Headers.ContentType = null; form.Add(a5, "\"orig_resize\"");
            var a6 = new StringContent("0"); a6.Headers.ContentType = null; form.Add(a6, "\"orig_rotate\"");
            var a7 = new StringContent("75"); a7.Headers.ContentType = null; form.Add(a7, "\"jpeg_quality\"");
            var a8 = new StringContent("Загрузить"); a8.Headers.ContentType = null; form.Add(a8, "\"submit\"");
            var a9 = new StringContent("1"); a9.Headers.ContentType = null; form.Add(a9, "\"uploading\"");

            string sd = "";
            try
            {
                Task<HttpResponseMessage> response = httpClient.PostAsync(uriaction, form);
                HttpResponseMessage res2 = response.Result;
                //res2.EnsureSuccessStatusCode();
                HttpContent Cont = res2.Content;
                httpClient.Dispose();
                sd = res2.Content.ReadAsStringAsync().Result;
                Log.Store(sd);
                sd = sd.Substring(sd.IndexOf("HTML:"));
                sd = sd.Substring(sd.IndexOf("src=\"") + 5);
                sd = sd.Substring(0, sd.IndexOf("\""));
            }
            catch
            {
                Log.Write("ERROR: не удалось выполнить аплоад картинки ");
                Log.Write(uriaction);
                Log.Write(filepath);
                return "";
            }
            Log.Write("закончили загрузку");
            return sd;
        }

    }

    // saveimg.ru
    public class HosterSaveimgRu : IUploadHoster
    {
        private Log Log = new Log("HosterSaveimgRu");
        public string GetUrl(string filepath)
        {
            Log.Write("начали загрузку");
            string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
            string uriaction = "http://saveimg.ru/process.php";
            HttpClient httpClient = new HttpClient();

            //System.Net.ServicePointManager.Expect100Continue = false;

            MultipartFormDataContent form = new MultipartFormDataContent();

            var streamContent2 = new StreamContent(File.Open(filepath, FileMode.Open));
            streamContent2.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            form.Add(streamContent2, "image1", filename);

            var a1 = new StringContent("1", System.Text.Encoding.UTF8, null);
            a1.Headers.ContentType = null;
            form.Add(a1, "tags1");
            var a2 = new StringContent("0°", System.Text.Encoding.UTF8);
            a2.Headers.ContentType = null;
            form.Add(a2, "rotate_angle");
            var a3 = new StringContent("false", System.Text.Encoding.UTF8, null);
            a3.Headers.ContentType = null;
            form.Add(a3, "grayscale_mode");
            var a4 = new StringContent("false", System.Text.Encoding.UTF8, null);
            a4.Headers.ContentType = null;
            form.Add(a4, "rating_mode");


            string sd = "";
            try
            {
                Task<HttpResponseMessage> response = httpClient.PostAsync(uriaction, form);
                HttpResponseMessage res2 = response.Result;
                res2.EnsureSuccessStatusCode();
                HttpContent Cont = res2.Content;
                httpClient.Dispose();
                sd = res2.Content.ReadAsStringAsync().Result;
                Log.Store(sd);
                sd = sd.Substring(sd.IndexOf("(HTML)"));
                sd = sd.Substring(sd.IndexOf("src='") + 5);
                sd = sd.Substring(0, sd.IndexOf("'"));
            }
            catch
            {
                Log.Write("ERROR: не удалось выполнить аплоад картинки ");
                Log.Write(uriaction);
                Log.Write(filepath);
                return "";
            }
            Log.Write("закончили загрузку");
            return sd;
        }

    }

    // savepic.ru
    public class HosterSavepicRu : IUploadHoster
    {
        private Log Log = new Log("HosterSavepicRu");
        public string GetUrl(string filepath)
        {
            Log.Write("начали загрузку");
            string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
            string uriaction = "http://savepic.ru/index.php";
            HttpClient httpClient = new HttpClient();

            System.Net.ServicePointManager.Expect100Continue = false;

            MultipartFormDataContent form = new MultipartFormDataContent();
            var streamContent2 = new StreamContent(File.Open(filepath, FileMode.Open));
            form.Add(streamContent2, "file", filename);

            var a1 = new StringContent(""); a1.Headers.ContentType = null; form.Add(a1, "note");
            var a2 = new StringContent("decor"); a2.Headers.ContentType = null; form.Add(a2, "font1");
            var a3 = new StringContent("20"); a3.Headers.ContentType = null; form.Add(a3, "font2");
            var a4 = new StringContent("h"); a4.Headers.ContentType = null; form.Add(a4, "orient");
            var a5 = new StringContent("x"); a5.Headers.ContentType = null; form.Add(a5, "size1");
            var a6 = new StringContent("1024x768"); a6.Headers.ContentType = null; form.Add(a6, "size2");
            var a7 = new StringContent("00"); a7.Headers.ContentType = null; form.Add(a7, "rotate");
            var a8 = new StringContent("0"); a8.Headers.ContentType = null; form.Add(a8, "flip");
            var a9 = new StringContent("300x225"); a9.Headers.ContentType = null; form.Add(a9, "mini");
            var a10 = new StringContent("zoom"); a10.Headers.ContentType = null; form.Add(a10, "opt3[]");
            var a11 = new StringContent(""); a11.Headers.ContentType = null; form.Add(a11, "email");

            string sd = "";
            try
            {
                Task<HttpResponseMessage> response = httpClient.PostAsync(uriaction, form);
                HttpResponseMessage res2 = response.Result;
                res2.EnsureSuccessStatusCode();
                HttpContent Cont = res2.Content;
                httpClient.Dispose();
                sd = res2.Content.ReadAsStringAsync().Result;
                Log.Store(sd);
                sd = sd.Substring(sd.IndexOf("Прямая ссылка на изображение"));
                sd = sd.Substring(sd.IndexOf("value=\"") + 7);
                sd = sd.Substring(0, sd.IndexOf("\""));
            }
            catch
            {
                Log.Write("ERROR: не удалось выполнить аплоад картинки ");
                Log.Write(uriaction);
                Log.Write(filepath);
                return "";
            }
            Log.Write("закончили загрузку");
            return sd;
        }

    }

    // vfl.ru
    public class HosterVflRu : IUploadHoster
    {
        private Log Log = new Log("HosterVflRu");
        public static string UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1";

        public string GetUrl(string filepath)
        {
            Log.Write("начали загрузку");
            string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
            string uriaction = "http://storage.vfl.ru/index.sema";
            HttpClient httpClient = new HttpClient();
            /*
            WebClient wc = new WebClient();
            wc.Encoding = System.Text.Encoding.UTF8;
            wc.Headers.Add("User-Agent", UserAgent);
            wc.Headers.Add("Accept-Language", "ru-ru");
            wc.Headers.Add("Content-Language", "ru-ru");
            string img_id = wc.DownloadString("http://vfl.ru/");
            img_id = img_id.Substring(img_id.IndexOf("name=\"img_id\"") + 13);
            img_id = img_id.Substring(img_id.IndexOf("value=\"") + 7);
            img_id = img_id.Substring(0, img_id.IndexOf("\""));
            */
            //System.Net.ServicePointManager.Expect100Continue = false;

            MultipartFormDataContent form = new MultipartFormDataContent();

            var a1 = new StringContent("fotos"); a1.Headers.ContentType = null; form.Add(a1, "\"a\"");
            var a2 = new StringContent("upload"); a2.Headers.ContentType = null; form.Add(a2, "\"sa\"");
            var a3 = new StringContent("1"); a3.Headers.ContentType = null; form.Add(a3, "\"img_id\"");

            var streamContent1 = new StreamContent(new MemoryStream());
            streamContent1.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            form.Add(streamContent1, "upload_file2");
            var b1 = new StringContent(""); b1.Headers.ContentType = null; form.Add(b1, "\"upload_nazv2\"");
            var b2 = new StringContent(""); b2.Headers.ContentType = null; form.Add(b2, "\"inet_foto\"");
            var b3 = new StringContent(""); b3.Headers.ContentType = null; form.Add(b3, "\"inet_foto_nazv\"");

            var streamContent2 = new StreamContent(File.Open(filepath, FileMode.Open));
            streamContent2.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            form.Add(streamContent2, "upload_file3", filename);
            var a4 = new StringContent(""); a4.Headers.ContentType = null; form.Add(a4, "\"upload_nazv3\"");

            var a5 = new StringContent("5"); a5.Headers.ContentType = null; form.Add(a5, "\"resize2\"");
            var a6 = new StringContent("90"); a6.Headers.ContentType = null; form.Add(a6, "\"rotate2\"");
            var a7 = new StringContent("640"); a7.Headers.ContentType = null; form.Add(a7, "\"preview_x\"");
            var a8 = new StringContent("480"); a8.Headers.ContentType = null; form.Add(a8, "\"preview_y\"");
            var a9 = new StringContent("1"); a9.Headers.ContentType = null; form.Add(a9, "\"dont_show\"");
            var a10 = new StringContent("1"); a10.Headers.ContentType = null; form.Add(a10, "\"del\"");
            var a11 = new StringContent("24"); a11.Headers.ContentType = null; form.Add(a11, "\"del2\"");
            var a12 = new StringContent(""); a12.Headers.ContentType = null; form.Add(a12, "\"tags\"");

            string sd = "";
            try
            {
                Task<HttpResponseMessage> response = httpClient.PostAsync(uriaction, form);
                HttpResponseMessage res2 = response.Result;
                res2.EnsureSuccessStatusCode();
                HttpContent Cont = res2.Content;
                httpClient.Dispose();
                sd = res2.Content.ReadAsStringAsync().Result;
                Log.Store(sd);
                sd = sd.Substring(sd.IndexOf("Прямая ссылка на изображение:"));
                sd = sd.Substring(sd.IndexOf("http://"));
                sd = sd.Substring(0, sd.IndexOf("<"));
            }
            catch
            {
                Log.Write("ERROR: не удалось выполнить аплоад картинки ");
                Log.Write(uriaction);
                Log.Write(filepath);
                return "";
            }
            Log.Write("закончили загрузку");
            return sd;
        }
    }

    // jpegshare.net
    public class HosterJpegshareNet : IUploadHoster
    {
        private Log Log = new Log("HosterJpegshareNet");
        public string GetUrl(string filepath)
        {
            string res = GetUrl2(filepath);
            Log.Write(res);
            if (res != "") { return res; }
            res = GetUrl2(filepath);
            Log.Write(res);
            return res;
        }

        private string GetUrl2(string filepath)
        {
            Log.Write("начали загрузку");
            string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
            string uriaction = "http://jpegshare.net/upload.php";
            HttpClient httpClient = new HttpClient();

            System.Net.ServicePointManager.Expect100Continue = false;

            MultipartFormDataContent form = new MultipartFormDataContent();

            var streamContent2 = new StreamContent(File.Open(filepath, FileMode.Open));
            streamContent2.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            form.Add(streamContent2, "imgfile", filename);
            string sd = "";
            try
            {
                Task<HttpResponseMessage> response = httpClient.PostAsync(uriaction, form);
                HttpResponseMessage res2 = response.Result;
                //res2.EnsureSuccessStatusCode();
                HttpContent Cont = res2.Content;
                httpClient.Dispose();
                sd = res2.Content.ReadAsStringAsync().Result;
                Log.Store(sd);
                sd = sd.Substring(sd.IndexOf("<b>BBCODE</b>:") + 14);
                sd = sd.Substring(sd.IndexOf("[img]") + 5);
                sd = sd.Substring(0, sd.IndexOf("[/img]"));
            }
            catch
            {
                Log.Write("ERROR: не удалось выполнить аплоад картинки ");
                Log.Write(uriaction);
                Log.Write(filepath);
                return "";
            }
            Log.Write("закончили загрузку");
            return sd;
        }

    }

    // ii4.ru
    public class HosterIi4Ru : IUploadHoster
    {
        private Log Log = new Log("HosterIi4Ru");
        public string GetUrl(string filepath)
        {
            Log.Write("начали загрузку");
            string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
            string uriaction = "http://ii4.ru/";
            HttpClient httpClient = new HttpClient();

            //System.Net.ServicePointManager.Expect100Continue = false;

            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new StringContent("1"), "category");
            form.Add(new StringContent(""), "comment");
            form.Add(new StringContent("no"), "showingallery"); // ?? 0/1 yes/no
            //form.Add(new StringContent("/"), "link");
            form.Add(new StringContent(""), "action");

            var streamContent2 = new StreamContent(File.Open(filepath, FileMode.Open));
            form.Add(streamContent2, "file", filename);
            string sd = "";
            try
            {
                Task<HttpResponseMessage> response = httpClient.PostAsync(uriaction, form);
                HttpResponseMessage res2 = response.Result;
                res2.EnsureSuccessStatusCode();
                HttpContent Cont = res2.Content;
                httpClient.Dispose();
                sd = res2.Content.ReadAsStringAsync().Result;
                Log.Store(sd);
                sd = sd.Substring(sd.IndexOf("[img]") + 5);
                sd = sd.Substring(0, sd.IndexOf("[/img]"));
            }
            catch
            {
                Log.Write("ERROR: не удалось выполнить аплоад картинки ");
                Log.Write(uriaction);
                Log.Write(filepath);
                return "";
            }
            Log.Write("закончили загрузку");
            return sd;
        }

    }

    // pixic.ru
    public class HosterPixicRu : IUploadHoster
    {
        private Log Log = new Log("HosterPixicRu");
        public string GetUrl(string filepath)
        {
            Log.Write("начали загрузку");
            string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
            string uriaction = "http://www.pixic.ru/";
            HttpClient httpClient = new HttpClient();

            //System.Net.ServicePointManager.Expect100Continue = false;

            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new StringContent("1"), "send");
            var streamContent2 = new StreamContent(File.Open(filepath, FileMode.Open));
            form.Add(streamContent2, "file1", filename);
            string sd = "";
            try
            {
                Task<HttpResponseMessage> response = httpClient.PostAsync(uriaction, form);
                HttpResponseMessage res2 = response.Result;
                res2.EnsureSuccessStatusCode();
                HttpContent Cont = res2.Content;
                httpClient.Dispose();
                sd = res2.Content.ReadAsStringAsync().Result;
                Log.Store(sd);
                sd = sd.Substring(sd.IndexOf("large_input") + ("large_input").Length);
                sd = sd.Substring(sd.IndexOf("value='") + 7);
                sd = sd.Substring(0, sd.IndexOf("'"));
            }
            catch
            {
                Log.Write("ERROR: не удалось выполнить аплоад картинки ");
                Log.Write(uriaction);
                Log.Write(filepath);
                return "";
            }
            Log.Write("закончили загрузку");
            return sd;
        }
    }

    // ipic.su
    public class HosterIpicSu : IUploadHoster
    {
        private Log Log = new Log("HosterIpicSu");
        public string GetUrl(string filepath)
        {
            Log.Write("начали загрузку");
            string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
            string uriaction = "http://ipic.su/";
            HttpClient httpClient = new HttpClient();

            //System.Net.ServicePointManager.Expect100Continue = false;

            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new StringContent("/"), "link");
            form.Add(new StringContent("loadimg"), "action");
            form.Add(new StringContent("ipic.su"), "client");
            var streamContent2 = new StreamContent(File.Open(filepath, FileMode.Open));
            form.Add(streamContent2, "image", filename);
            string sd = "";
            try
            {
                Task<HttpResponseMessage> response = httpClient.PostAsync(uriaction, form);
                HttpResponseMessage res2 = response.Result;
                res2.EnsureSuccessStatusCode();
                HttpContent Cont = res2.Content;
                httpClient.Dispose();
                sd = res2.Content.ReadAsStringAsync().Result;
                Log.Store(sd);
                sd = sd.Substring(sd.IndexOf("[edit]") + 6);
                sd = sd.Substring(sd.IndexOf("value=\"") + 7);
                sd = sd.Substring(0, sd.IndexOf("\""));
            }
            catch
            {
                Log.Write("ERROR: не удалось выполнить аплоад картинки ");
                Log.Write(uriaction);
                Log.Write(filepath);
                return "";
            }
            Log.Write("закончили загрузку");
            return sd;
        }

    }
}
