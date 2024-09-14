using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;
using WebMeetingParticipantChecker.Models.Config;

namespace WebMeetingParticipantChecker.ViewModels
{


    /// <summary>
    ///  MainWindow用VM
    /// </summary>
    internal class MainWindowViewModel : ObservableObject
    {
        public static string Title
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                return $"Web会議参加者チェック - ver{assembly.GetName().Version}";
            }
        }

        /// <summary>
        /// 常に最前面に表示する
        /// </summary>
        public bool IsAlwaysTop
        {
            get
            {
                return AppSettingsManager.IsAlwaysTop;
            }
        }
    }

}
