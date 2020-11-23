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
            Shutdown();
            ProcessProtection.Unprotect();
        }

        void Shutdown()
        {
            Process.Start("shutdown", "/h /f");
        }

        private static readonly List<DateTime> Log = new List<DateTime>();
        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var nowTime = DateTime.Now;
            var currentMinute = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, nowTime.Hour, nowTime.Minute, 0);

            var currentMinuteTimeSpan = currentMinute.TimeOfDay;

            if (currentMinuteTimeSpan == Config.NoticeTime && Log.All(i => i != currentMinute))
            {
                Log.Add(currentMinute);
                Prompter.SendMsg2User(MsgType.Notice);
            }
            if (currentMinuteTimeSpan == Config.WarningTime && Log.All(i => i != currentMinute))
            {
                Log.Add(currentMinute);
                Prompter.SendMsg2User(MsgType.Warning);
            }
            //避免时间设置成了00:00无限关机
            if (currentMinuteTimeSpan >= Config.SleepTime && currentMinuteTimeSpan <= new TimeSpan(6, 0, 0))
            {
                Prompter.SendMsg2User(MsgType.Sleep);
                Shutdown();
            }

        }
    }


}