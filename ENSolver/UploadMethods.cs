// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ENSolver
{
    /// <summary>
    /// методы аплоада картинок для различных хостеров
    /// </summary>
    interface IUploadHoster
    {
        string GetUrl(string path);
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
