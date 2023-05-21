using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WebMeetingParticipantChecker.Models.Preset;
using WebMeetingParticipantChecker.ViewModels;

namespace WebMeetingParticipantChecker.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _mainWindowViewModel;
        private SettingDialog _settingDialog = new SettingDialog();

        public MainWindow()
        {
            InitializeComponent();

            _mainWindowViewModel = App.Services.GetService<MainWindowViewModel>()!;
            DataContext = _mainWindowViewModel;
            Task.Run(async () =>
            {
                await _mainWindowViewModel.ReadPresetData();
            });
        }

        /// <summary>
        /// プリセット選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionChangedPreset(object sender, SelectionChangedEventArgs e)
        {
            PresetInfo selectedItem = (PresetInfo)cbPreset.SelectedItem;
            if (selectedItem != null)
            {
                _mainWindowViewModel.SetSelectedPreset(selectedItem);
            }
        }

        private void HandleClose(object sender, RoutedEventArgs e)
        {
            _settingDialog.Close();
            SystemCommands.CloseWindow(this);
        }

        private void HandleMinimize(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void HandleMaximizeOrRestoreWindow(object sender, RoutedEventArgs e)
        {
            // 最大化，縮小に合わせてボタンコンテンツを切り替える
            if (WindowState != WindowState.Maximized)
            {
                SystemCommands.MaximizeWindow(this);
                if (sender is Button)
                {
                    ((Button)sender).Content = "2";
                }
            }
            else
            {
                SystemCommands.RestoreWindow(this);
                if (sender is Button)
                {
                    ((Button)sender).Content = "1";
                }
            }
        }

        private void WindowStateChanged(object sender, object e)
        {
            // 最大化，縮小に合わせてボタンコンテンツを切り替える
            if (!(FindName("MaximizeButton") is Button button))
            {
                return;
            }
            if (WindowState == WindowState.Maximized)
            {
                button.Content = "2";
            }
            else
            {
                button.Content = "1";
            }
        }

        private void HandleSetting(object sender, RoutedEventArgs e)
        {
            _settingDialog.Owner = this;
            _settingDialog.Show();
        }
    }
}
