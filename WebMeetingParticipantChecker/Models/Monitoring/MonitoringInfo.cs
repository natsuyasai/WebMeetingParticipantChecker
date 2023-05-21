namespace WebMeetingParticipantChecker.Models.Monitoring
{
    /// <summary>
    /// モニタ情報
    /// </summary>
    class MonitoringInfo
    {
        /// <summary>
        /// 識別子 
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// 表示名
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 参加済みか
        /// </summary>
        public bool IsJoin { get; private set; } = false;
        /// <summary>
        /// 手動更新
        /// </summary>
        public bool IsManual { get; private set; } = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="name">名前</param>
        public MonitoringInfo(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// 自身か
        /// </summary>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public bool IsMine(int targetId)
            => Id == targetId;

        /// <summary>
        /// 参加中か(手動変更を含む)
        /// 手動で参加中にしているか自動で参加状態になっているか
        /// </summary>
        /// <returns></returns>
        public bool IsJoinIncludeManual()
            => IsManual || IsJoin;

        /// <summary>
        /// 参加状態切り替え
        /// </summary>
        public void SwitchJoinState()
        {
            IsJoin = !IsJoin;
        }

        /// <summary>
        /// 参加状態切り替え(手動)
        /// </summary>
        public void SwitchJoinStateOfManual()
        {
            SwitchJoinState();
            IsManual = true;
        }

        /// <summary>
        /// 参加状態リセット
        /// </summary>
        public void ResetJoinState()
        {
            IsJoin = false;
            IsManual = false;
        }

        /// <summary>
        /// 参加状態設定
        /// </summary>
        public void SetJoin()
        {
            IsJoin = true;
        }
    }
}
