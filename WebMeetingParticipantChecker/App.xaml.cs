using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.IO;
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
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public static IServiceProvider Services { get; } = ConfigureServices();

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
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
            _logger.Info("起動");
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
        private static int GetAppsUseLightTheme()
        {
#pragma warning disable CA1416 // プラットフォームの互換性を検証
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
#pragma warning restore CA1416 // プラットフォームの互換性を検証
        }
    }
}
