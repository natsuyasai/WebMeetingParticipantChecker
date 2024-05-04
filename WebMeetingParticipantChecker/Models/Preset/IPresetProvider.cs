using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebMeetingParticipantChecker.Models.Preset
{
    internal interface IPresetProvider
    {
        void Clear();
        IEnumerable<string> GetCurrentPresetDataList();
        string GetCurrntPresetFilePath();
        IEnumerable<PresetInfo> GetPreset();
        IEnumerable<string> GetPresetDataList(int index);
        IEnumerable<string> GetPresetNameList();
        Task<bool> ReadPresetData(string rootPath, string targetFolderName);
        void UpdateCurrentIndex(int id);
    }
}