using BiliCommenter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BiliCommenter.API
{
    public static class Account
    {
        public static string AccessKey { get; set; }
        public static int AuthResultCode { get; set; }
        public static string CookieString { get; set; }
        public static string CookieJson
        {
            get
            {
                string result_json = $"{{\"{CookieString.TrimEnd(';', ' ')}\"}}";
                result_json = result_json.Replace("=", "\":\"");
                result_json = result_json.Replace("; ", "\",\"");
                return result_json;
            }
        }
        public static JObject CookieJObjet { get { return JObject.Parse(CookieJson); } }
        public static bool OnlineStatus { get; set; }
        public static UserInfoModel.UserInfo UserInfo { get; set; }
        public static async Task FreahUserInfoAsync(){
            var url = $"https://api.bilibili.com/x/space/acc/info?mid={CookieJObjet["DedeUserID"]}";
            
            using (HttpClient hc = new HttpClient())
            {
                var response = await hc.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<UserInfoModel>(result);
                if (model.code == 0)
                    UserInfo = model.data;
                else
                    UserInfo = null;
            }
        }
        public static async Task<string> GetMyInfoJsonNetCoreAsync()
        {
            using (var client = new HttpClient())
            {
                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://api.bilibili.com/x/space/myinfo?jsonp=jsonp"),
                    Headers =
                    {
                        { HttpRequestHeader.Host.ToString(), "api.bilibili.com" },
                        { HttpRequestHeader.Connection.ToString(), "keep-alive" },
                        { HttpRequestHeader.Accept.ToString(), "*/*" },
                        { HttpRequestHeader.UserAgent.ToString(), "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36" },
                        { HttpRequestHeader.ContentType.ToString(), "application/x-www-form-urlencoded" },
                        { HttpRequestHeader.Referer.ToString(),  "https://space.bilibili.com/" + CookieJObjet["DedeUserID"] },
                        { HttpRequestHeader.AcceptEncoding.ToString(), "gzip, deflate, br" },
                        { HttpRequestHeader.AcceptLanguage.ToString(), "zh-CN,zh;q=0.9" },
                        { HttpRequestHeader.Cookie.ToString(), Account.CookieString },
                    }
                };

                var response = client.SendAsync(httpRequestMessage).Result;
                return await response.Content.ReadAsStringAsync();
            }
        }
        public static async Task<string> GetMyStatusJsonNetCoreAsync()
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync($"https://api.bilibili.com/x/relation/stat?vmid={CookieJObjet["DedeUserID"]}").Result;
                return await response.Content.ReadAsStringAsync();
            }
        }
        public static async Task<string> GetMyDynamicHistoryJsonNetCoreAsync()
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync($"https://api.vc.bilibili.com/dynamic_svr/v1/dynamic_svr/space_history?host_uid={CookieJObjet["DedeUserID"]}").Result;
                return await response.Content.ReadAsStringAsync();
            }
        }

        public static async Task FreshStatusAsync()
        {
            var url = $"https://passport.bilibili.com/api/oauth2/info?access_token={AccessKey}&appkey={Common.AppKey}&ts={Common.TimeSpan}";
            url += "&sign=" + Common.GetSign(url);
            using(HttpClient hc = new HttpClient())
            {
                var response = await hc.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                JObject obj = JObject.Parse(result);
                var x = obj["code"];
                if (obj["code"].Value<int>() == 0)
                {
                    OnlineStatus = true;
                }
                else
                {
                    AccessKey = "";
                    AuthResultCode = 0;
                    CookieString = "";
                    OnlineStatus = false;
                }
            }
        }
    }
}
