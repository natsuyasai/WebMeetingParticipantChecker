using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
