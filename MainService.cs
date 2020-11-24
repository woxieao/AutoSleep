using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
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

        protected override void OnStop()
        {
            ProcessProtection.Unprotect();
            Shutdown();
        }

        void Shutdown()
        {
            Process.Start("shutdown", "/h /f");
        }

        void Trigger()
        {
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

            if (currentMinute == noticeTime && Log.All(i => i != currentMinute))
            {
                Log.Add(currentMinute);
                Prompter.SendMsg2User(MsgType.Notice);
            }
            if (currentMinute == warningTime && Log.All(i => i != currentMinute))
            {
                Log.Add(currentMinute);
                Prompter.SendMsg2User(MsgType.Warning);
            }

            #region 触发关机

            var timeDiff = currentMinute - deadline;
            if (timeDiff <= TimeSpan.FromHours(1) && timeDiff > TimeSpan.FromHours(0))
            {
                Prompter.SendMsg2User(MsgType.Sleep);
                Shutdown();
            }

            #endregion
        }

        private static readonly List<DateTime> Log = new List<DateTime>();
        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Trigger();
        }
    }


}