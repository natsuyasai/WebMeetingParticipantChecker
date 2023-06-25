using System;
using System.Collections.Generic;
using System.Windows.Forms;
using UIAutomationClient;
using WebMeetingParticipantChecker.Models.Config;
using WebMeetingParticipantChecker.Models.Monitoring;
using WebMeetingParticipantChecker.Utils;
using static WebMeetingParticipantChecker.Models.Monitoring.MonitoringType;

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
        private Dictionary<string, string> _nameInfos = new();

        /// <summary>
        /// UIAutomation
        /// </summary>
        protected readonly CUIAutomation _automation;

        /// <summary>
        /// キーダウンイベントを発生さえる最大回数(1回の更新あたり)
        /// </summary>
        private readonly int KeyDonwMaxCount = 200;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UserNameElementGetter(CUIAutomation automation, IUIAutomationElement element, int? keyDonwMaxCount = null)
        {
            _automation = automation;
            _targetElement = element;
            if (keyDonwMaxCount == null)
            {
                KeyDonwMaxCount = AppSettingsManager.KyedownMaxCount;
            }
            else
            {
                KeyDonwMaxCount = keyDonwMaxCount.Value;
            }
        }

        /// <summary>
        /// 名前情報更新
        /// </summary>
        public IDictionary<string, string> GetNameList(bool isEnableAutoScroll)
        {
            SetAllChildrenName(isEnableAutoScroll);
            return _nameInfos;
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
        private void SetAllChildrenName(bool isEnableAutoScroll)
        {
            try
            {
                string? beforLastElement = null;
                string? firstElement = null;
                var isSearch = true;
                var downCount = 0;
                do
                {
                    var elementItems = GetNameElements();
                    if (elementItems == null)
                    {
                        continue;
                    }
                    IUIAutomationElement? lastElement = null;
                    // ひとまず取れた要素全てチェック
                    for (int i = 0; i < elementItems.Length; i++)
                    {
                        var item = elementItems.GetElement(i);
                        // 2週目で同じ要素なら終了
                        if (firstElement == item?.CurrentName)
                        {
                            isSearch = false;
                            break;
                        }
                        if (item?.CurrentName == null || item.CurrentName == "")
                        {
                            continue;
                        }
                        // 名前の後の「(ホスト,自分)」や操作ボタンの文字もカンマ区切りで取れるため，分割して登録
                        // (名前にカンマを入れると，先頭要素だけが名前とは限らなくなるため，一応全て保持)
                        foreach (var str in GetSplittedTargetElementName(item.CurrentName))
                        {
                            var addStr = StringUtils.RemoveSpace(str);
                            _nameInfos[addStr] = item.CurrentName;
                        }
                        lastElement = item;
                        firstElement ??= item.CurrentName;
                    }
                    // 自動スクロールが許可されていなければ中断
                    if (!isEnableAutoScroll)
                    {
                        break;
                    }
                    if (!isSearch || elementItems.Length == 0)
                    {
                        break;
                    }
                    // 前回の末尾の要素と今回の末尾が一致したら終了
                    if (isSearch && lastElement?.CurrentName == beforLastElement)
                    {
                        break;
                    }
                    if (lastElement != null)
                    {
                        // 最後の要素にフォーカスをあて，↓キー押下イベントを送ることで，スクロールが必要な場合に，移動することで表示させる．
                        beforLastElement = lastElement.CurrentName;
                        if (lastElement.GetCurrentPattern(UIAutomationIdDefine.UIA_SelectionPatternId) is IUIAutomationSelectionItemPattern pattern)
                        {
                            pattern.Select();
                            SendKeys.SendWait("{DOWN}");
                        }
                        downCount++;
                    }
                    // 無限ループになってしまった場合のため，200回で抜ける
                    if (downCount >= KeyDonwMaxCount)
                    {
                        isSearch = false;
                    }
                } while (isSearch);
            }
            catch
            {
                Console.WriteLine("エラー");
            }
            return;
        }
    }
}
