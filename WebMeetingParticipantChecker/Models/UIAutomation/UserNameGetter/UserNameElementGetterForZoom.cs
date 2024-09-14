using System.Collections.Generic;
using UIAutomationClient;
using WebMeetingParticipantChecker.Models.UIAutomation.Define;
using WebMeetingParticipantChecker.Models.UIAutomation.Utils;

namespace WebMeetingParticipantChecker.Models.UIAutomation.UserNameGetter
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
            int keyDonwMaxCount)
            : base(automation, element, keyEventSender, keyDonwMaxCount)
        {
        }

        protected override IUIAutomationCondition GetCondition()
        {
            return _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_ListItemControlTypeId);
        }

        protected override UIAutomationElementArray? GetNameElements()
        {
            var items = _targetElement.FindAll(TreeScope.TreeScope_Children, GetCondition());
            return new UIAutomationElementArray(items);
        }

        protected override IEnumerable<string> GetSplittedTargetElementName(string elementName)
        {
            return elementName.Split(ElementTargetNameSplitChars);
        }
    }
}
