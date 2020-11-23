using System.ComponentModel;

namespace AutoSleep.Models
{
    public enum MsgType
    {
        [Description("该睡觉啦")]
        Notice,
        [Description("再不睡觉就关机了")]
        Warning,
        [Description("Good Night!")]
        Sleep
    }
}
