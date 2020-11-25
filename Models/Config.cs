using System;
using System.Configuration;

namespace AutoSleep.Models
{
    public class Config
    {
        public static TimeSpan SleepTime => TimeSpan.TryParse(ConfigurationManager.AppSettings[nameof(SleepTime)], out var sleepTime)
            ? sleepTime
            : new TimeSpan(23, 0, 0);


        public static readonly string AudioFilePath = ConfigurationManager.AppSettings[nameof(AudioFilePath)];
        public static readonly string SyncTimeApiUrl = ConfigurationManager.AppSettings[nameof(SyncTimeApiUrl)];
    }
}
