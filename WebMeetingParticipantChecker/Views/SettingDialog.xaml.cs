using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Controls;
using WebMeetingParticipantChecker.ViewModels;
using ZoomParticipantChecker.Model.Message;

namespace WebMeetingParticipantChecker.Views
{
    /// <summary>
    /// SettingDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingDialog : Window
    {
        private readonly SettingDialogViewModel settingDialogViewModel;

        public SettingDialog()
        {
            InitializeComponent();

            settingDialogViewModel = new SettingDialogViewModel();
            DataContext = settingDialogViewModel;

            WeakReferenceMessenger.Default.Register<SettingDialog, SettingApplyMessage>(this, (s, e) =>
            {
                ShowApplyMessage();
            });
        }

        private void HandleClose(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        private void HandleTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBox tb) { return; }
            tb.SelectAll();
        }
        private void HandleMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is not TextBox tb) { return; }

            if (tb.IsFocused) { return; }
            tb.Focus();
            e.Handled = true;
        }

        private void ShowApplyMessage()
        {
            var msg = new MessageDialog();
            msg.Initialize("情報", "適用しました。設定値は再起動後有効となります。", this);
            msg.ShowDialog();
        }
    }
}
