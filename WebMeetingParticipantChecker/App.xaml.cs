using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using WebMeetingParticipantChecker.Models.Config;
using WebMeetingParticipantChecker.Models.Monitoring;
using WebMeetingParticipantChecker.Models.UIAutomation;
using WebMeetingParticipantChecker.ViewModels;
using WebMeetingParticipantChecker.Views;

namespace WebMeetingParticipantChecker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    partial class App : Application
    {
        public static IServiceProvider Services { get; } = ConfigureServices();

        private static readonly ILoggerFactory _loggerFactory 
            = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Information));

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton<ILogger>(a => _loggerFactory.CreateLogger<App>())
                .AddSingleton<AutomationElementGetter[]>(
                provider => new AutomationElementGetter[] {
                    new AutomationElementGetterForZoom(),
                    new AutomationElementGetterForTeams() })
                .AddSingleton<MonitoringModel>()
                .AddSingleton<IMonitoring, MonitoringService>()
                .AddTransient<MainWindowViewModel>();

            return services.BuildServiceProvider();
        }

        public App()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(ConfigDefine.FileName)
                .Build();
            AppSettingsManager.Intialization(config);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // テーマ更新
            string dicPath = (GetAppsUseLightTheme() == 0) ? @"Resources\Dark.xaml" : @"Resources\Light.xaml";
            var dic = new ResourceDictionary
            {
                Source = new Uri(dicPath, UriKind.Relative)
            };
            Current.Resources.MergedDictionaries.Clear();
            Current.Resources.MergedDictionaries.Add(dic);
            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(dic);
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
