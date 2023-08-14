using System.Collections.Generic;
using UIAutomationClient;

namespace WebMeetingParticipantChecker.Models.UIAutomation
{
    internal class UserNameElementGetterForZoom : UserNameElementGetter
    {
        /// <summary>
        /// 対象名の分割文字
        /// </summary>
        private readonly char[] ElementTargetNameSplitChars = new char[] { ',' };

        public UserNameElementGetterForZoom(
            CUIAutomation automation, 
            IUIAutomationElement element,
            IKeyEventSender keyEventSender,
            int? keyDonwMaxCount = null)
            : base(automation, element, keyEventSender, keyDonwMaxCount)
        {
        }

        protected override IUIAutomationCondition GetCondition()
        {
            return _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_ListItemControlTypeId);
        }

        protected override UIAutomationElementArray? GetNameElements()
        {
            var items = _targetElement.FindAll(TreeScope.TreeScope_Descendants, GetCondition());
            return new UIAutomationElementArray(items);
        }

        protected override IEnumerable<string> GetSplittedTargetElementName(string elementName)
        {
            return elementName.Split(ElementTargetNameSplitChars);
        }
    }
}
