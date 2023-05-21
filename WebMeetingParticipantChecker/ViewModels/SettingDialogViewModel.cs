using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Configuration;
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
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["MonitoringCycleMs"].Value = _monitoringCycleMs;
                config.Save();
                OnPropertyChanged(nameof(ExistsNotAppliedData));
                WeakReferenceMessenger.Default.Send(new SettingApplyMessage("設定完了"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}