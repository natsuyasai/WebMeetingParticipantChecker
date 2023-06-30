using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using WebMeetingParticipantChecker.Models.Config;
using ZoomParticipantChecker.Model.Message;

namespace WebMeetingParticipantChecker.ViewModels
{
    /// <summary>
    /// 設定画面用VM
    /// </summary>
    internal class SettingDialogViewModel : ObservableObject
    {
        private readonly string _initMonitoringCycleMs;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #region 表示データ

        /// <summary>
        /// 
        /// </summary>
        private string _monitoringCycleMs = "";
        public string MonitoringCycleMs
        {
            get { return _monitoringCycleMs; }
            set
            {
                _monitoringCycleMs = value;
                OnPropertyChanged(nameof(MonitoringCycleMs));
            }
        }

        public string ExistsNotAppliedData
        {
            get
            {
                if (_initMonitoringCycleMs != _monitoringCycleMs)
                {
                    return "※ 変更適用後再起動されていません。";
                }
                return "";
            }
        }

        #endregion 表示データ

        #region コマンド

        /// <summary>
        /// 適用
        /// </summary>
        private RelayCommand? _applyCommand;
        public RelayCommand ApplyCommand
        {
            get
            {
                return _applyCommand ??= new RelayCommand(Apply);
            }
        }

        #endregion

        public SettingDialogViewModel()
        {
            _monitoringCycleMs = AppSettingsManager.MonitoringCycleMs.ToString();
            _initMonitoringCycleMs = _monitoringCycleMs;
        }

        private void Apply()
        {
            try
            {
                var path = ConfigDefine.GetFileNameForFullPath();

                var config = ReadCurrentSetting(path);
                if (config == null)
                {
                    _logger.Error("読み込み失敗");
                    return;
                }

                var props = typeof(ConfigrationParameter).GetProperties();

                var targetProp = props.FirstOrDefault(item => item.Name == "MonitoringCycleMs");
                targetProp?.SetValue(config, _monitoringCycleMs);

                using var writer = new StreamWriter(path);
                var json = JsonSerializer.Serialize(config);
                writer.Write(json);

                OnPropertyChanged(nameof(ExistsNotAppliedData));
                WeakReferenceMessenger.Default.Send(new SettingApplyMessage("設定完了"));
                _logger.Info("設定変更");
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        private static ConfigrationParameter? ReadCurrentSetting(string path)
        {
            using var reader = new StreamReader(path);
            var json = reader.ReadToEnd();
            var config = JsonSerializer.Deserialize<ConfigrationParameter>(json);
            return config;
        }
    }
}