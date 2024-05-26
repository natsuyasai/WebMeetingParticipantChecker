using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;

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
    }

}
