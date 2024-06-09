using Microsoft.Extensions.Configuration;
using WebMeetingParticipantChecker.Models.Theme;

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
        public static int KeydownMaxCount
        {
            get
            {
                if (int.TryParse(_configuration?["KeydownMaxCount"], out var value))
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
                if (string.IsNullOrEmpty(_configuration?["ZoomParticipantListName"]))
                {
                    return "参加者リスト";
                }
                return _configuration["ZoomParticipantListName"]!;
            }
        }

        /// <summary>
        /// 参加者リスト名
        /// </summary>
        public static string TeamsParticipantListName
        {
            get
            {
                if (string.IsNullOrEmpty(_configuration?["TeamsParticipantListName"]))
                {
                    return "出席者";
                }
                return _configuration["TeamsParticipantListName"]!;
            }
        }

        /// <summary>
        /// テーマID（補正あり）
        /// </summary>
        public static int ThemeId
        {
            get
            {
                if (int.TryParse(_configuration?["ThemeId"], out var value))
                {
                    return value;
                }
                return ThemeDefine.MaxThemeId;
            }
        }

        /// <summary>
        /// テーマID
        /// 補正なし。データがなければnull
        /// </summary>
        public static int? ThemeIdNotReturnDefault
        {
            get
            {
                if (int.TryParse(_configuration?["ThemeId"], out var value))
                {
                    return value;
                }
                return null;
            }
        }

        /// <summary>
        /// 現在のテーマ
        /// </summary>
        private static int? _currentTheme = null;
        public static int? CurrentThemeId
        {
            get
            {
                return _currentTheme;
            }
            set
            {
                _currentTheme ??= value;
            }
        }
    }
}
