using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMeetingParticipantChecker.Models.Preset
{
    internal interface IReadOnlyPreset
    {
        IEnumerable<string> GetCurrentPresetUsers();
        string GetCurrntPresetFilePath();
        IEnumerable<PresetInfo> GetPreset();
        IEnumerable<string> GetPresetUsers(int index);
        IEnumerable<string> GetPresetNames();
    }
}
