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
        private readonly string _rootWindowNameEn;

        /// <summary>
        /// 参加者リストウィンドウ要素（ポップアウト時）
        /// </summary>
        private readonly string _participantListRootName;
        private readonly string _participantListRootNameEn;

        /// <summary>
        /// 参加者リスト名
        /// </summary>
        private readonly string _participantListName;
        private readonly string _participantListNameEn;


        public AutomationElementGetterForZoom(string rootWindowName, string rootWindowNameEn, string participantListRootName, string participantListRootNameEn, string participantListName, string participantListNameEn)
        {
            _automation = new CUIAutomation();
            _rootWindowName = rootWindowName;
            _rootWindowNameEn = rootWindowNameEn;
            _participantListRootName = participantListRootName;
            _participantListRootNameEn = participantListRootNameEn;
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
            var rootWindow = TryGetElementByNames(root, _rootWindowName, _rootWindowNameEn, windowCondition);
            if (!automationElementGetterUtil.ExistElement(rootWindow))
            {
                // 画面共有中は「Zoomミーティング」では見つからない
                rootWindow = TryGetElementByNames(root, _participantListRootName, _participantListRootNameEn, windowCondition);
                if (!automationElementGetterUtil.ExistElement(rootWindow))
                {
                    return null;
                }
            }
            // 参加者リスト
            var targetElement = TryGetTargetElement(rootWindow!);

            if (!automationElementGetterUtil.ExistElement(targetElement))
            {
                rootWindow = TryGetElementByNames(root, _participantListRootName, _participantListRootNameEn, windowCondition);
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
            return TryGetElementByNames(rootWindow, _participantListName, _participantListNameEn, listCondition);
        }

        private IUIAutomationElement? TryGetElementByNames(IUIAutomationElement root, string name, string nameEn, IUIAutomationCondition condition)
        {
            var element = automationElementGetterUtil.TryGetTargetElementForChildren(root, name, condition);
            if (!automationElementGetterUtil.ExistElement(element))
            {
                element = automationElementGetterUtil.TryGetTargetElementForChildren(root, nameEn, condition);
            }
            return element;
        }
    }
}
