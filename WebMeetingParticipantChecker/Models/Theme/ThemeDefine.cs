using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMeetingParticipantChecker.Models.Theme
{
    internal static class ThemeDefine
    {
        public static readonly IEnumerable<Theme> ThemeDefault = new List<Theme>() { new(0, "Dark"), new(1, "Light"), new(2, "Auto") };
        public static readonly int MinThemeId = 0;
        public static readonly int MaxThemeId = 2;

        public static bool IsContaine(int id)
        {
            return (id >= MinThemeId && id <= MaxThemeId);
        }
    }
}
