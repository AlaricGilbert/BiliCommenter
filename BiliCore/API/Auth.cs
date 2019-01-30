using BiliCommenter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BiliCommenter.API
{
    public static class Auth
    {
        private static async Task<string> EncryptPassword(string passWord)
        {
            string base64String;
            string url = "https://passport.bilibili.com/login?act=getkey&_=" + Common.TimeSpan;
            using (HttpClient hc = new HttpClient())
            {
                var response = await hc.GetAsync(url);
                var stringAsync = await response.Content.ReadAsStringAsync();
                JObject jObjects = JObject.Parse(stringAsync);
                string str = jObjects["hash"].ToString();
                string str1 = jObjects["key"].ToString();
                string str2 = string.Concat(str, passWord);
                string str3 = Regex.Match(str1, "BEGIN PUBLIC KEY-----(?<key>[\\s\\S]+)-----END PUBLIC KEY").Groups["key"].Value.Trim();
                byte[] numArray = Convert.FromBase64String(str3);
                Asn1Object obj = Asn1Object.FromByteArray(numArray);
                DerSequence publicKeySequence = (DerSequence)obj;

                DerBitString encodedPublicKey = (DerBitString)publicKeySequence[1];
                DerSequence publicKey = (DerSequence)Asn1Object.FromByteArray(encodedPublicKey.GetBytes());

                DerInteger modulus = (DerInteger)publicKey[0];
                DerInteger exponent = (DerInteger)publicKey[1];
                RsaKeyParameters keyParameters = new RsaKeyParameters(false, modulus.PositiveValue, exponent.PositiveValue);
                RSAParameters parameters = DotNetUtilities.ToRSAParameters(keyParameters);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(parameters);
                var result = rsa.Encrypt(Encoding.UTF8.GetBytes(str2), RSAEncryptionPadding.Pkcs1);
                base64String = Convert.ToBase64String(result);
            }
            return base64String;
        }
        public static async Task LoginFromAccessKey(string accesskey)
        {
            Account.AccessKey = accesskey;
            await Account.FreshStatusAsync();
            if (Account.AccessKey != "") await FreshSSO();
        }
        public static async Task LoginV3(string username, string password, string captcha = "")
        {
            string url = "https://passport.bilibili.com/api/v3/oauth2/login";
            string data = $"appkey={Common.AppKey}&build={Common.Build}&mobi_app=android&password={Uri.EscapeDataString(await EncryptPassword(password))}&platform=android&ts={Common.TimeSpan}&username={Uri.EscapeDataString(username)}";
            if (data != "")
            {
                data += "&captcha=" + captcha;
            }
            data += "&sign=" + Common.GetSign(data);
            using (HttpClient hc = new HttpClient())
            {
                hc.DefaultRequestHeaders.Referrer = new Uri("https://www.bilibili.com");
                var response = await hc.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded"));
                var result = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<AuthModelV3>(result);
                Account.AccessKey = model.data.token_info.access_token;
                Account.AuthResultCode = model.code;
                await FreshSSO();
            }
        }
        public static async Task LoginV2(string username, string password, string captcha = "")
        {
            string url = "https://passport.bilibili.com/api/oauth2/login";
            string data = $"appkey={Common.AppKey}&build={Common.Build}&mobi_app=android&password={Uri.EscapeDataString(await EncryptPassword(password))}&platform=android&ts={Common.TimeSpan}&username={Uri.EscapeDataString(username)}";
            if (data != "")
            {
                data += "&captcha=" + captcha;
            }
            data += "&sign=" + Common.GetSign(data);
            using (HttpClient hc = new HttpClient())
            {
                hc.DefaultRequestHeaders.Referrer = new Uri("https://www.bilibili.com");
                var response = await hc.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded"));
                var result = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<AuthModelV2>(result);
                Account.AccessKey = model.data.access_token;
                Account.AuthResultCode = model.code;
                await FreshSSO();
            }
        }
        public static async Task FreshSSO()
        {
            var url = $"https://api.kaaass.net/biliapi/user/sso?access_key={Account.AccessKey}";
            using(HttpClient hc = new HttpClient())
            {
                var response = await hc.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                var obj = JObject.Parse(result);
                Account.CookieString = obj["cookie"].Value<string>();
            }
        }
    }
}
