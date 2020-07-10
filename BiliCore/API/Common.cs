using BiliCommenter.Models;
using BiliCore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        public const string AppKey = "4409e2ce8ffd12b8";
        public const string AppSecret = "59b43e04ad6965f34319062b478f83dd";
        public const string Build = "5370000";



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

        public static async Task<List<CheckedEmojiPack>> CheckCacheAsync(EmojisModel emojis)
        {
            List<CheckedEmojiPack> list = new List<CheckedEmojiPack>();
            for (int i = 0; i < emojis.Data.Count; i++)
            {
                var emojiPack = emojis.Data[i];
                var checkedEmojiPack = new CheckedEmojiPack();

                checkedEmojiPack.PackName = emojiPack.Pname;

                for (int j = 0; j < emojiPack.Emojis.Count; j++)
                {
                    var emoji = emojiPack.Emojis[j];
                    var checkedEmoji = new CheckedEmojiPack.CheckedEmoji();
                    var url = emoji.Url;
                    var fileName = url.Split('/').Last();
                    var filePath = Path.Combine("cache", fileName);
                    if (!File.Exists(filePath))
                    {
                        WebClient wc = new WebClient();
                        await wc.DownloadFileTaskAsync(new Uri(url), filePath);
                    }
                    checkedEmoji.Name = emoji.Name;
                    checkedEmoji.ImagePath = "pack://siteoforigin:,,,/cache/" + fileName;
                    checkedEmojiPack.CheckedEmojis.Add(checkedEmoji);
                }
                list.Add(checkedEmojiPack);
            }
            return list;
        }
    }
}
