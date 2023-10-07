using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebMeetingParticipantChecker.Models.UIAutomation
{
    internal enum KeyCode
    {
        Down = 0,
        Up
    }

    internal static class KeyCodeExt
    {
        private static readonly string[] Code =
        {
            "{DOWN}",
            "{UP}"
        };
        public static string GetCodeString(this KeyCode code)
        {
            return Code[(int)code];
        }
    }

    internal interface IKeyEventSender
    {
        public void SendWait(KeyCode code);
    }
}
