using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AutoSleep.Models;
using Newtonsoft.Json;

namespace AutoSleep.Core
{
    public class DateTimeUpdater
    {
        private static readonly HttpClient Spider = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME st);
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }

        public static async Task SyncTimeAsync()
        {
            try
            {
                var timeResp = JsonConvert.DeserializeObject<dynamic>(await (await Spider.GetAsync(Config.SyncTimeApiUrl)).Content.ReadAsStringAsync());

                string offset = timeResp.utc_offset.ToString();
                string uctTimeStr = timeResp.utc_datetime.ToString();
                var localTime = DateTime.Parse(uctTimeStr).Subtract(TimeSpan.Parse(offset.Substring(1)));
                var st = new SYSTEMTIME
                {
                    wYear = (short)localTime.Year,
                    wMonth = (short)localTime.Month,
                    wDay = (short)localTime.Day,
                    wHour = (short)localTime.Hour,
                    wMinute = (short)localTime.Minute,
                    wSecond = (short)localTime.Second
                };
                SetSystemTime(ref st);
            }
            catch
            {
                //联网查询失败时,为降低影响 不更新本地时间
            }
        }
    }
}
