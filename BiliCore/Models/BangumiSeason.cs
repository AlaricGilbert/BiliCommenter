using System.Collections.Generic;

namespace BiliCommenter.Models
{
    public class BangumiSeason
    {
        public class BangumiSeasonData
        {
            public class BangumiInfo
            {
                public string Cover { get; set; }
                public int Delay { get; set; }
                public string Ep_id { get; set; }
                public int Favorites { get; set; }
                public int Follow { get; set; }
                public int Is_published { get; set; }
                public string Pub_index { get; set; }
                public string Pub_time { get; set; }
                public int Pub_ts { get; set; }
                public int Season_id { get; set; }
                public int Season_status { get; set; }
                public string Square_cover { get; set; }
                public string Title { get; set; }
            }
            public string Date { get; set; }
            public int Date_ts { get; set; }
            public int Day_of_week { get; set; }
            public int Is_today { get; set; }
            public List<BangumiInfo> Seasons { get; set; }

        }
        public int Code { get; set; }
        public string Message { get; set; }
        public List<BangumiSeasonData> Result { get; set; }

    }
}
