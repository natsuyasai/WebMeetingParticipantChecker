using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Controls;
using WebMeetingParticipantChecker.Models.Message;
using WebMeetingParticipantChecker.ViewModels;

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

            WeakReferenceMessenger.Default.Register<SettingDialog, Message<SettingDialog>>(this, (s, e) =>
            {
                ShowMessage(s, e);
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

        private void ShowMessage(object sender, Message<SettingDialog> message)
        {
            var msg = new MessageDialog();
            msg.Initialize(message.Value.Title, message.Value.Message, this);
            msg.ShowDialog();
        }
    }
}
