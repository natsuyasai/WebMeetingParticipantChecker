using System;
using System.Threading.Tasks;
using UIAutomationClient;
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
            IUIAutomationCondition WindowCondition() =>
                _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_WindowTypePropertyId);
            IUIAutomationCondition ListCondition() =>
                _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_ListControlTypeId);

            // Zoomミーティングウィンドウ
            var rootWindow = TryGetElementByNames(root, _rootWindowName, _rootWindowNameEn, WindowCondition);
            if (!automationElementGetterUtil.ExistElement(rootWindow))
            {
                // 画面共有中は「Zoomミーティング」では見つからない
                rootWindow = TryGetElementByNames(root, _participantListRootName, _participantListRootNameEn, WindowCondition);
                if (!automationElementGetterUtil.ExistElement(rootWindow))
                {
                    return null;
                }
            }
            // 参加者リスト
            var targetElement = TryGetElementByNames(rootWindow!, _participantListName, _participantListNameEn, ListCondition);

            if (!automationElementGetterUtil.ExistElement(targetElement))
            {
                rootWindow = TryGetElementByNames(root, _participantListRootName, _participantListRootNameEn, WindowCondition);
                if (automationElementGetterUtil.ExistElement(rootWindow))
                {
                    targetElement = TryGetElementByNames(rootWindow!, _participantListName, _participantListNameEn, ListCondition);
                }
            }
            return targetElement;
        }

        /// <summary>
        /// JA・EN名で並列検索し、先に有効な要素を返した結果を採用する
        /// </summary>
        private IUIAutomationElement? TryGetElementByNames(
            IUIAutomationElement root, string name, string nameEn,
            Func<IUIAutomationCondition> conditionFactory)
        {
            var taskJa = Task.Run(() =>
            {
                var util = new AutomationElementGetterUtil();
                return util.TryGetTargetElementForChildren(root, name, conditionFactory());
            });
            var taskEn = Task.Run(() =>
            {
                var util = new AutomationElementGetterUtil();
                return util.TryGetTargetElementForChildren(root, nameEn, conditionFactory());
            });

            while (true)
            {
                var completed = Task.WhenAny(taskJa, taskEn).GetAwaiter().GetResult();
                var result = completed.GetAwaiter().GetResult();
                if (automationElementGetterUtil.ExistElement(result))
                {
                    return result;
                }
                if (taskJa.IsCompleted && taskEn.IsCompleted)
                {
                    return null;
                }
            }
        }
    }
}
