using BiliCommenter.API;
using BiliCommenter.Models;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace BiliCommenter.Core
{

    public class CommentTask : IDisposable
    {
        public int BangumiListId { get; set; }
        public int TaskId { get; set; }
        public BangumiInfo BangumiInfo { get; set; }
        public string Message { get; set; }
        public Noticer Noticer { get; set; }
        public delegate void CommentTaskFinishedCallback(int taskid);
        [JsonIgnore]
        public CommentTaskFinishedCallback Callback { get; set; }
        public CommentTask(DateTime time, int blid, int taskid, BangumiInfo bi, string message, CommentTaskFinishedCallback callback = null)
        {
            Noticer = new Noticer(time);
            BangumiListId = blid;
            TaskId = taskid;
            BangumiInfo = bi;
            Message = message;
            Callback = callback;
        }
        public void Start() => Noticer.Start(NoticerCallback);
        public void Stop() => Noticer.Stop();
        public void NoticerCallback(Noticer sender)
        {
            bool running = true;
            while (running)
            {
                var ep = Bangumi.GetBangumiEpAsync(BangumiInfo.SeasonId).Result;
                foreach (var item in ep.Result)
                {
                    if (item.Episode_id == BangumiInfo.EpNumber)
                    {
                        Thread.Sleep(800);
                        var resu = Comment.SendAsync($"{item.Avid}", Message).Result;
                        Console.WriteLine(resu);
                        running = false;
                    }
                }
                Thread.Sleep(20);
            }
            Callback(TaskId);
        }
        public void Dispose()
        {
            Noticer.Dispose();
        }
    }
}
