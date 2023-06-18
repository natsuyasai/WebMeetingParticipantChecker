using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIAutomationClient;

namespace WebMeetingParticipantChecker.Models.UIAutomation
{
    internal class ZoomAutomationElementChildNameInfoGetter : AutomationElementChildNameInfoGetter
    {
        /// <summary>
        /// 対象名の分割文字
        /// </summary>
        private readonly char[] ElementTargetNameSplitChars = new char[] { ',' };

        public ZoomAutomationElementChildNameInfoGetter(CUIAutomation automation, IUIAutomationElement element, int? keyDonwMaxCount = null) 
            : base(automation, element, keyDonwMaxCount)
        {
        }

        protected override IUIAutomationCondition GetCondition()
        {
            return _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_ListItemControlTypeId);
        }

        protected override UIAutomationElementArray? GetElementItems()
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
