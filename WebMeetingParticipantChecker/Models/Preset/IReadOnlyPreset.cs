using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMeetingParticipantChecker.Models.Preset
{
    internal interface IReadOnlyPreset
    {
        IEnumerable<string> GetCurrentPresetDataList();
        string GetCurrntPresetFilePath();
        IEnumerable<PresetInfo> GetPreset();
        IEnumerable<string> GetPresetDataList(int index);
        IEnumerable<string> GetPresetNameList();
    }
}
