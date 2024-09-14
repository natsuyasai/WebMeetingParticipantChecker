using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.IO;
using System.Windows;
using WebMeetingParticipantChecker.Models.Config;
using WebMeetingParticipantChecker.Models.FileWriter;
using WebMeetingParticipantChecker.Models.Monitoring;
using WebMeetingParticipantChecker.Models.Preset;
using WebMeetingParticipantChecker.Models.Theme;
using WebMeetingParticipantChecker.Models.UIAutomation.Utils;
using WebMeetingParticipantChecker.ViewModels;
using Auto = WebMeetingParticipantChecker.Models.UIAutomation.TargetElementGetter.Auto;
using Manual = WebMeetingParticipantChecker.Models.UIAutomation.TargetElementGetter.Manual;

namespace WebMeetingParticipantChecker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    partial class App : Application
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static IServiceProvider Services { get; } = ConfigureServices();

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddTransient<IKeyEventSender, ArrowKeyEventSender>()
                .AddTransient(provider => 
                    new Auto.IAutomationElementGetter[] {
                        new Auto.AutomationElementGetterForZoom(AppSettingsManager.ZoomRootName,AppSettingsManager.ZoomParticipantListRootName ,AppSettingsManager.ZoomParticipantListName),
                        new Auto.AutomationElementGetterForTeams(AppSettingsManager.TeamsRootName, AppSettingsManager.TeamsParticipantListName) })
                .AddTransient(provider =>
                    new Manual.IAutomationElementGetter[] {
                        new Manual.AutomationElementGetterForZoom(AppSettingsManager.ZoomParticipantListName),
                        new Manual.AutomationElementGetterForTeams(AppSettingsManager.TeamsParticipantListName) })
                .AddTransient<IMonitoringResultExportable, MonitoringResultExporter>()
                .AddTransient<MonitoringModel>(provider => new MonitoringModel(AppSettingsManager.MonitoringCycleMs))
                .AddSingleton<IPresetProvider, PresetModel>() // プリセット情報はシステムで一意とする
                .AddSingleton<IReadOnlyPreset>(provider => provider.GetService<IPresetProvider>()!) // 読み取り専用プリセット情報
                .AddTransient<PresetViewModel>()
                .AddTransient<MonitoringViewModel>(provider => 
                    new MonitoringViewModel(
                        provider.GetService<Auto.IAutomationElementGetter[]>()!,
                        provider.GetService<Manual.IAutomationElementGetter[]>()!,
                        provider.GetService<MonitoringModel>()!, 
                        provider.GetService<IKeyEventSender>()!,
                        provider.GetService<IReadOnlyPreset>()!,
                        provider.GetService<IMonitoringResultExportable>()!,
                        AppSettingsManager.KeydownMaxCount))
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
            var currentTheme = GetAppsUseLightTheme();
            string dicPath = (currentTheme == 0) ? @"Resources\Dark.xaml" : @"Resources\Light.xaml";
            AppSettingsManager.CurrentThemeId = currentTheme;
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
            var currentThemeId = AppSettingsManager.ThemeIdNotReturnDefault;
            if (currentThemeId != null && ThemeDefine.IsDefaultThemeValue((int)currentThemeId))
            {
                return (int)currentThemeId;
            }

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
