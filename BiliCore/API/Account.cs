using BiliCommenter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
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
