using System.Windows.Automation;
using UIAutomationClient;
using WebMeetingParticipantChecker.Models.Config;
using WebMeetingParticipantChecker.Models.UIAutomation.Define;

namespace WebMeetingParticipantChecker.Models.UIAutomation.TargetElementGetter.Auto
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
        /// 参加者リスト名
        /// </summary>
        private readonly string _participantListName;
        private readonly string _participantListNameEn;


        public AutomationElementGetterForTeams(string rootWindowName, string rootWindowNameEn, string participantListName, string participantListNameEn)
        {
            _automation = new CUIAutomation();
            _rootWindowName = rootWindowName;
            _rootWindowNameEn = rootWindowNameEn;
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
            var rootWindow = automationElementGetterUtil.TryGetTargetElementForChildren(root, _rootWindowName, windowCondition);
            if (!automationElementGetterUtil.ExistElement(rootWindow))
            {
                rootWindow = automationElementGetterUtil.TryGetTargetElementForChildren(root, _rootWindowNameEn, windowCondition);
                if (!automationElementGetterUtil.ExistElement(rootWindow))
                {
                    return null;
                }
            }
            var treeCondition = _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_TreeControlTypeId);
            var targetElement = automationElementGetterUtil.TryGetTargetElementForChildren(rootWindow!, _participantListName, treeCondition);
            if (!automationElementGetterUtil.ExistElement(targetElement))
            {
                targetElement = automationElementGetterUtil.TryGetTargetElementForChildren(rootWindow!, _participantListNameEn, treeCondition);
            }
            return targetElement;
        }
    }
}
