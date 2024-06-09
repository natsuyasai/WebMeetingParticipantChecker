using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebMeetingParticipantChecker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using System.Collections.ObjectModel;
using WebMeetingParticipantChecker.Models.Monitoring;
using WebMeetingParticipantChecker.Models.Preset;

namespace WebMeetingParticipantChecker.ViewModels.Tests
{
    [TestClass()]
    public class PresetViewModelTests
    {
        [TestMethod()]
        [TestCategory("プリセットデータ読み込み")]
        public async Task プリセットデータ読み込み()
        {
            var preset = new PresetModel();
            var target = new PresetViewModel(preset);

            await target.ReadPresetData();

            var targetFilePath1 = System.AppDomain.CurrentDomain.BaseDirectory + @"\Preset\" + "テンプレートプリセット1.csv";
            var expectedNames = new List<PresetInfo>()
            {
                new(0, targetFilePath1, "テンプレートプリセット1", new List<string>(){ "テンプレート1","テンプレート2"})
            };
            var expected = new ObservableCollection<PresetInfo>(expectedNames);
            Assert.AreEqual(expected[0].Id, target.PresetNames[0].Id);
            Assert.AreEqual(expected[0].Name, target.PresetNames[0].Name);
            Assert.AreEqual(expected[0].FilePath, target.PresetNames[0].FilePath);
            CollectionAssert.AreEqual(expected[0].UserNames.ToList(), target.PresetNames[0].UserNames.ToList());
        }

        [TestMethod()]
        [TestCategory("プリセット選択アイテム設定")]
        public async Task 選択したプリセットを保持する()
        {
            var preset = new PresetModel();
            var target = new PresetViewModel(preset);
            var selectedPreset = new PresetInfo(0, "", "テンプレートプリセット1", new List<string>() { "テンプレート1", "テンプレート2" });
            await target.ReadPresetData();

            target.SetSelectedPreset(selectedPreset);

            CollectionAssert.AreEqual(selectedPreset.UserNames.ToList(), target.SelectPresetUsers.ToList());
        }
    }
}