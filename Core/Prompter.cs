using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using AutoSleep.Models;

namespace AutoSleep.Core
{
    public class Prompter
    {
        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern bool WTSSendMessage(
           IntPtr hServer,
           [MarshalAs(UnmanagedType.I4)] int SessionId,
           String pTitle,
           [MarshalAs(UnmanagedType.U4)] int TitleLength,
           String pMessage,
           [MarshalAs(UnmanagedType.U4)] int MessageLength,
           [MarshalAs(UnmanagedType.U4)] int Style,
           [MarshalAs(UnmanagedType.U4)] int Timeout,
           [MarshalAs(UnmanagedType.U4)] out int pResponse,
           bool bWait);
        static readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;


        private static void Alert(string msg, string title)
        {

            for (var userSession = 10; userSession > 0; userSession--)
            {
                var t = new Thread(() =>
                {
                    try
                    {
                        bool result = false;
                        int tlen = title.Length;
                        int mlen = msg.Length;
                        int resp = 7;
                        result = WTSSendMessage(WTS_CURRENT_SERVER_HANDLE, userSession, title, tlen, msg, mlen, 4, 0, out resp, true);
                        int err = Marshal.GetLastWin32Error();
                        if (err == 0)
                        {
                            if (result) //user responded to box
                            {
                                if (resp == 7) //user clicked no
                                {

                                }
                                else if (resp == 6) //user clicked yes
                                {

                                }
                                Debug.WriteLine("user_session:" + userSession + " err:" + err + " resp:" + resp);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("no such thread exists", ex);
                    }
                });
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
        }



        [DllImport("winmm.dll", EntryPoint = "PlaySound", SetLastError = true, CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]
        static extern bool PlaySound(
            string szSound,
            System.IntPtr hMod,
            PlaySoundFlags flags);

        [System.Flags]
        enum PlaySoundFlags : int
        {
            SND_SYNC = 0x0000,/* play synchronously (default) */
            SND_ASYNC = 0x0001, /* play asynchronously */
            SND_NODEFAULT = 0x0002, /* silence (!default) if sound not found */
            SND_MEMORY = 0x0004, /* pszSound points to a memory file */
            SND_LOOP = 0x0008, /* loop the sound until next sndPlaySound */
            SND_NOSTOP = 0x0010, /* don't stop any currently playing sound */
            SND_NOWAIT = 0x00002000, /* don't wait if the driver is busy */
            SND_ALIAS = 0x00010000,/* name is a registry alias */
            SND_ALIAS_ID = 0x00110000, /* alias is a pre d ID */
            SND_FILENAME = 0x00020000, /* name is file name */
            SND_RESOURCE = 0x00040004, /* name is resource name or atom */
            SND_PURGE = 0x0040,  /* purge non-static events for task */
            SND_APPLICATION = 0x0080, /* look for application specific association */
            SND_SENTRY = 0x00080000, /* Generate a SoundSentry event with this sound */
            SND_RING = 0x00100000, /* Treat this as a "ring" from a communications app - don't duck me */
            SND_SYSTEM = 0x00200000 /* Treat this as a system sound */
        }

        private static void PlaySound(MsgType msgType)
        {
            PlaySound($"{Config.AudioFilePath}\\{(int)msgType}.wav", new IntPtr(), PlaySoundFlags.SND_ASYNC | PlaySoundFlags.SND_SYSTEM);
        }

        public static void SendMsg2User(MsgType msgType)
        {
            Alert(msgType.GetDescription(), "身体重要啊喂~");
            PlaySound(msgType);
        }
    }
}
