using BiliCommenter.API;
using BiliCommenter.Models;
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
        public Action<CommentTask> DeleteTask { get; set; }
        public Noticer Noticer { get; set; }
        public CommentTask(DateTime time, int blid, int taskid, BangumiInfo bi, string message, Action<CommentTask> task)
        {
            Noticer = new Noticer(time, NoticerCallback);
            BangumiListId = blid;
            TaskId = taskid;
            BangumiInfo = bi;
            Message = message;
            DeleteTask = task;
            Noticer.Start();
        }
        public void NoticerCallback(Noticer sender)
        {
            bool running = true;
            while (running)
            {
                var ep = Bangumi.GetBangumiEpAsync(BangumiInfo.SeasonId).Result;
                if (ep.Result.Count == BangumiInfo.Index)
                {
                    var avid = ep.Result[BangumiInfo.Index - 1].Avid;
                    var resu = Comment.SendAsync($"{avid}", Message).Result;
                    Console.WriteLine(resu);
                    running = false;
                }
                Thread.Sleep(20);
            }
            DeleteTask?.Invoke(this);
        }
        public void Dispose()
        {
            Noticer.Dispose();
        }
    }
}
