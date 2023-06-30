using System.Collections.Generic;
using UIAutomationClient;

namespace WebMeetingParticipantChecker.Models.UIAutomation
{
    internal class UserNameElementGetterForTeams : UserNameElementGetter
    {
        public UserNameElementGetterForTeams(CUIAutomation automation, IUIAutomationElement element, int? keyDonwMaxCount = null)
            : base(automation, element, keyDonwMaxCount)
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
                // 2要素目の子の子が名前の要素のため、そこまでたどる
                var walker = _automation.CreateTreeWalker(GetConditionForChildren());
                var firstChild = walker.GetFirstChildElement(item);
                if (firstChild?.CurrentName == null)
                {
                    continue;
                }
                var secondChild = walker.GetNextSiblingElement(firstChild);
                if (secondChild?.CurrentName == null)
                {
                    continue;
                }
                var secondChildsChild = walker.GetFirstChildElement(secondChild);
                elements.Add(secondChildsChild ?? item);
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
