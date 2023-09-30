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

namespace WebMeetingParticipantChecker.Models.UIAutomation
{
    internal class ElementScroller
    {
        private int _keyDownCount = 0;

        /// <summary>
        /// キーダウンイベントを発生さえる最大回数(1回の更新あたり)
        /// </summary>
        private readonly int KeyDonwMaxCount = 200;

        private readonly IKeyEventSender _arrowDownKeyEventSender;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ElementScroller(IKeyEventSender keyEventSender, int? keyDonwMaxCount = null)
        {
            _arrowDownKeyEventSender = keyEventSender;
            if (keyDonwMaxCount == null)
            {
                KeyDonwMaxCount = AppSettingsManager.KyedownMaxCount;
            }
            else
            {
                KeyDonwMaxCount = keyDonwMaxCount.Value;
            }
        }

        public bool IsOverflowScroll()
        {
            return _keyDownCount >= KeyDonwMaxCount;
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
                _arrowDownKeyEventSender.SendWait();
            }
            _keyDownCount++;
        }
    }
}
