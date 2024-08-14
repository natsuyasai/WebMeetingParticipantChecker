using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using UIAutomationClient;

namespace WebMeetingParticipantChecker.Models.UIAutomation
{
    internal class AutomationElementGetterUtil
    {
        /// <summary>
        /// CUIAutomation
        /// </summary>
        private readonly CUIAutomation _automation;

        public AutomationElementGetterUtil() 
        {
            _automation = new CUIAutomation();
        }

        /// <summary>
        /// 子要素から対象要素を取得
        /// </summary>
        /// <param name="root"></param>
        /// <param name="targetName"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IUIAutomationElement? TryGetTargetElementForChildren(IUIAutomationElement root, string targetName, IUIAutomationCondition condition)
        {
            var walker = _automation.CreateTreeWalker(condition);
            var filstChild = walker.GetFirstChildElement(root);
            if (filstChild == null)
            {
                return null;
            }
            // 最初の要素が対象要素ならその時点で終了
            if (ContainsTargetName(filstChild, targetName))
            {
                return filstChild;
            }
            // ルートの子要素を順に確認していく
            var target = filstChild;
            IUIAutomationElement child;
            var lastChild = walker.GetLastChildElement(root);
            var count = 0;
            do
            {
                child = walker.GetNextSiblingElement(target);
                if (ContainsTargetName(child, targetName))
                {
                    return child;
                }
                target = child;
                // 無限ループ対策
                // 外的要因を終了条件にしているため、念のため追加
                count++;
                if (count > 1000)
                {
                    break;
                }
            } while (lastChild.GetHashCode() != child?.GetHashCode());

            return null;
        }

        /// <summary>
        /// 対象の要素の名前が含まれているか
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool ContainsTargetName(IUIAutomationElement element, string targetName)
        {
            return element?.CurrentName?.Replace(" ", "")?.ToLower()?.Contains(targetName.Replace(" ", "").ToLower()) == true;
        }
    }
}
