using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMeetingParticipantChecker.Models.Preset;

namespace WebMeetingParticipantChecker.ViewModels
{
    internal class PresetViewModel : ObservableObject
    {
        /// <summary>
        /// プリセット関連
        /// </summary>
        private readonly IPresetProvider _preset;

        /// <summary>
        /// プリセットフォルダ名
        /// </summary>
        private const string PresetFolderName = "Preset";

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public PresetViewModel(IPresetProvider preset)
        {
            _preset = preset;
        }

        /// <summary>
        /// プリセット名一覧
        /// </summary>
        public ObservableCollection<PresetInfo> PresetNames
        {
            get
            {
                return new ObservableCollection<PresetInfo>(_preset.GetPreset());
            }
        }

        /// <summary>
        /// 選択中のプリセットデータ
        /// </summary>
        public IEnumerable<string> SelectPresetData
        {
            get
            {
                return _preset.GetCurrentPresetDataList();
            }
        }

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
        /// プリセットデータ読み込み
        /// </summary>
        public async Task ReadPresetData()
        {
            await Task.Run(() =>
            {
                _preset.Clear();
                OnPropertyChanged(nameof(PresetNames));
                _preset.ReadPresetData(System.AppDomain.CurrentDomain.BaseDirectory, PresetFolderName);
                OnPropertyChanged(nameof(PresetNames));
            });
        }

        /// <summary>
        /// プリセット編集
        /// </summary>
        private void EditPresetData()
        {
            Process.Start(new ProcessStartInfo((_preset.GetCurrntPresetFilePath())) { UseShellExecute = true });
        }

        /// <summary>
        /// プリセット選択アイテム設定
        /// </summary>
        /// <param name="presetInfo"></param>
        public void SetSelectedPreset(PresetInfo presetInfo)
        {
            _preset.UpdateCurrentIndex(presetInfo.Id);
            OnPropertyChanged(nameof(SelectPresetData));
        }
    }
}
