﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIAutomationClient;
using WebMeetingParticipantChecker.Models.Config;
using WebMeetingParticipantChecker.Models.FileWriter;
using WebMeetingParticipantChecker.Models.Message;
using WebMeetingParticipantChecker.Models.Monitoring;
using WebMeetingParticipantChecker.Models.Preset;
using WebMeetingParticipantChecker.Models.UIAutomation;
using WebMeetingParticipantChecker.Views;

namespace WebMeetingParticipantChecker.ViewModels
{
    internal class MonitoringViewModel : ObservableObject
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
        private readonly IReadOnlyPreset _preset;

        /// <summary>
        /// 参加者リスト取得
        /// </summary>
        private readonly IAutomationElementGetter[] _automationElementGetter;

        /// <summary>
        /// 監視
        /// </summary>
        private readonly MonitoringModel _monitoringModel;

        private readonly IMonitoringResultExportable _resultExporter;

        private readonly IKeyEventSender _arrowDownKeyEventSender;

        private readonly int _keydownMaxCount;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #region 表示データ

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
        public ObservableCollection<UserState> UserStates
        {
            get
            {
                return new ObservableCollection<UserState>(_monitoringModel.GetUserStates().OrderBy(info => info.IsJoin));
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
                    int maxcount = _monitoringModel.GetUserStates().Count();
                    int joincount = _monitoringModel.GetUserStates().Count(item => item.IsJoin);
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
            get { return _monitoringModel.IsEnableAutoScroll; }
            set
            {
                _monitoringModel.IsEnableAutoScroll = value;
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

        private RelayCommand? _exportResultCommand;
        public RelayCommand ExportResultCommand
        {
            get
            {
                return _exportResultCommand ??= new RelayCommand(ExportResult);
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
        public MonitoringViewModel(IAutomationElementGetter[] automationElementGetter,
                                   MonitoringModel monitoringModel,
                                   IKeyEventSender arrowDownKeyEventSender,
                                   IReadOnlyPreset preset,
                                   IMonitoringResultExportable resultExporter,
                                   int keydownMaxCount)
        {
            _status = StatusValue.Init;
            _automationElementGetter = automationElementGetter;
            _monitoringModel = monitoringModel;
            _arrowDownKeyEventSender = arrowDownKeyEventSender;
            _preset = preset;
            OnPropertyChanged(nameof(StatusDisplayString));
            _resultExporter = resultExporter;
            _keydownMaxCount = keydownMaxCount;
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
            _monitoringModel.RegisterMonitoringTargets(_preset.GetCurrentPresetUsers());
            OnPropertyChanged(nameof(UserStates));

            // Zoomの参加者ウィンドウ検索開始
            UpdateStatus(StatusValue.TargetWindowCaputure);
            await Task.Run(() =>
            {
                _automationElementGetter[(int)_targetType].SubscribeToFocusChange(() =>
                {
                    _logger.Info("対象エレメント検知");
                    OnDetectedTargetElemetCallback();
                });
            });
        }

        /// <summary>
        /// 監視対象エレメント検出コールバック
        /// </summary>
        private async void OnDetectedTargetElemetCallback()
        {
            _logger.Info("対象要素検出");
            UpdateMonitoringStates();
            if (_automationElementGetter[(int)_targetType].GetTargetElement() == null)
            {
                _logger.Error("対象の要素が見つかっていません");
                await StopMonitoring();
                return;
            }
            _logger.Info("タスク開始");
            await _monitoringModel.StartMonitoring(OnJoinStateChangeCallback, GetUserNameElementGetter());
        }

        /// <summary>
        /// 子要素取得クラス取得
        /// </summary>
        /// <returns></returns>
        private UserNameElementGetter GetUserNameElementGetter()
        {
            return _targetType switch
            {
                MonitoringType.Target.Zoom =>
                new UserNameElementGetterForZoom(
                    new CUIAutomation(), _automationElementGetter[(int)_targetType].GetTargetElement()!, _arrowDownKeyEventSender, _keydownMaxCount),
                MonitoringType.Target.Teams =>
                new UserNameElementGetterForTeams(
                    new CUIAutomation(), _automationElementGetter[(int)_targetType].GetTargetElement()!, _arrowDownKeyEventSender, _keydownMaxCount),
                _ =>
                new UserNameElementGetterForZoom(
                    new CUIAutomation(), _automationElementGetter[(int)_targetType].GetTargetElement()!, _arrowDownKeyEventSender, _keydownMaxCount),
            };
        }

        /// <summary>
        /// 参加状態変更コールバック
        /// </summary>
        private void OnJoinStateChangeCallback()
        {
            OnPropertyChanged(nameof(UserStates));
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
                _monitoringModel.StopMonitoring();
                OnPropertyChanged(nameof(UserStates));
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
                _monitoringModel.StopMonitoring();
                OnPropertyChanged(nameof(UserStates));
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
                _monitoringModel.Pause();
                UpdateStatus(StatusValue.Pause);
            }
            else
            {
                _logger.Info("再開");
                _monitoringModel.Resume();
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
                _monitoringModel.SwitchingParticipantState(@int);
                OnPropertyChanged(nameof(UserStates));
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
                _monitoringModel.SetParticipantAuto(@int);
                OnPropertyChanged(nameof(UserStates));
            }
        }

        /// <summary>
        /// 監視状態更新
        /// </summary>
        private void UpdateMonitoringStates()
        {
            if (_monitoringModel.IsAllJoin())
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

        /// <summary>
        /// 結果出力
        /// </summary>
        private void ExportResult()
        {
            var filename = _preset.GetCurrentPresetName();
            if (!_monitoringModel.GetUserStates().Any())
            {
                WeakReferenceMessenger.Default.Send(new Message<MainWindow>(
                    new MessageInfo
                    {
                        Title = "情報",
                        Message = "出力する結果がありません。"
                    }));
                return;
            }
            var result = _resultExporter.Export(filename, _monitoringModel.GetUserStates());
            if (result)
            {
                WeakReferenceMessenger.Default.Send(new Message<MainWindow>(
                    new MessageInfo
                    {
                        Title = "情報",
                        Message = $"出力しました。\r\nファイル名：{filename}"
                    }));
            }
            else
            {
                WeakReferenceMessenger.Default.Send(new Message<MainWindow>(
                    new MessageInfo
                    {
                        Title = "エラー",
                        Message = $"出力に失敗しました。"
                    }));
            }
        }
    }
}
