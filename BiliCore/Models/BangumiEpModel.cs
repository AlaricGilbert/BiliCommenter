using System.Collections.Generic;

namespace BiliCommenter.Models
{
    public class BangumiEpModel
    {
        public class BangumiEp
        {
            public int Avid { get; set; }
            public int Cid { get; set; }
            public int Episode_id { get; set; }
            public string Index { get; set; }

        }

        public int Code { get; set; }
        public string Message { get; set; }
        public List<BangumiEp> Result { get; set; }
    }
}
