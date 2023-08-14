using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebMeetingParticipantChecker.Models.UIAutomation
{
    internal class ArrowDownKeyEventSender : IKeyEventSender
    {
        private readonly string ArrowDownCode = "{DOWN}";


        public void SendWait()
        {
            SendKeys.SendWait(ArrowDownCode);
        }
    }
}
