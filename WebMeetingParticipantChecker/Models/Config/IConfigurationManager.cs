using System.Collections.Specialized;
using System.Configuration;

namespace WebMeetingParticipantChecker.Models.Config
{
    public interface IConfigurationManager
    {
        NameValueCollection AppSettings
        {
            get;
        }

        Configuration OpenExeConfiguration(ConfigurationUserLevel userLevel);
    }
}