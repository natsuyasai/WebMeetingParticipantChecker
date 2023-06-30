using System.IO;

namespace WebMeetingParticipantChecker.Models.Config
{
    internal static class ConfigDefine
    {
        public static string FileName { get; } = "usersettings.json";

        public static string GetFileNameForFullPath()
        {
            return System.IO.Path.Join(Directory.GetCurrentDirectory(), FileName);
        }
    }
}
