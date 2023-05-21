using System.Configuration;

namespace WebMeetingParticipantChecker.Models.Config
{
    internal static class AppSettingsManager
    {
        private static IConfigurationManager? _configurationManager;

        public static void Intialization(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }
        /// <summary>
        /// 監視周期
        /// </summary>
        public static int MonitoringCycleMs
        {
            get
            {
                if (int.TryParse(_configurationManager?.AppSettings["MonitoringCycleMs"], out var value))
                {
                    return value;
                }
                return 2000;
            }
            set
            {
                SetSetting("MonitoringCycleMs", value.ToString());
            }
        }

        /// <summary>
        /// 下キー入力上限(フェールセーフ)
        /// </summary>
        public static int KyedownMaxCount
        {
            get
            {
                if (int.TryParse(_configurationManager?.AppSettings["KyedownMaxCount"], out var value))
                {
                    return value;
                }
                return 200;
            }
            set
            {
                SetSetting("KyedownMaxCount", value.ToString());
            }
        }

        /// <summary>
        /// 参加者リスト名
        /// </summary>
        public static string ParticipantListName
        {
            get
            {
                return _configurationManager?.AppSettings["ParticipantListName"] ?? "";
            }
            set
            {
                SetSetting("ParticipantListName", value);
            }
        }

        /// <summary>
        /// 設定更新
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetSetting(string key, string value)
        {
            var config = _configurationManager?.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config == null)
            {
                return;
            }
            config.AppSettings.Settings[key].Value = value;
            config?.Save();
        }
    }
}
