using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;
using UIAutomationClient;
using WebMeetingParticipantChecker.Models.Config;
using WebMeetingParticipantChecker.Models.UIAutomation.Define;

namespace WebMeetingParticipantChecker.Models.UIAutomation.Utils
{
    internal class ElementScroller
    {
        private int _keyDownCount = 0;

        /// <summary>
        /// キーダウンイベントを発生さえる最大回数(1回の更新あたり)
        /// </summary>
        private readonly int KeyEventMaxCount = 500;

        private readonly IKeyEventSender _arrowDownKeyEventSender;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ElementScroller(IKeyEventSender keyEventSender, int keyDonwMaxCount)
        {
            _arrowDownKeyEventSender = keyEventSender;
            KeyEventMaxCount = keyDonwMaxCount;
        }

        public bool IsOverflowScroll()
        {
            return _keyDownCount >= KeyEventMaxCount;
        }

        /// <summary>
        /// 検索位置移動
        /// </summary>
        /// <param name="lastElement"></param>
        public void MoveSearchPotision(IUIAutomationElement? lastElement)
        {
            // 最後の要素にフォーカスをあて，↓キー押下イベントを送ることで，スクロールが必要な場合に，移動することで表示させる．
            if (lastElement?.GetCurrentPattern(UIAutomationIdDefine.UIA_SelectionPatternId) is IUIAutomationSelectionItemPattern pattern)
            {
                pattern.Select();
                _arrowDownKeyEventSender.SendWait(KeyCode.Down);
            }
            _keyDownCount++;
        }

        /// <summary>
        /// スクロール位置を先頭に戻す
        /// </summary>
        public void ReturnScrollPositionToTop(IUIAutomationElement? lastElement, int moveCount)
        {
            // zoomが下キー入力連打しても一番下で止まってしまうようになっているため、
            // 一番上まで戻せるようにする
            if (lastElement?.GetCurrentPattern(UIAutomationIdDefine.UIA_SelectionPatternId) is IUIAutomationSelectionItemPattern pattern)
            {
                pattern.Select();
                for (int i = 0; i < moveCount; i++)
                {
                    _arrowDownKeyEventSender.SendWait(KeyCode.Up);
                }
            }
        }
    }
}
