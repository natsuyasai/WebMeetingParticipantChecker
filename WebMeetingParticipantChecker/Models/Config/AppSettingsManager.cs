using Microsoft.Extensions.Configuration;

namespace WebMeetingParticipantChecker.Models.Config
{
    internal static class AppSettingsManager
    {
        private static IConfigurationRoot? _configuration;

        public static void Intialization(IConfigurationRoot configurationRoot)
        {
            _configuration = configurationRoot;
        }
        /// <summary>
        /// 監視周期
        /// </summary>
        public static int MonitoringCycleMs
        {
            get
            {
                if (int.TryParse(_configuration?["MonitoringCycleMs"], out var value))
                {
                    return value;
                }
                return 2000;
            }
        }

        /// <summary>
        /// 下キー入力上限(フェールセーフ)
        /// </summary>
        public static int KyedownMaxCount
        {
            get
            {
                if (int.TryParse(_configuration?["KyedownMaxCount"], out var value))
                {
                    return value;
                }
                return 500;
            }
        }

        /// <summary>
        /// 参加者リスト名
        /// </summary>
        public static string ZoomParticipantListName
        {
            get
            {
                return _configuration?["ZoomParticipantListName"] ?? "参加者リスト";
            }
        }

        /// <summary>
        /// 参加者リスト名
        /// </summary>
        public static string TeamsParticipantListName
        {
            get
            {
                return _configuration?["TeamsParticipantListName"] ?? "出席者";
            }
        }
    }
}
