using UIAutomationClient;
using WebMeetingParticipantChecker.Models.Config;

namespace WebMeetingParticipantChecker.Models.UIAutomation
{
    internal class AutomationElementGetterForZoom : AutomationElementGetter
    {
        public AutomationElementGetterForZoom() : base()
        {
        }

        protected override string GetTargetElementName()
        {
            return AppSettingsManager.ZoomParticipantListName;
        }

        protected override IUIAutomationCondition GetConditon()
        {
            return _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_ListControlTypeId);
        }
    }
}
