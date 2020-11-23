﻿using System;
using System.Configuration;

namespace AutoSleep.Models
{
    public class Config
    {
        public static TimeSpan SleepTime = TimeSpan.TryParse(ConfigurationManager.AppSettings[nameof(SleepTime)], out var sleepTime)
            ? sleepTime
            : new TimeSpan(23, 0, 0);

        public static TimeSpan NoticeTime = SleepTime.Add(new TimeSpan(-1, 0, 0));
        public static TimeSpan WarningTime = SleepTime.Add(new TimeSpan(0, -30, 0));

        public static readonly string ServicesRootPath = ConfigurationManager.AppSettings[nameof(ServicesRootPath)];
    }
}