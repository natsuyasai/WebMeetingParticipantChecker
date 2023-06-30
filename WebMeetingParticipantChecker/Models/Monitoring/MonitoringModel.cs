using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMeetingParticipantChecker.Models.Config;
using WebMeetingParticipantChecker.Models.UIAutomation;
using WebMeetingParticipantChecker.Utils;

namespace WebMeetingParticipantChecker.Models.Monitoring
{
    /// <summary>
    /// タスクのキャンセル状態管理
    /// </summary>
    internal class TaskCancelStatus
    {
        /// <summary>
        /// 監視タスクをキャンセルするか
        /// </summary>
        public bool MonitoringTaskCancel { get; private set; } = false;

        /// <summary>
        /// 監視開始
        /// </summary>
        public void StartMonitoring()
        {
            MonitoringTaskCancel = false;
        }

        /// <summary>
        /// 監視停止
        /// </summary>
        public void StopMonitoring()
        {
            MonitoringTaskCancel = true;
        }
    }
    /// <summary>
    /// 監視関連
    /// </summary>
    internal class MonitoringModel
    {
        /// <summary>
        /// 監視情報
        /// </summary>
        private List<MonitoringInfo> _monitoringInfos = new List<MonitoringInfo>();
        private List<MonitoringInfo> _searchInfos = new List<MonitoringInfo>();

        /// <summary>
        /// タスクキャンセル状態管理
        /// </summary>
        private TaskCancelStatus _taskCancelStatus = new TaskCancelStatus();

        /// <summary>
        /// タスク一時停止用オブジェクト
        /// </summary>
        private SemaphoreSlim _taskPauseObject = new SemaphoreSlim(1, 1);

        /// <summary>
        /// タスク待機時間[ms]
        /// </summary>
        private readonly int DelayTimeMs = 2000;

        /// <summary>
        /// 自動スクロールを有効とするか
        /// </summary>
        public bool IsEnableAutoScroll = false;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MonitoringModel()
        {
            DelayTimeMs = AppSettingsManager.MonitoringCycleMs;
        }

        /// <summary>
        /// 監視情報取得
        /// </summary>
        public IEnumerable<MonitoringInfo> GetMonitoringInfos()
        {
            return _monitoringInfos;
        }

        /// <summary>
        /// 監視対象登録
        /// </summary>
        /// <param name="targetUsers"></param>
        public void RegisterMonitoringTargets(IEnumerable<string> targetUsers)
        {
            _searchInfos.Clear();
            _monitoringInfos.Clear();
            _monitoringInfos = targetUsers.Select((user, index) => new MonitoringInfo(index, user)).ToList();
            _searchInfos = _monitoringInfos.Select(info => new MonitoringInfo(info.Id, StringUtils.RemoveSpace(info.Name))).ToList();
            // スペースを区切りとみなして姓名入れ替え後の名前も検索用に保持
            _searchInfos.AddRange(_monitoringInfos.Aggregate(new List<MonitoringInfo>(), (accumulate, current) =>
            {
                if (StringUtils.TrySwapBeforeAndAfterTheSpace(current.Name, out var result))
                {
                    // 間にスペースがあった物のみ追加しておく
                    accumulate.Add(new MonitoringInfo(current.Id, StringUtils.RemoveSpace(result)));
                }
                return accumulate;
            }));
        }

        /// <summary>
        /// 監視開始
        /// </summary>
        public async Task StartMonitoring(Action onJoinStateChangeCallback, IUserNameElementGetter automationElementTreeInfoGetter)
        {
            _taskCancelStatus.StartMonitoring();
            await MonitoringParticioant(automationElementTreeInfoGetter, onJoinStateChangeCallback);
        }

        /// <summary>
        /// 監視一時停止
        /// </summary>
        public void Pause()
        {
            _taskPauseObject.Wait();
        }

        /// <summary>
        /// 監視再開
        /// </summary>
        public void Resume()
        {
            _taskPauseObject.Release();
        }

        /// <summary>
        /// 監視停止
        /// </summary>
        public void StopMonitoring()
        {
            _taskCancelStatus.StopMonitoring();
            if (_taskPauseObject.CurrentCount == 0)
            {
                try
                {
                    _taskPauseObject.Release();
                }
                catch
                {
                    _logger.Error("開放失敗");
                }
            }
        }

        /// <summary>
        /// 参加状態切り替え
        /// </summary>
        /// <param name="targetId"></param>
        public void SwitchingParticipantState(int targetId)
        {
            lock (_monitoringInfos)
            {
                var index = _monitoringInfos.FindIndex(item => item.IsMine(targetId));
                if (index >= 0)
                {
                    _monitoringInfos[index].SwitchJoinStateOfManual();
                }
            }
        }

        /// <summary>
        /// 全参加
        /// </summary>
        /// <returns></returns>
        public bool IsAllJoin()
        {
            lock (_monitoringInfos)
            {
                return _monitoringInfos.All(item => item.IsJoin);
            }
        }


        /// <summary>
        /// 監視状態を自動に設定
        /// </summary>
        /// <param name="target"></param>
        public void SetParticipantAuto(int target)
        {
            lock (_monitoringInfos)
            {
                var index = _monitoringInfos.FindIndex(item => item.IsMine(target));
                if (index >= 0)
                {
                    _monitoringInfos[index].ResetJoinState();
                }
            }
        }

        /// <summary>
        /// 対象監視
        /// </summary>
        private async Task MonitoringParticioant(IUserNameElementGetter treeInfoGetter, Action onJoinStateChangeCallback)
        {
            await Task.Run(async () =>
            {
                _logger.Info("監視中");
                while (!_taskCancelStatus.MonitoringTaskCancel)
                {
                    _taskPauseObject.Wait();
                    _taskPauseObject.Release();
                    // UIに変更通知
                    if (UpdateJoinState(treeInfoGetter))
                    {
                        onJoinStateChangeCallback();
                    }
                    if (IsAllJoin())
                    {
                        _taskCancelStatus.StopMonitoring();
                    }
                    else
                    {
                        // 未参加有のため，一定期間待つ
                        await Task.Delay(DelayTimeMs);
                    }
                }
            });
        }

        /// <summary>
        /// 参加状態更新
        /// </summary>
        /// <param name="treeInfoGetter"></param>
        /// <returns></returns>
        private bool UpdateJoinState(IUserNameElementGetter treeInfoGetter)
        {
            bool needNotifyChange = false;
            var dict = treeInfoGetter.GetNameList(IsEnableAutoScroll);
            foreach (var info in _searchInfos)
            {
                if (dict.ContainsKey(info.Name) || dict.Where(a => a.Key.Contains(info.Name)).FirstOrDefault().Value != null)
                {
                    lock (_monitoringInfos)
                    {
                        // 未参加なら変更通知
                        if (!_monitoringInfos[info.Id].IsJoinIncludeManual())
                        {
                            _monitoringInfos[info.Id].SetJoin();
                            needNotifyChange = true;
                        }
                    }
                }
            }
            return needNotifyChange;
        }
    }
}
