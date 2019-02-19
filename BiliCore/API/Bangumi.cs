using BiliCommenter.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BiliCommenter.API
{
    public static class Bangumi
    {
        public static async Task<BangumiSeason> GetBangumiSeasonAsync()
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync("https://bangumi.bilibili.com/web_api/timeline_global");
                var context = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<BangumiSeason>(context);
            }
        }
        public static List<BangumiInfo> GetBangumiInfos(BangumiSeason bseason)
        {
            var result = new List<BangumiInfo>();
            for (int i = 0;i<bseason.Result.Count;i++)
            {
                var daySeason = bseason.Result[i];
                for (int j=0;j<daySeason.Seasons.Count;j++)
                {
                    var season = daySeason.Seasons[j];
                    if (season.Is_published == 1||season.Delay==1)
                        continue;
                    long pubts = season.Pub_ts;
                    pubts *= 10000000;
                    DateTime t = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    t = t.Add(new TimeSpan(pubts)).ToLocalTime();
                    Console.WriteLine(season.Pub_index);
                    var binfo = new BangumiInfo
                    {
                        Cover = season.Cover,
                        SquareCover = season.Square_cover,
                        Index = season.Pub_index.Substring(1, season.Pub_index.Length - 2),
                        EpNumber = Convert.ToInt32(season.Ep_id),
                        SeasonId = season.Season_id,
                        Title = season.Title,
                        UpdateTime = t
                    };
                    result.Add(binfo);
                }
            }
            return result;
        }
        public static async Task<List<BangumiInfo>> GetBangumiInfosAsync() => GetBangumiInfos(await GetBangumiSeasonAsync());
        public static async Task<BangumiEpModel> GetBangumiEpAsync(int season_id)
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync($"https://bangumi.bilibili.com/web_api/get_ep_list?season_id={season_id}&season_type=1");
                var context = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<BangumiEpModel>(context);
            }
        }
    }
}
