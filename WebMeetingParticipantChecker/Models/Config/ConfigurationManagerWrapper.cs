using System.Collections.Specialized;
using System.Configuration;

namespace WebMeetingParticipantChecker.Models.Config
{
    internal class ConfigurationManagerWrapper : IConfigurationManager
    {
        public NameValueCollection AppSettings => ConfigurationManager.AppSettings;

        public Configuration OpenExeConfiguration(ConfigurationUserLevel userLevel)
            => ConfigurationManager.OpenExeConfiguration(userLevel);
    }
}
