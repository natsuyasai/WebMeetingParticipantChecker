using System.Collections.Generic;

namespace WebMeetingParticipantChecker.Models.UIAutomation.UserNameGetter
{
    /// <summary>
    /// AutomationElementのユーザー名表示要素取得
    /// </summary>
    public interface IUserNameElementGetter
    {
        /// <summary>
        /// 名前要素取得
        /// </summary>
        /// <returns>
        /// Key：名前、Value：加工前の名前
        /// </returns>
        IDictionary<string, string> GetNameList(bool isEnableAutoScroll);
    }
}
