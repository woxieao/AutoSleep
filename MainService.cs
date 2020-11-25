using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using AutoSleep.Core;
using AutoSleep.Models;

namespace AutoSleep
{
    public partial class MainService : ServiceBase
    {
        public MainService()
        {
            InitializeComponent();
        }

        private static int _timeElapsed = 0;
        private const int SyncTimeInterval = 5 * 60 * 1000;
        private static int _cancelForceSleepCounter = 10;
        private static readonly List<DateTime> Log = new List<DateTime>();
        protected override void OnStart(string[] args)
        {
            try
            {
                ProcessProtection.Protect();
            }
            catch (Exception e)
            {
                File.WriteAllText($"C:\\AutoSleep.log", e.Message + e.StackTrace);
            }
        }

        protected override void OnShutdown()
        {
            ProcessProtection.Unprotect();
            base.OnShutdown();
        }

        void Sleep()
        {
            Process.Start("shutdown", "/h /f");
        }


        async Task Trigger()
        {
            //在调用api之前看自增,避免多次触发
            _timeElapsed += (int)timer.Interval;
            //第0分钟或者第N分钟时的时候
            if (_timeElapsed % SyncTimeInterval == 0 || _timeElapsed == (int)timer.Interval)
            {
                await DateTimeUpdater.SyncTimeAsync();
            }

            var nowTime = DateTime.Now;
            var currentMinute = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, nowTime.Hour, nowTime.Minute, 0);

            var deadline = DateTime.Today.Add(Config.SleepTime);
            //小于明天6点算作今天
            var nextDayStartTime = new TimeSpan(5, 59, 59);
            if (Config.SleepTime <= nextDayStartTime && nowTime.TimeOfDay > nextDayStartTime)
            {
                deadline = deadline.AddDays(1);
            }

            var noticeTime = deadline.AddMinutes(-60);
            var warningTime = deadline.AddMinutes(-30);
            var sleepTime = deadline.AddMinutes(-5);

            if (currentMinute == noticeTime && Log.All(i => i != currentMinute))
            {
                _cancelForceSleepCounter = 10;
                Log.Add(currentMinute);
                Prompter.SendMsg2User(MsgType.Notice);
            }
            if (currentMinute == warningTime && Log.All(i => i != currentMinute))
            {
                Log.Add(currentMinute);
                Prompter.SendMsg2User(MsgType.Warning);
            }
            if (currentMinute == sleepTime && Log.All(i => i != currentMinute))
            {
                Log.Add(currentMinute);
                Prompter.SendMsg2User(MsgType.Sleep);
            }

            // 触发休眠,2小时之内无法开机,如果次数超过了十次则取消强制休眠,避免休眠之前触发多次
            var timeDiff = currentMinute - deadline;
            if (timeDiff <= TimeSpan.FromHours(2) && timeDiff > TimeSpan.FromHours(0) && --_cancelForceSleepCounter > 0)
            {
                Sleep();
            }
        }

        private async void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            await Trigger();
        }
    }
}