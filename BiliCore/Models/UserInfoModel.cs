namespace BiliCommenter.Models
{
    public class UserInfoModel
    {
        public class UserInfo
        {
            public class OfficialVerify
            {
                public int role { get; set; }
                public string title { get; set; }
                public string disc { get; set; }
            }
            public class VIPInfo
            {
                public int type { get; set; }
                public int status { get; set; }
            }
            public int mid { get; set; }
            public string name { get; set; }
            public string sex { get; set; }
            public string face { get; set; }
            public string sign { get; set; }
            public int rank { get; set; }
            public int level { get; set; }
            public int jointime { get; set; }
            public int moral { get; set; }
            public int silence { get; set; }
            public double coins { get; set; }
            public bool fans_badge { get; set; }
            public OfficialVerify official { get; set; }
            public VIPInfo vip { get; set; }
            public bool is_followed { get; set; }
            public string top_photo { get; set; }

        }
        public int code { get; set; }
        public string meassage { get; set; }
        public int ttl { get; set; }
        public UserInfo data { get; set; }

    }
}
