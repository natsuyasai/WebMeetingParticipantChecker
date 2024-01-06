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
