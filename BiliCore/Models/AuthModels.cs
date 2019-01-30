using System.Collections.Generic;

namespace BiliCommenter.Models
{
    public class AuthTokenModel
    {
        public int mid { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
    }
    public class AuthModelV2
    {
        public int ts { get; set; }
        public int code { get; set; }
        public AuthTokenModel data { get; set; }
    }
    public class AuthModelV3
    {
        public class AuthModelData
        {
            public class CookieInfo
            {
                public class CookieItem
                {
                    public string name { get; set; }
                    public string value { get; set; }
                    public int http_only { get; set; }
                    public int expires { get; set; }
                }
                public List<CookieItem> cookies { get; set; }
                public List<string> domains { get; set; }
            }
            public int status { get; set; }
            public AuthTokenModel token_info { get; set; }
            public CookieInfo cookie_info { get; set; }
            public List<string> sso { get; set; }
        }
        public int ts { get; set; }
        public int code { get; set; }
        public AuthModelData data { get; set; }
    }
}
