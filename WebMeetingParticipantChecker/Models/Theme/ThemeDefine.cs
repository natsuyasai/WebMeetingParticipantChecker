using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMeetingParticipantChecker.Models.Theme
{
    internal static class ThemeDefine
    {
        internal enum ThmeValue
        {
            Dark = 0,
            Light,
            Auto,
        }

        public static readonly IEnumerable<Theme> ThemeDefault = new List<Theme>() { new((int)ThmeValue.Dark, "Dark"), new((int)ThmeValue.Light, "Light"), new((int)ThmeValue.Auto, "Auto") };
        public static readonly int MinThemeId = 0;
        public static readonly int MaxThemeId = 2;

        public static bool IsContaine(int id)
        {
            return (id >= MinThemeId && id <= MaxThemeId);
        }

        public static bool IsDefaultThemeValue(int currentThemeId)
        {
            return (currentThemeId == 0 || currentThemeId == 1);
        }
    }
}
