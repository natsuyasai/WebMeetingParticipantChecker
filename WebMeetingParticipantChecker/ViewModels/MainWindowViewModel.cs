using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        /// プリセット関連
        /// </summary>
        private IPresetModel _presetList { get; set; } = default!;
        /// <summary>
        /// 監視関連
        /// </summary>
        private IMonitoringFacade _monitoringFacade { get; set; }


        /// <summary>
        /// プリセットフォルダ名
        /// </summary>
        private const string PresetFolderName = "Preset";


        #region 表示データ

        public string Title
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                return $"Zoom参加者チェック - ver{assembly.GetName().Version}";
            }
        }

        /// <summary>
        /// プリセット名一覧
        /// </summary>
        public ObservableCollection<PresetInfo> PresetNames
        {
            get
            {
                return new ObservableCollection<PresetInfo>(_presetList.GetPreset());
            }
        }

        /// <summary>
        /// 選択中のプリセットデータ
        /// </summary>
        public IEnumerable<string> SelectPresetData
        {
            get
            {
                return _presetList.GetCurrentPresetDataList();
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
                return new ObservableCollection<MonitoringInfo>(_monitoringFacade.GetMonitoringInfos().OrderBy(info => info.IsJoin));
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
                    int maxcount = _monitoringFacade.GetMonitoringInfos().Count();
                    int joincount = _monitoringFacade.GetMonitoringInfos().Count(item => item.IsJoin);
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
            get { return _monitoringFacade.IsEnableAutoScroll(); }
            set
            {
                _monitoringFacade.SetEnableAutoScroll(value);
                OnPropertyChanged(nameof(IsEnableAutoScroll));
            }
        }

        #endregion 表示データ

        #region コマンド

        /// <summary>
        /// プリセット再読込コマンド
        /// </summary>
        private AsyncRelayCommand? _reloadPresetCommand;
        public AsyncRelayCommand ReloadPresetCommand
        {
            get
            {
                return _reloadPresetCommand ??= new AsyncRelayCommand(ReadPresetData);
            }
        }

        /// <summary>
        /// プリセット編集コマンド
        /// </summary>
        private RelayCommand? _editPresetCommand;
        public RelayCommand EditPresetCommand
        {
            get
            {
                return _editPresetCommand ??= new RelayCommand(EditPresetData);
            }
        }

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
        public MainWindowViewModel(IMonitoringFacade monitoringFacade)
        {
            _status = StatusValue.Init;
            _monitoringFacade = monitoringFacade;
            OnPropertyChanged(nameof(StatusDisplayString));
        }

        /// <summary>
        /// プリセットデータ読み込み
        /// </summary>
        public async Task ReadPresetData()
        {
            await Task.Run(() =>
            {
                _presetList = new PresetModel();
                _presetList.Clear();
                OnPropertyChanged(nameof(PresetNames));
                _presetList.ReadPresetData(System.AppDomain.CurrentDomain.BaseDirectory, PresetFolderName);
                OnPropertyChanged(nameof(PresetNames));
            });
        }

        /// <summary>
        /// プリセット編集
        /// </summary>
        private void EditPresetData()
        {
            Process.Start(new ProcessStartInfo((_presetList.GetCurrntPresetFilePath())) { UseShellExecute = true });
        }

        /// <summary>
        /// プリセット選択アイテム設定
        /// </summary>
        /// <param name="presetInfo"></param>
        public void SetSelectedPreset(PresetInfo presetInfo)
        {
            _presetList.UpdateCurrentIndex(presetInfo.Id);
            OnPropertyChanged(nameof(SelectPresetData));
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
            OnPropertyChanged(nameof(CanStart));
            OnPropertyChanged(nameof(CanStop));
            OnPropertyChanged(nameof(CanPauseAndResume));
        }

        /// <summary>
        /// 監視開始
        /// </summary>
        private async Task StartMonitoring()
        {
            // 監視開始
            UpdateStatus(StatusValue.PreparingTargetWindowCaputure);
            _monitoringFacade.RegisterMonitoringTargets(_presetList.GetCurrentPresetDataList());
            OnPropertyChanged(nameof(MonitoringInfos));

            // Zoomの参加者ウィンドウ検索開始
            UpdateStatus(StatusValue.TargetWindowCaputure);
            await _monitoringFacade.SelectZoomParticipantElement(MonitoringType.Target.Teams, OnDetectedTargetElemetCallback);
        }

        /// <summary>
        /// 監視対象エレメント検出コールバック
        /// </summary>
        private async void OnDetectedTargetElemetCallback()
        {
            UpdateMonitoringStates();
            await _monitoringFacade.StartMonitoring(OnJoinStateChangeCallback);
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
            await Task.Run(() =>
            {
                UpdateStatus(StatusValue.Init);
                _monitoringFacade.StopMonitoring();
                OnPropertyChanged(nameof(MonitoringInfos));
            });
        }

        /// <summary>
        /// 監視停止(自動判定時)
        /// </summary>
        private async Task StopMonitoringWhenAutomaticJudgment()
        {
            await Task.Run(() =>
            {
                _monitoringFacade.StopMonitoring();
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
                _monitoringFacade.Pause();
                UpdateStatus(StatusValue.Pause);
            }
            else
            {
                _monitoringFacade.Resume();
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
                _monitoringFacade.SwitchingParticipantState(@int);
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
                _monitoringFacade.SetParticipantAuto(@int);
                OnPropertyChanged(nameof(MonitoringInfos));
            }
        }

        /// <summary>
        /// 監視状態更新
        /// </summary>
        private void UpdateMonitoringStates()
        {
            if (_monitoringFacade.IsAllJoin())
            {
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
