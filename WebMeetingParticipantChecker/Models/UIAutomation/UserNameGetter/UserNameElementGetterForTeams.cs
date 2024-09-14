using System.Collections.Generic;
using UIAutomationClient;
using WebMeetingParticipantChecker.Models.UIAutomation.Define;
using WebMeetingParticipantChecker.Models.UIAutomation.Utils;

namespace WebMeetingParticipantChecker.Models.UIAutomation.UserNameGetter
{
    internal class UserNameElementGetterForTeams : UserNameElementGetter
    {
        public UserNameElementGetterForTeams(
            CUIAutomation automation,
            IUIAutomationElement element,
            IKeyEventSender keyEventSender,
            int keyDonwMaxCount)
            : base(automation, element, keyEventSender, keyDonwMaxCount)
        {
        }

        protected override IUIAutomationCondition GetCondition()
        {
            return _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_TreeItemControlTypeId);
        }

        protected override UIAutomationElementArray? GetNameElements()
        {
            var elements = new List<IUIAutomationElement>();
            var items = _targetElement.FindAll(TreeScope.TreeScope_Descendants, GetCondition());
            for (int i = 0; i < items.Length; i++)
            {
                var item = items.GetElement(i);
                if (item?.CurrentName == null)
                {
                    continue;
                }
                elements.Add(item);
            }
            return new UIAutomationElementArray(elements);
        }

        protected override IEnumerable<string> GetSplittedTargetElementName(string elementName)
        {
            // teamsは空白区切りのため、名前の部分の空白と区別ができない
            return new List<string>() { elementName };
        }

        private IUIAutomationCondition GetConditionForChildren()
        {
            return _automation.CreatePropertyCondition(
                UIAutomationIdDefine.UIA_ControlTypePropertyId,
                UIAutomationIdDefine.UIA_GroupControlTypeId);
        }
    }
}
