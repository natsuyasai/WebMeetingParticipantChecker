using System.Collections.Generic;

namespace WebMeetingParticipantChecker.Models.UIAutomation
{
    /// <summary>
    /// AutomationElementツリー情報取得
    /// </summary>
    public interface IAutomationElementChildNameInfoGetter
    {
        /// <summary>
        /// ツリー情報更新
        /// </summary>
        IDictionary<string, string> UpdateNameListInfo(bool isEnableAutoScroll);
    }
}
