using UIAutomationClient;
using WebMeetingParticipantChecker.Models.Config;

namespace WebMeetingParticipantChecker.Models.UIAutomation
{
    internal class AutomationElementGetterForZoom : AutomationElementGetter
    {
        public AutomationElementGetterForZoom(string targetElementName) : base(targetElementName)
        {
        }

        protected override IUIAutomationCondition GetConditon()
        {
            return _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_ListControlTypeId);
        }
    }
}
