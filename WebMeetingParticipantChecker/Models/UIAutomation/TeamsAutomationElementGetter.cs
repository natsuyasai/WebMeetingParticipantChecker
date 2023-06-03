using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIAutomationClient;
using WebMeetingParticipantChecker.Models.Monitoring;

namespace WebMeetingParticipantChecker.Models.UIAutomation
{
    internal class TeamsAutomationElementGetter : AutomationElementGetter
    {
        public TeamsAutomationElementGetter() : base()
        {
        }

        protected override string getTargetName()
        {
            return "出席者";
        }

        protected override IUIAutomationCondition getConditon()
        {
            return _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_TreeControlTypeId);
        }
    }
}
