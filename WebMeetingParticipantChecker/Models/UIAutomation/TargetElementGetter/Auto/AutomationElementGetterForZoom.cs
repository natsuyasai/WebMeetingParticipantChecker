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
        private readonly string _participantListNameEn;


        public AutomationElementGetterForZoom(string rootWindowName, string participantListRootName, string participantListName, string participantListNameEn)
        {
            _automation = new CUIAutomation();
            _rootWindowName = rootWindowName;
            _participantListRootName = participantListRootName;
            _participantListName = participantListName;
            _participantListNameEn = participantListNameEn;
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
            if (!automationElementGetterUtil.ExistElement(rootWindow))
            {
                // 画面共有中は「Zoomミーティング」では見つからない
                rootWindow = automationElementGetterUtil.TryGetTargetElementForChildren(root, _participantListRootName, windowCondition);
                if (!automationElementGetterUtil.ExistElement(rootWindow))
                {
                    return null;
                }
            }
            // 参加者リスト
            var targetElement = TryGetTargetElement(rootWindow!);

            if (!automationElementGetterUtil.ExistElement(targetElement))
            {
                rootWindow = automationElementGetterUtil.TryGetTargetElementForChildren(root, _participantListRootName, windowCondition);
                if (automationElementGetterUtil.ExistElement(rootWindow))
                {
                    targetElement = TryGetTargetElement(rootWindow!);
                }
            }
            return targetElement;
        }

        private IUIAutomationElement? TryGetTargetElement(IUIAutomationElement rootWindow)
        {
            var listCondition = _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_ListControlTypeId);
            var targetElement = automationElementGetterUtil.TryGetTargetElementForChildren(rootWindow!, _participantListName, listCondition);
            // 参加者リストの要素の名前が環境に応じた言語になっていない可能性を考慮して、英語表記の場合の取得も試みる
            if (!automationElementGetterUtil.ExistElement(targetElement))
            {
                targetElement = automationElementGetterUtil.TryGetTargetElementForChildren(rootWindow!, _participantListNameEn, listCondition);
            }
            return targetElement;
        }
    }
}
