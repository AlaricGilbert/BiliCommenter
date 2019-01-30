using BiliCommenter.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BiliCommenter.API
{
    public static class Common
    {
        public static long TimeSpan
        {
            get { return Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0, 0)).TotalSeconds); }
        }

        public const string AppKey = "1d8b6e7d45233436";
        public const string AppSecret = "560c52ccd288fed045859ed18bffd973";
        public const string Build = "5290000";

        public static string GetSign(string url)
        {
            string result;
            string str = url.Substring(url.IndexOf("?", 4) + 1);
            List<string> list = str.Split('&').ToList();
            list.Sort();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string str1 in list)
            {
                stringBuilder.Append((stringBuilder.Length > 0 ? "&" : string.Empty));
                stringBuilder.Append(str1);
            }
            stringBuilder.Append(AppSecret);
            result = MD5.GetMd5String(stringBuilder.ToString()).ToLower();
            return result;
        }
        public static async Task<EmojisModel> GetEmojisAsync()
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync("https://api.bilibili.com/x/v2/reply/web/emojis");
                var context = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<EmojisModel>(context);
            }
        }
    }
}
