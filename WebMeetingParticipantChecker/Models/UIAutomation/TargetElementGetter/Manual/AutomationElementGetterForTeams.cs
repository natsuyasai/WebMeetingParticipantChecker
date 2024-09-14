﻿using UIAutomationClient;
using WebMeetingParticipantChecker.Models.Config;
using WebMeetingParticipantChecker.Models.UIAutomation.Define;

namespace WebMeetingParticipantChecker.Models.UIAutomation.TargetElementGetter.Manual
{
    internal class AutomationElementGetterForTeams : AutomationElementGetter
    {
        public AutomationElementGetterForTeams(string targetElementName) : base(targetElementName)
        {
        }

        protected override IUIAutomationCondition GetConditon()
        {
            return _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_TreeControlTypeId);
        }
    }
}
