using BiliCommenter.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace BiliCommenter.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists("tasks.json"))
                return;
            var tasks = JsonConvert.DeserializeObject<List<CommentTask>>(File.ReadAllText("tasks.json"));
            foreach (var task in tasks)
            {
                task.Callback = new CommentTask.CommentTaskFinishedCallback((taskid) =>
                {
                    tasks.Remove(task);
                    if (tasks.Count == 0)
                        return;
                });
                task.Start();
            }
        }
    }
}
