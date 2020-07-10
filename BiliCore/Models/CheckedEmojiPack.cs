using System;
using System.Collections.Generic;
using System.Text;

namespace BiliCore.Models
{

    public class CheckedEmojiPack
    {
        public class CheckedEmoji
        {
            public string Name { get; set; }
            public string ImagePath { get; set; }
        }
        public List<CheckedEmoji> CheckedEmojis = new List<CheckedEmoji>();
        public string PackName { get; set; }
    }
}
