using UIAutomationClient;
using WebMeetingParticipantChecker.Models.Config;

namespace WebMeetingParticipantChecker.Models.UIAutomation
{
    /// <summary>
    /// teamseの参加者リスト検出用
    /// </summary>
    /// <remarks>
    /// https://docs.microsoft.com/ja-jp/dotnet/framework/ui-automation/subscribe-to-ui-automation-events
    /// https://docs.microsoft.com/ja-jp/windows/win32/winauto/uiauto-eventsforclients
    /// </remarks>
    internal class AutomationElementGetterForTeams : IAutomationElementGetter
    {

        /// <summary>
        /// 対象の要素
        /// </summary>
        private IUIAutomationElement? _targetElement = null;

        /// <summary>
        /// CUIAutomation
        /// </summary>
        private readonly CUIAutomation _automation;


        public AutomationElementGetterForTeams()
        {
            _automation = new CUIAutomation();
        }

        /// <summary>
        /// 対象要素取得
        /// </summary>
        /// <returns></returns>
        public IUIAutomationElement? GetTargetElement()
        {
            return _targetElement;
        }

        /// <summary>
        /// 参加者リスト要素選択
        /// </summary>
        public bool DetectiParticipantElement()
        {
            try
            {
                _targetElement = null;
                var rootElement = _automation.GetRootElement();
                _targetElement = TryGetParticipantElement(rootElement);
                return _targetElement != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 参加者要素取得
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private IUIAutomationElement? TryGetParticipantElement(IUIAutomationElement root)
        {
            var windowCondition = _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_WindowTypePropertyId);
            var rootWindow = TryGetTargetElementForChildren(root, "Zoom ミーティング", windowCondition);
            if (rootWindow == null)
            {
                return null;
            }
            var contentRightPanel = TryGetTargetElementForChildren(rootWindow, "ContentRightPanel", windowCondition);
            if (contentRightPanel == null)
            {
                return null;
            }
            var pListContainer = TryGetTargetElementForChildren(contentRightPanel, "PListContainer", windowCondition);
            if (pListContainer == null)
            {
                return null;
            }
            var participantRoot = TryGetTargetElementForChildren(pListContainer, "参加者", windowCondition);
            if (participantRoot == null)
            {
                return null;
            }
            var listCondition = _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_ListControlTypeId);
            return TryGetTargetElementForChildren(participantRoot, "参加者リスト", listCondition);

        }

        /// <summary>
        /// 子要素から対象要素を取得
        /// </summary>
        /// <param name="root"></param>
        /// <param name="targetName"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        private IUIAutomationElement? TryGetTargetElementForChildren(IUIAutomationElement root, string targetName, IUIAutomationCondition condition)
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
        private bool ContainsTargetName(IUIAutomationElement element, string targetName)
        {
            return element?.CurrentName?.Replace(" ", "")?.ToLower()?.Contains(targetName.Replace(" ", "").ToLower()) == true;
        }
    }
}
