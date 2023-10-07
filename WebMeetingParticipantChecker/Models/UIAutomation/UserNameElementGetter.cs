using NLog;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using UIAutomationClient;
using WebMeetingParticipantChecker.Models.Config;
using WebMeetingParticipantChecker.Utils;

namespace WebMeetingParticipantChecker.Models.UIAutomation
{
    /// <summary>
    /// AutomationElementツリー情報取得
    /// </summary>
    internal abstract class UserNameElementGetter : IUserNameElementGetter
    {
        /// <summary>
        /// 捜査対象の要素
        /// </summary>
        protected readonly IUIAutomationElement _targetElement;

        /// <summary>
        /// 取得した名前情報
        /// </summary>
        /// <remarks>
        /// 本来は都度取得のためメンバに保持する必要はないが、
        /// zoomで自動スクロール時に取りこぼす可能性を考慮して、
        /// 一度取得したものは常に保持しておくようにする
        /// </remarks>
#pragma warning disable IDE0044 // 読み取り専用修飾子を追加します
        private Dictionary<string, string> _nameInfos = new();
#pragma warning restore IDE0044 // 読み取り専用修飾子を追加します

        /// <summary>
        /// UIAutomation
        /// </summary>
        protected readonly CUIAutomation _automation;


        private readonly ElementScroller _autoScroll;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UserNameElementGetter(
            CUIAutomation automation,
            IUIAutomationElement element,
            IKeyEventSender keyEventSender,
            int? keyDonwMaxCount = null)
        {
            _automation = automation;
            _targetElement = element;
            _autoScroll = new ElementScroller(keyEventSender, keyDonwMaxCount);
        }

        /// <summary>
        /// 名前情報更新
        /// </summary>
        public IDictionary<string, string> GetNameList(bool isEnableAutoScroll)
        {
            try
            {
                var result = GetAllChildrenName(isEnableAutoScroll);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "監視実行エラー");
                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// condition取得
        /// </summary>
        /// <returns></returns>
        protected abstract IUIAutomationCondition GetCondition();

        /// <summary>
        /// 名前要素リスト取得
        /// </summary>
        /// <returns></returns>
        protected abstract UIAutomationElementArray? GetNameElements();

        /// <summary>
        /// 対象要素の名前の文字列を分割したものを取得
        /// </summary>
        /// <remarks>
        /// Zoomではカンマ区切りでコントロールの名前も取得できてしまうので、適宜分割してもらう
        /// </remarks>
        /// <param name="elementName"></param>
        /// <returns></returns>
        protected abstract IEnumerable<string> GetSplittedTargetElementName(string elementName);

        /// <summary>
        /// 全子要素の名前設定
        /// </summary>
        private Dictionary<string, string> GetAllChildrenName(bool isEnableAutoScroll)
        {
            IUIAutomationElement? beforLastElement = null;
            IUIAutomationElement? firstElement = null;
            var isContinue = true;
            var itemSumCount = 0;
            do
            {
                var elementItems = GetNameElements();
                if (!IsEnableElements(elementItems))
                {
                    break;
                }
                var analysisResult = AnalysisName(firstElement, elementItems);
                // 2週目で同一要素になったので処理終了
                if (analysisResult.isExit)
                {
                    break;
                }
                itemSumCount += elementItems?.Length ?? 0;
                firstElement ??= analysisResult.firstElement;
                if (isEnableAutoScroll)
                {
                    // 前回の末尾の要素と今回の末尾が一致したら終了
                    if (analysisResult.lastElement?.CurrentName == beforLastElement?.CurrentName)
                    {
                        isContinue = false;
                    }
                    else
                    {
                        _autoScroll.MoveSearchPotision(analysisResult.lastElement);
                        beforLastElement = analysisResult.lastElement;
                        // 無限ループになってしまった場合のため一定回数で抜ける
                        isContinue = !_autoScroll.IsOverflowScroll();
                    }
                }
            } while (isContinue && isEnableAutoScroll);

            if (isEnableAutoScroll)
            {
                _autoScroll.ReturnScrollPositionToTop(beforLastElement, itemSumCount);
            }
            return _nameInfos;
        }

        /// <summary>
        /// 名前情報解析
        /// </summary>
        /// <param name="elementItems"></param>
        /// <returns></returns>
        private (IUIAutomationElement? firstElement, IUIAutomationElement? lastElement, bool isExit) AnalysisName(
            IUIAutomationElement? firstElement, UIAutomationElementArray? elementItems)
        {
            IUIAutomationElement? currentFirstElement = null;
            IUIAutomationElement? currentLastElement = null;
            for (int i = 0; i < elementItems?.Length; i++)
            {
                var item = elementItems.GetElement(i);
                // 2週目で同じ要素なら終了
                if (firstElement != null && item.CurrentName == firstElement.CurrentName)
                {
                    return (null, null, true);
                }
                if (item?.CurrentName == null || item.CurrentName == "")
                {
                    continue;
                }
                AddNameInfo(item);
                currentLastElement = item;
                currentFirstElement ??= item;
            }
            return (currentFirstElement, currentLastElement, false);
        }

        /// <summary>
        /// 有効な要素か
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        private bool IsEnableElements(UIAutomationElementArray? elements)
        {
            return (elements != null && elements.Length > 0);
        }

        /// <summary>
        /// 名前情報追加
        /// </summary>
        /// <param name="item"></param>
        private void AddNameInfo(IUIAutomationElement item)
        {
            // 名前の後の「(ホスト,自分)」や操作ボタンの文字もカンマ区切りで取れるため，分割して登録
            // (名前にカンマを入れると，先頭要素だけが名前とは限らなくなるため，一応全て保持)
            foreach (var str in GetSplittedTargetElementName(item.CurrentName))
            {
                var addStr = StringUtils.RemoveSpace(str);
                _nameInfos[addStr] = item.CurrentName;
            }
        }
    }
}
