using UIAutomationClient;
using WebMeetingParticipantChecker.Models.Config;

namespace WebMeetingParticipantChecker.Models.UIAutomation
{
    internal class AutomationElementGetterForTeams : AutomationElementGetter
    {
        public AutomationElementGetterForTeams() : base()
        {
        }

        protected override string GetTargetElementName()
        {
            return AppSettingsManager.TeamsParticipantListName;
        }

        protected override IUIAutomationCondition GetConditon()
        {
            return _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_TreeControlTypeId);
        }
    }
}
