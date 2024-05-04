using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NLog;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebMeetingParticipantChecker.Models.Monitoring;
using WebMeetingParticipantChecker.Models.Preset;

namespace WebMeetingParticipantChecker.ViewModels
{


    /// <summary>
    ///  MainWindow用VM
    /// </summary>
    internal class MainWindowViewModel : ObservableObject
    {

        /// <summary>
        /// ステータス値
        /// </summary>
        enum StatusValue
        {
            /// <summary>
            /// 初期状態
            /// </summary>
            Init = 0,
            /// <summary>
            /// 対象ウィンドウ捕捉準備
            /// </summary>
            PreparingTargetWindowCaputure,
            /// <summary>
            /// 対象ウィンドウ捕捉中
            /// </summary>
            TargetWindowCaputure,
            /// <summary>
            /// 監視中
            /// </summary>
            Monitoring,
            /// <summary>
            /// 完了
            /// </summary>
            Complete,
            /// <summary>
            /// 一時停止
            /// </summary>
            Pause,
            Max,
        }
        /// <summary>
        /// ステータス文字列
        /// </summary>
        private readonly string[] StatusString = new string[(int)StatusValue.Max]
        {
        "未監視",
        "準備中",
        "対象ウィンドウ捕捉中(参加者リスト要素をクリックしてください)",
        "監視中……",
        "対象者参加済み",
        "一時停止中",
        };


        /// <summary>
        /// 監視関連
        /// </summary>
        private readonly IMonitoring _monitoringService;

        /// <summary>
        /// プリセット関連
        /// </summary>
        private readonly IPreset _preset;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();


        #region 表示データ

