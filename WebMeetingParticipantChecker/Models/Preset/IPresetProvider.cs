using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebMeetingParticipantChecker.Models.Preset
{
    internal interface IPresetProvider : IReadOnlyPreset
    {
        void Clear();
        Task<bool> ReadPresetData(string rootPath, string targetFolderName);
        void UpdateCurrentIndex(int id);
    }
}