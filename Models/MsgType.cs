using System.ComponentModel;

namespace AutoSleep.Models
{
    public enum MsgType
    {
        [Description("该睡觉啦~")]
        Notice,
        [Description("半小时后就关机了")]
        Warning,
        [Description("5分钟后就关机啦!")]
        Sleep
    }
}
