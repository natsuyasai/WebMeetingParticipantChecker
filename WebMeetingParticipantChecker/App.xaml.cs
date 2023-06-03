using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WebMeetingParticipantChecker.Models.Config;
using WebMeetingParticipantChecker.Models.Monitoring;
using WebMeetingParticipantChecker.Models.UIAutomation;
using WebMeetingParticipantChecker.ViewModels;

namespace WebMeetingParticipantChecker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    partial class App : Application
    {
        public static IServiceProvider Services { get; } = ConfigureServices();

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton<IConfigurationManager, ConfigurationManagerWrapper>()
                .AddSingleton<AutomationElementGetter[]>(
                provider => new AutomationElementGetter[] {
                    new ZoomAutomationElementGetter(),
                    new TeamsAutomationElementGetter() })
                .AddSingleton<MonitoringModel>()
                .AddSingleton<IMonitoringFacade, MonitoringFacade>()
                .AddTransient<MainWindowViewModel>();

            return services.BuildServiceProvider();
        }

        public App()
        {
            string dicPath = (GetAppsUseLightTheme() == 0) ? @"Resources\Dark.xaml" : @"Resources\Light.xaml";
            var dic = new ResourceDictionary
            {
                Source = new Uri(dicPath, UriKind.Relative)
            };
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(dic);


            AppSettingsManager.Intialization(Services.GetRequiredService<IConfigurationManager>());
        }


        /// 
        /// AppsUseLightTheme の値を取得する。
        /// ダークモード：0 ライトモード：1 値がないなどのエラー：-1
        /// using Microsoft.Win32;()
        /// 
        private int GetAppsUseLightTheme()
        {
            int getmode = -1;
            string rKeyName = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            string rGetValueName = "AppsUseLightTheme";
            try
            {
                var rKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(rKeyName);
                getmode = (int)(rKey?.GetValue(rGetValueName) ?? 1);

                // 開いたレジストリ・キーを閉じる
                rKey?.Close();
            }
            catch (NullReferenceException)
            {
            }
            return getmode;
        }
    }
}
