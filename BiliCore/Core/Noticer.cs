using System;
using System.Timers;

namespace BiliCommenter.Core
{
    public class Noticer:IDisposable
    {
        private DateTime Target { get; set; }
        private double Delta { get { return (Target - DateTime.Now).TotalMilliseconds; } }
        private Timer MainTimer { get; set; }
        private Timer PreciousTimer { get; set; }
        private NoticerElapsedCallback Callback { get; set; }
        public delegate void NoticerElapsedCallback(Noticer sender);
       
        public Noticer(DateTime target, NoticerElapsedCallback callback)
        {
            Target = target;
            Callback = callback;
            if (Delta < 0)
                throw new Exception();
            MainTimer = new Timer(1000);
            MainTimer.Elapsed += CheckTime;
            PreciousTimer = new Timer(20);
            PreciousTimer.Elapsed += PreciousTimer_Elapsed;
        }

        private void PreciousTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Delta <= 10 && Delta >= -40)
            {
                PreciousTimer.Stop();
                Callback(this);
            }
        }

        public void Start() => MainTimer.Start();
        public void Stop() => MainTimer.Stop();
        private void CheckTime(object sender, ElapsedEventArgs e)
        {
            if (Delta < 0)
                throw new Exception();
            if (Delta < 1500) {
                PreciousTimer.Start();
                MainTimer.Stop();
            }
        }

        public void Dispose()
        {
            MainTimer.Dispose();
        }
    }
}
