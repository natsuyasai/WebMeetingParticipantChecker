using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebMeetingParticipantChecker.Models.UIAutomation
{

    internal class ArrowKeyEventSender : IKeyEventSender
    {
        public void SendWait(KeyCode code)
        {
            SendKeys.SendWait(code.GetCodeString());
        }
    }
}
