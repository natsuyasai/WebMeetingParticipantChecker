using NLog;
using System;
using UIAutomationClient;

namespace WebMeetingParticipantChecker.Models.UIAutomation.TargetElementGetter.Manual
{
    /// <summary>
    /// AutomationElement取得用クラス
    /// </summary>
    /// <remarks>
    /// https://docs.microsoft.com/ja-jp/dotnet/framework/ui-automation/subscribe-to-ui-automation-events
    /// https://docs.microsoft.com/ja-jp/windows/win32/winauto/uiauto-eventsforclients
    /// </remarks>
    internal abstract class AutomationElementGetter : IAutomationElementGetter
    {
        /// <summary>
        /// 対象の要素
        /// </summary>
        private IUIAutomationElement? _targetElement { get; set; } = null;

        /// <summary>
        /// フォーカスイベントハンドラ
        /// </summary>
        private FocusChangeHandler? _focusHandler = null;


        /// <summary>
        /// 対象の要素検出コールバック
        /// </summary>
        private Action? _onDetectedTargetElemetCallback = null;

        private readonly string _targetElementName;

        /// <summary>
        /// CUIAutomation
        /// </summary>
        protected readonly CUIAutomation _automation;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="targetName"></param>
        public AutomationElementGetter(string targetElementName)
        {
            _automation = new CUIAutomation();
            _targetElementName = targetElementName;
        }

        /// <summary>
        /// 取得対象の要素の名前
        /// </summary>
        /// <returns></returns>
        protected string GetTargetElementName()
            => _targetElementName;

        /// <summary>
        /// 要素取得時のコンディション取得
        /// </summary>
        /// <returns></returns>
        protected abstract IUIAutomationCondition GetConditon();

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// フォーカスイベント購読
        /// </summary>
        public bool DetectiParticipantElement()
        {
            _targetElement = null;
            return true;
        }

        /// <summary>
        /// フォーカスイベント購読破棄
        /// </summary>
        public void UnsubscribeFocusChange()
        {
            if (_focusHandler != null)
            {
                _automation.RemoveFocusChangedEventHandler(_focusHandler);
                _focusHandler = null;
            }
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
        /// フォーカスイベント
        /// </summary>
        private void OnFocusChange(IUIAutomationElement element)
        {
            try
            {
                if (_targetElement != null || element.CurrentName == null)
                {
                    return;
                }
                _logger.Info($"name:[{element.CurrentName}]");
                if (element.CurrentName.Contains(GetTargetElementName()))
                {
                    SetTargetElement(element);
                }
                else
                {
                    // 表示リストの要素が選択状態だと，表示リストへのフォーカスイベントが発生しないため，
                    // 順に親をたどって探す(参加者要素の親の親が参加者リストのため，2つ上までにする)
                    var parent = element;
                    for (int i = 0; i < 5; i++)
                    {
                        var ret = TryGetParentElement(parent);
                        if (ret == null || ret.CurrentName == null)
                        {
                            break;
                        }
                        if (ret.CurrentName.Contains(GetTargetElementName()))
                        {
                            SetTargetElement(ret);
                            break;
                        }
                        parent = ret;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "要素確認失敗");
            }
        }

        /// <summary>
        /// 親要素取得
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private IUIAutomationElement? TryGetParentElement(IUIAutomationElement current)
        {
            var condition = GetConditon();
            var walker = _automation.CreateTreeWalker(condition);
            var parent = walker.GetParentElement(current);
            return parent;
        }

        /// <summary>
        /// 対象要素設定 
        /// </summary>
        /// <param name="element"></param>
        private void SetTargetElement(IUIAutomationElement element)
        {
            _targetElement = element;
            _onDetectedTargetElemetCallback?.Invoke();
            UnsubscribeFocusChange();
        }

        /// <summary>
        /// イベントハンドラ
        /// </summary>
        /// <remarks>
        /// コンストラクタ
        /// </remarks>
        private class FocusChangeHandler(Action<IUIAutomationElement> handler) : IUIAutomationFocusChangedEventHandler
        {
            private readonly Action<IUIAutomationElement> Handler = handler;

            /// <summary>
            /// イベント
            /// </summary>
            /// <param name="sender"></param>
            public void HandleFocusChangedEvent(IUIAutomationElement sender)
            {
                Handler(sender);
            }
        }
    }
}
