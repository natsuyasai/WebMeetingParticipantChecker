using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Documents;
using WebMeetingParticipantChecker.Models.Config;
using WebMeetingParticipantChecker.Models.Message;
using WebMeetingParticipantChecker.Models.Theme;
using WebMeetingParticipantChecker.Views;

namespace WebMeetingParticipantChecker.ViewModels
{
    /// <summary>
    /// 設定画面用VM
    /// </summary>
    internal class SettingDialogViewModel : ObservableObject
    {
        private readonly string _initMonitoringCycleMs;
        private readonly int? _initThemeId;

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

        private Theme _selectedTheme = ThemeDefine.ThemeDefault.ElementAt(2);
        public Theme SelectedTheme
        {
            get { return _selectedTheme; }
            set
            {
                SetProperty(ref _selectedTheme, value);
                OnPropertyChanged(nameof(SelectedTheme));
            }
        }
        private IEnumerable<Theme> _theme = ThemeDefine.ThemeDefault;
        public ObservableCollection<Theme> Theme
        {
            get { return new ObservableCollection<Theme>(_theme); }
        }

        public string ExistsNotAppliedData
        {
            get
            {
                if (_initMonitoringCycleMs != _monitoringCycleMs
                    || _initThemeId != _selectedTheme.Id)
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
            var currentThemeId = AppSettingsManager.ThemeId;
            _selectedTheme = (ThemeDefine.IsContaine(currentThemeId)) 
                ? ThemeDefine.ThemeDefault.ElementAt(currentThemeId) : ThemeDefine.ThemeDefault.ElementAt(2);
            _initThemeId = currentThemeId;
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

                UpdateProperty(ref config, "MonitoringCycleMs", _monitoringCycleMs);
                UpdateProperty(ref config, "ThemeId", _selectedTheme.Id.ToString());

                using var writer = new StreamWriter(path);
                var json = JsonSerializer.Serialize(config);
                writer.Write(json);

                OnPropertyChanged(nameof(ExistsNotAppliedData));
                WeakReferenceMessenger.Default.Send(new Message<SettingDialog>(new MessageInfo
                {
                    Title = "情報",
                    Message = "適用しました。設定値は再起動後有効となります。"
                }));
                _logger.Info("設定変更");
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        private void UpdateProperty(ref ConfigrationParameter config, string key, string value)
        {
            var props = typeof(ConfigrationParameter).GetProperties();
            var targetProp = props.FirstOrDefault(item => item.Name == key);
            targetProp?.SetValue(config, value);
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