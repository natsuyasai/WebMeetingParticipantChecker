using System.Windows.Automation;
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

        /// <summary>
        /// 共通処理
        /// </summary>
        private readonly AutomationElementGetterUtil automationElementGetterUtil = new();

        /// <summary>
        /// ウィンドウのルート要素名
        /// </summary>
        private readonly string _rootWindowName;

        /// <summary>
        /// 参加者リスト名
        /// </summary>
        private readonly string _participantListName;


        public AutomationElementGetterForTeams(string rootWindowName, string participantListName)
        {
            _automation = new CUIAutomation();
            _rootWindowName = rootWindowName;
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
            if (rootWindow == null)
            {
                return null;
            }
            var treeCondition = _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_TreeControlTypeId);
            return automationElementGetterUtil.TryGetTargetElementForChildren(rootWindow, _participantListName, treeCondition);

        }
    }
}
