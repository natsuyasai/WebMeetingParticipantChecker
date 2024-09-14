using System.DirectoryServices.ActiveDirectory;
using UIAutomationClient;
using WebMeetingParticipantChecker.Models.Config;
using WebMeetingParticipantChecker.Models.UIAutomation.Define;

namespace WebMeetingParticipantChecker.Models.UIAutomation.TargetElementGetter.Auto
{
    /// <summary>
    /// zoomの参加者リスト検出用
    /// </summary>
    /// <remarks>
    /// https://docs.microsoft.com/ja-jp/dotnet/framework/ui-automation/subscribe-to-ui-automation-events
    /// https://docs.microsoft.com/ja-jp/windows/win32/winauto/uiauto-eventsforclients
    /// </remarks>
    internal class AutomationElementGetterForZoom : IAutomationElementGetter
    {
        /// <summary>
        /// 対象の要素
        /// </summary>
        private IUIAutomationElement? _targetElement = null;

        /// <summary>
        /// CUIAutomation
        /// </summary>
        private readonly CUIAutomation _automation;

        /// <summary>
        /// 共通処理
        /// </summary>
        private readonly AutomationElementGetterUtil automationElementGetterUtil = new();

        /// <summary>
        /// ウィンドウのルート要素名
        /// </summary>
        private readonly string _rootWindowName;

        /// <summary>
        /// 参加者リストウィンドウ要素（ポップアウト時）
        /// </summary>
        private readonly string _participantListRootName;

        /// <summary>
        /// 参加者リスト名
        /// </summary>
        private readonly string _participantListName;


        public AutomationElementGetterForZoom(string rootWindowName, string participantListRootName, string participantListName)
        {
            _automation = new CUIAutomation();
            _rootWindowName = rootWindowName;
            _participantListRootName = participantListRootName;
            _participantListName = participantListName;
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
                var result = automationElementGetterUtil.ExistElement(_targetElement);
                return result;
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
            // Zoomミーティングウィンドウ
            var windowCondition = _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_WindowTypePropertyId);
            var rootWindow = automationElementGetterUtil.TryGetTargetElementForChildren(root, _rootWindowName, windowCondition);
            if (rootWindow == null || !automationElementGetterUtil.ExistElement(rootWindow))
            {
                // 画面共有中は「Zoomミーティング」では見つからない
                rootWindow = automationElementGetterUtil.TryGetTargetElementForChildren(root, _participantListRootName, windowCondition);
                if (rootWindow == null || !automationElementGetterUtil.ExistElement(rootWindow))
                {
                    return null;
                }
            }
            // 参加者リスト
            var listCondition = _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_ListControlTypeId);
            var targetElement = automationElementGetterUtil.TryGetTargetElementForChildren(rootWindow, _participantListName, listCondition);

            if (targetElement == null || !automationElementGetterUtil.ExistElement(targetElement))
            {
                rootWindow = automationElementGetterUtil.TryGetTargetElementForChildren(root, _participantListRootName, windowCondition);
                if (rootWindow != null && automationElementGetterUtil.ExistElement(rootWindow))
                {
                    targetElement = automationElementGetterUtil.TryGetTargetElementForChildren(rootWindow, _participantListName, listCondition);
                }
            }
            return targetElement;
        }
    }
}