        public static string Title
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                return $"Web会議参加者チェック - ver{assembly.GetName().Version}";
            }
        }

        /// <summary>
        /// ポーズ/再開ボタン名
        /// </summary>
        public string PauseButtonString
        {
            get
            {
                return _status == StatusValue.Pause ? "再開" : "一時停止";
            }
        }
        public string PauseButtonIcon
        {
            get
            {
                return _status == StatusValue.Pause ?
                    "resume_FILL0_wght400_GRAD0_opsz24.png" :
                    "pause_FILL0_wght400_GRAD0_opsz24.png";
            }
        }


        /// <summary>
        /// 実行可能か
        /// </summary>
        public bool CanStart
        {
            get
            {
                return _status == StatusValue.Init || _status == StatusValue.Complete;
            }
        }
        /// <summary>
        /// 停止可能か
        /// </summary>
        public bool CanStop
        {
            get
            {
                return !CanStart;
            }
        }

        /// <summary>
        /// 一時停止/再開可能か
        /// </summary>
        public bool CanPauseAndResume
        {
            get
            {
                return CanStop && _status >= StatusValue.Monitoring;
            }
        }

        /// <summary>
        /// 監視情報
        /// </summary>
        public ObservableCollection<MonitoringInfo> MonitoringInfos
        {
            get
            {
                return new ObservableCollection<MonitoringInfo>(_monitoringService.GetMonitoringInfos().OrderBy(info => info.IsJoin));
            }
        }

        /// <summary>
        /// ステータス
        /// </summary>
        private StatusValue _status;
        public string StatusDisplayString
        {
            get
            {
                if (_status == StatusValue.Monitoring || _status == StatusValue.Pause)
                {
                    int maxcount = _monitoringService.GetMonitoringInfos().Count();
                    int joincount = _monitoringService.GetMonitoringInfos().Count(item => item.IsJoin);
                    return StatusString[(int)_status] + $"(参加：{joincount}、未参加：{maxcount - joincount})";
                }
                else
                {
                    return StatusString[(int)_status];
                }
            }
        }

        /// <summary>
        /// 自動スクロールを有効とするか
        /// </summary>
        public bool IsEnableAutoScroll
        {
            get { return _monitoringService.IsEnableAutoScroll(); }
            set
            {
                _monitoringService.SetEnableAutoScroll(value);
                OnPropertyChanged(nameof(IsEnableAutoScroll));
            }
        }

        /// <summary>
        /// 監視対象
        /// </summary>
        private MonitoringType.Target _targetType = MonitoringType.Target.Zoom;
        public MonitoringType.Target CheckTarget
        {
            get => _targetType;
            set
            {
                _targetType = value;
                OnPropertyChanged(nameof(CheckTarget));
            }
        }

        /// <summary>
        /// 監視対象切り替えラジオボタンを有効とするか
        /// </summary>
        public bool IsEnableTargetTypeRadio
        {
            get { return _status == StatusValue.Init; }
        }

        #endregion 表示データ

        #region コマンド

        /// <summary>
        /// 開始コマンド
        /// </summary>
        private AsyncRelayCommand? _startCommand;
        public AsyncRelayCommand StartCommand
        {
            get
            {
                return _startCommand ??= new AsyncRelayCommand(StartMonitoring);
            }
        }

        /// <summary>
        /// 停止コマンド
        /// </summary>
        private AsyncRelayCommand? _stioCommand;
        public AsyncRelayCommand StopCommand
        {
            get
            {
                return _stioCommand ??= new AsyncRelayCommand(StopMonitoring);
            }
        }

        /// <summary>
        /// 一時停止/再開コマンド
        /// </summary>
        private RelayCommand? _pauseCommand;
        public RelayCommand PauseCommand
        {
            get
            {
                return _pauseCommand ??= new RelayCommand(PauseOrResume);
            }
        }

        /// <summary>
        /// 参加状態切り替えコマンド
        /// </summary>
        private RelayCommand<object>? _switchingParticipantStateCommand;
        public RelayCommand<object> SwitchingParticipantStateCommand
        {
            get
            {
                return _switchingParticipantStateCommand ??= new RelayCommand<object>(SwitchingParticipantState);
            }
        }

        /// <summary>
        /// 参加状態自動監視コマンド
        /// </summary>
        private RelayCommand<object>? _setParticipantAutoCommand;
        public RelayCommand<object> SetParticipantAutoCommand
        {
            get
            {
                return _setParticipantAutoCommand ??= new RelayCommand<object>(SetParticipantAuto);
            }
        }
        #endregion コマンド

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel(IMonitoring monitoring, IPreset preset)
        {
            _status = StatusValue.Init;
            _monitoringService = monitoring;
            _preset = preset;
            OnPropertyChanged(nameof(StatusDisplayString));
        }

        /// <summary>
        /// ステータス更新
        /// </summary>
        /// <param name="value"></param>
        private void UpdateStatus(StatusValue value)
        {
            _status = value;
            OnPropertyChanged(nameof(StatusDisplayString));
            OnPropertyChanged(nameof(PauseButtonString));
            OnPropertyChanged(nameof(PauseButtonIcon));
            OnPropertyChanged(nameof(CanStart));
            OnPropertyChanged(nameof(CanStop));
            OnPropertyChanged(nameof(CanPauseAndResume));
            OnPropertyChanged(nameof(IsEnableTargetTypeRadio));
        }

        /// <summary>
        /// 監視開始
        /// </summary>
        private async Task StartMonitoring()
        {
            _logger.Info("監視開始");
            // 監視開始
            UpdateStatus(StatusValue.PreparingTargetWindowCaputure);
            _monitoringService.RegisterMonitoringTargets(_preset.GetCurrentPresetDataList());
            OnPropertyChanged(nameof(MonitoringInfos));

            // Zoomの参加者ウィンドウ検索開始
            UpdateStatus(StatusValue.TargetWindowCaputure);
            await _monitoringService.SelectZoomParticipantElement(_targetType, OnDetectedTargetElemetCallback);
        }

        /// <summary>
        /// 監視対象エレメント検出コールバック
        /// </summary>
        private async void OnDetectedTargetElemetCallback()
        {
            _logger.Info("対象要素検出");
            UpdateMonitoringStates();
            await _monitoringService.StartMonitoring(OnJoinStateChangeCallback);
        }

        /// <summary>
        /// 参加状態変更コールバック
        /// </summary>
        private void OnJoinStateChangeCallback()
        {
            OnPropertyChanged(nameof(MonitoringInfos));
            UpdateMonitoringStates();
        }

        /// <summary>
        /// 監視停止
        /// </summary>
        private async Task StopMonitoring()
        {
            _logger.Info("監視停止");
            await Task.Run(() =>
            {
                UpdateStatus(StatusValue.Init);
                _monitoringService.StopMonitoring();
                OnPropertyChanged(nameof(MonitoringInfos));
            });
        }

        /// <summary>
        /// 監視停止(自動判定時)
        /// </summary>
        private async Task StopMonitoringWhenAutomaticJudgment()
        {
            _logger.Info("監視停止(自動判定)");
            await Task.Run(() =>
            {
                UpdateStatus(StatusValue.Init);
                _monitoringService.StopMonitoring();
                OnPropertyChanged(nameof(MonitoringInfos));
            });
        }

        /// <summary>
        /// 一時停止
        /// </summary>
        private void PauseOrResume()
        {
            if (_status != StatusValue.Pause)
            {
                _logger.Info("一時停止");
                _monitoringService.Pause();
                UpdateStatus(StatusValue.Pause);
            }
            else
            {
                _logger.Info("再開");
                _monitoringService.Resume();
                UpdateMonitoringStates();
            }
        }

        /// <summary>
        /// 参加状態切り替え
        /// </summary>
        private void SwitchingParticipantState(object? target)
        {
            if (target is int @int)
            {
                _monitoringService.SwitchingParticipantState(@int);
                OnPropertyChanged(nameof(MonitoringInfos));
                if (_status == StatusValue.Monitoring)
                {
                    // 監視中なら通常通り更新
                    UpdateMonitoringStates();
                }
                else if (_status == StatusValue.Pause)
                {
                    // 一時停止中なら件数の更新のみ実施
                    UpdateStatus(StatusValue.Pause);
                }
            }
        }

        /// <summary>
        /// 自動監視に設定
        /// </summary>
        private void SetParticipantAuto(object? target)
        {
            if (target is int @int)
            {
                _monitoringService.SetParticipantAuto(@int);
                OnPropertyChanged(nameof(MonitoringInfos));
            }
        }

        /// <summary>
        /// 監視状態更新
        /// </summary>
        private void UpdateMonitoringStates()
        {
            if (_monitoringService.IsAllJoin())
            {
                _logger.Info("全員参加済み");
                UpdateStatus(StatusValue.Complete);
                _ = StopMonitoringWhenAutomaticJudgment();
            }
            else
            {
                UpdateStatus(StatusValue.Monitoring);
            }
        }
    }

}
