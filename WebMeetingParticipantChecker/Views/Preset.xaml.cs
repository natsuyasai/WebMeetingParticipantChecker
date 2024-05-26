using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebMeetingParticipantChecker.Models.Preset;
using WebMeetingParticipantChecker.ViewModels;

namespace WebMeetingParticipantChecker.Views
{
    /// <summary>
    /// Preset.xaml の相互作用ロジック
    /// </summary>
    public partial class Preset : UserControl
    {
        private readonly PresetViewModel _presetViewModel;

        public Preset()
        {
            InitializeComponent();
            _presetViewModel = App.Services.GetService<PresetViewModel>()!;
            DataContext = _presetViewModel;
            Task.Run(async () =>
            {
                await _presetViewModel.ReadPresetData();
            });
        }

        /// <summary>
        /// プリセット選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionChangedPreset(object _, SelectionChangedEventArgs e)
        {
            PresetInfo selectedItem = (PresetInfo)cbPreset.SelectedItem;
            if (selectedItem != null)
            {
                _presetViewModel.SetSelectedPreset(selectedItem);
            }
        }
    }
}
