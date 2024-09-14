using System.Windows;
using System.Windows.Controls;

namespace WebMeetingParticipantChecker.Views
{
    /// <summary>
    /// MessageDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class MessageDialog : Window
    {
        public MessageDialog()
        {
            InitializeComponent();
            SizeToContent = SizeToContent.Height;
        }

        public void Initialize(string title, string message, string okButtonMessage, Window? owner = null)
        {
            if (FindName("Message") is TextBlock messageElement)
            {
                messageElement.Text = message;
            }

            if (FindName("TitleLabel") is Label titleLabelElement)
            {
                titleLabelElement.Content = title;
            }

            if (FindName("OkButton") is Button okButton)
            {
                okButton.Content = okButtonMessage;
                if (okButtonMessage == "")
                {
                    okButton.Visibility = Visibility.Collapsed;
                }
            }

            if (owner != null)
            {
                Owner = owner;
            }
        }

        private void HandleOK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            SystemCommands.CloseWindow(this);
        }

        private void HandleClose(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }
    }
}
