using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using UIAutomationClient;

namespace WebMeetingParticipantChecker.Models.UIAutomation
{
    internal class TeamsAutomationElementChildNameInfoGetter : AutomationElementChildNameInfoGetter
    {
        public TeamsAutomationElementChildNameInfoGetter(CUIAutomation automation, IUIAutomationElement element, int? keyDonwMaxCount = null) : base(automation, element, keyDonwMaxCount)
        {
        }

        protected override IUIAutomationCondition GetConfition()
        {
            return _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_TreeItemControlTypeId);
        }
    }
}
