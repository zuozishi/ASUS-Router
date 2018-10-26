using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AsusRouterApp.Class
{
    public class Http
    {
        public static string Cookie
        {
            get
            {
                return (string)Setting.GetSetting("Cookie", "");
            }
            set
            {
                Setting.SetSetting("Cookie", value);
            }
        }

        public static async Task<string> Get(string url, IEnumerable<KeyValuePair<string, string>> header = null,bool isBrowser = false)
        {
            try
            {
                var handler = new HttpClientHandler() { UseCookies = false };
                HttpClient hc = new HttpClient(handler);
                CacheControlHeaderValue cacheControl = new CacheControlHeaderValue();
                cacheControl.NoCache = true;
                cacheControl.NoStore = true;
                hc.DefaultRequestHeaders.CacheControl = cacheControl;
                if (isBrowser)
                    hc.DefaultRequestHeaders.Add("user-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.63 Safari/537.36 Qiyu/2.1.1.1");
                else
                    hc.DefaultRequestHeaders.Add("user-Agent", "asusrouter-Android-DUTUtil-1.0.0.108");
                hc.DefaultRequestHeaders.Add("Cookie", Cookie);
                if (header != null)
                {
                    foreach (var item in header)
                    {
                        hc.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }
                var res = await hc.GetAsync(url);
                var json = await res.Content.ReadAsStringAsync();
                return json;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static async Task<string> Post(string url,IEnumerable<KeyValuePair<string,string>> data,IEnumerable<KeyValuePair<string,string>> header=null)
        {
            try
            {
                var handler = new HttpClientHandler() { UseCookies = false };
                HttpClient hc = new HttpClient(handler);
                CacheControlHeaderValue cacheControl = new CacheControlHeaderValue();
                cacheControl.NoCache = true;
                cacheControl.NoStore = true;
                hc.DefaultRequestHeaders.CacheControl = cacheControl;
                hc.DefaultRequestHeaders.Add("user-Agent", "asusrouter-Android-DUTUtil-1.0.0.108");
                hc.DefaultRequestHeaders.Add("Cookie",Cookie);
                if (header != null)
                {
                    foreach (var item in header)
                    {
                        hc.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }
                var res= await hc.PostAsync(url, new FormUrlEncodedContent(data));
                var json = await res.Content.ReadAsStringAsync();
                return json;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static async Task<string> Post(string url, string data, IEnumerable<KeyValuePair<string, string>> header = null)
        {
            try
            {
                var handler = new HttpClientHandler() { UseCookies = false };
                HttpClient hc = new HttpClient(handler);
                CacheControlHeaderValue cacheControl = new CacheControlHeaderValue();
                cacheControl.NoCache = true;
                cacheControl.NoStore = true;
                hc.DefaultRequestHeaders.CacheControl = cacheControl;
                hc.DefaultRequestHeaders.Add("user-Agent", "asusrouter-Android-DUTUtil-1.0.0.108");
                hc.DefaultRequestHeaders.Add("Cookie", Cookie);
                if (header != null)
                {
                    foreach (var item in header)
                    {
                        hc.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }
                var byteContent = new ByteArrayContent(Encoding.UTF8.GetBytes(data));
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var res = await hc.PostAsync(url, byteContent);
                var json = await res.Content.ReadAsStringAsync();
                return json;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
