using System.Collections.Generic;

namespace BiliCommenter.Models
{
    public class EmojisModel
    {
        public class EmojiPack
        {
            public class Emoji
            {
                public int Id { get; set; }
                public string Name { get; set; }
                public string Url { get; set; }
                public int State { get; set; }
                public string Remark { get; set; }

            }
            public int Pid { get; set; }
            public string Pname { get; set; }
            public int Pstate { get; set; }
            public string Purl { get; set; }
            public List<Emoji> Emojis { get; set; }
        }
        public int Code { get; set; }
        public string Message { get; set; }
        public int Ttl { get; set; }
        public List<EmojiPack> Data;


    }
}
