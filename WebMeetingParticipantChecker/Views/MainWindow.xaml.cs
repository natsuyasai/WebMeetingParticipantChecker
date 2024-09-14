using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WebMeetingParticipantChecker.Models.Message;
using WebMeetingParticipantChecker.Models.Preset;
using WebMeetingParticipantChecker.ViewModels;

namespace WebMeetingParticipantChecker.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly SettingDialog _settingDialog = new();

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();

            _mainWindowViewModel = App.Services.GetService<MainWindowViewModel>()!;
            DataContext = _mainWindowViewModel;

            WeakReferenceMessenger.Default.Register<MainWindow, Message<MainWindow>>(this, (s, e) =>
            {
                ShowMessage(s, e);
            });
        }

        private void HandleClose(object _, RoutedEventArgs e)
        {
            _logger.Info("終了");
            _settingDialog.Close();
            SystemCommands.CloseWindow(this);
        }

        private void HandleMinimize(object _, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void HandleMaximizeOrRestoreWindow(object sender, RoutedEventArgs e)
        {
            // 最大化，縮小に合わせてボタンコンテンツを切り替える
            if (WindowState != WindowState.Maximized)
            {
                SystemCommands.MaximizeWindow(this);
                if (sender is Button button)
                {
                    button.Content = "2";
                }
            }
            else
            {
                SystemCommands.RestoreWindow(this);
                if (sender is Button button)
                {
                    button.Content = "1";
                }
            }
        }

        private void WindowStateChanged(object _, object e)
        {
            // 最大化，縮小に合わせてボタンコンテンツを切り替える
            if (FindName("MaximizeButton") is not Button button)
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

        private void HandleSetting(object _, RoutedEventArgs e)
        {
            _settingDialog.Owner = this;
            _settingDialog.Show();
        }

        private void ShowMessage(object _, Message<MainWindow> message)
        {
            var msg = new MessageDialog();
            msg.Initialize(message.Value.Title, message.Value.Message, message.Value.OkButtonMessage, this);
            var result = msg.ShowDialog();
            message.Value.OnCloseDialog?.Invoke(result == true ? ResultCode.OK : ResultCode.Close);
        }
    }
}
