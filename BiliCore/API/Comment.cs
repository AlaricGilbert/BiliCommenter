using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BiliCommenter.API
{
    public static class Comment
    {
        public static async Task<string> SendAsyncNetCore(string av_number, string comment)
        {
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("oid", av_number),
                new KeyValuePair<string, string>("type", "1"),
                new KeyValuePair<string, string>("message", comment),
                new KeyValuePair<string, string>("plat", "1"),
                new KeyValuePair<string, string>("jsonp", "jsonp"),
                new KeyValuePair<string, string>("csrf", Account.CookieJObjet["bili_jct"].Value<string>())
            };
            using (var client = new HttpClient())
            {
                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://api.bilibili.com/x/v2/reply/add"),
                    Headers =
                    {
                        { HttpRequestHeader.Host.ToString(), "api.bilibili.com" },
                        { HttpRequestHeader.Connection.ToString(), "keep-alive" },
                        { HttpRequestHeader.Accept.ToString(), "application/json, text/javascript, */*; q=0.01" },
                        { "Origin", "https://www.bilibili.com" },
                        { HttpRequestHeader.UserAgent.ToString(), "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36" },
                        { HttpRequestHeader.ContentType.ToString(), "application/x-www-form-urlencoded" },
                        { HttpRequestHeader.Referer.ToString(), "https://www.bilibili.com/video/av"+av_number },
                        { HttpRequestHeader.AcceptEncoding.ToString(), "gzip, deflate, br" },
                        { HttpRequestHeader.AcceptLanguage.ToString(), "zh-CN,zh;q=0.9" },
                        { HttpRequestHeader.Cookie.ToString(), Account.CookieString },
                    },
                    Content = new FormUrlEncodedContent(pairs)
                };

                var response = client.SendAsync(httpRequestMessage).Result;
                return await response.Content.ReadAsStringAsync();
            }
        }
        public static async Task<string> SendAsync(string av_number, string comment)
        {
            var req = WebRequest.CreateHttp($"https://api.bilibili.com/x/v2/reply/add?oid={av_number}&type=1&message={comment}&plat=1&jsonp=jsonp&csrf={Account.CookieJObjet["bili_jct"].Value<string>()}");
            req.Method = "POST";
            req.Host = "api.bilibili.com";
            //req.Connection = "keep-alive";
            req.Accept = "application/json, text/javascript, */*; q=0.01";
            req.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Referer = "https://www.bilibili.com/video/av" + av_number;
            req.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            req.Headers.Add("Cookie", Account.CookieString);
            var response = await req.GetResponseAsync();
            var r_stream = response.GetResponseStream();
            byte[] buffer = new byte[response.ContentLength];
            r_stream.ReadAsync(buffer, 0, (int)response.ContentLength).Wait();
            return Encoding.UTF8.GetString(buffer);
        }
    }
}
