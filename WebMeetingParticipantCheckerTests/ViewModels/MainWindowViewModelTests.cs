using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebMeetingParticipantChecker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using WebMeetingParticipantChecker.Models.Config;
using Moq;
using System.Collections.ObjectModel;
using WebMeetingParticipantChecker.Models.Monitoring;
using WebMeetingParticipantChecker.Models.Preset;
using Microsoft.Extensions.Configuration;

namespace WebMeetingParticipantChecker.ViewModels.Tests
{
    [TestClass()]
    public class MainWindowViewModelTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            var moq = new Mock<IConfigurationRoot>();
            moq.SetupGet(x => x["MonitoringCycleMs"]).Returns("100");
            AppSettingsManager.Intialization(moq.Object);
        }

        [TestMethod()]
        [TestCategory("プリセットデータ読み込み")]
        public async Task プリセットデータ読み込み()
        {
            var target = new MainWindowViewModel(new Mock<IMonitoring>().Object);

            await target.ReadPresetData();

            var targetFilePath1 = System.AppDomain.CurrentDomain.BaseDirectory + @"\Preset\" + "テンプレートプリセット1.csv";
            var expectedNames = new List<PresetInfo>()
            {
                new PresetInfo(0, targetFilePath1, "テンプレートプリセット1", new List<string>(){ "テンプレート1","テンプレート2"})
            };
            var expected = new ObservableCollection<PresetInfo>(expectedNames);
            Assert.AreEqual(expected[0].Id, target.PresetNames[0].Id);
            Assert.AreEqual(expected[0].Name, target.PresetNames[0].Name);
            Assert.AreEqual(expected[0].FilePath, target.PresetNames[0].FilePath);
            CollectionAssert.AreEqual(expected[0].Data.ToList(), target.PresetNames[0].Data.ToList());
        }

        [TestMethod()]
        [TestCategory("プリセット選択アイテム設定")]
        public async Task 選択したプリセットを保持する()
        {
            var target = new MainWindowViewModel(new Mock<IMonitoring>().Object);
            var selectedPreset = new PresetInfo(0, "", "テンプレートプリセット1", new List<string>() { "テンプレート1", "テンプレート2" });
            await target.ReadPresetData();

            target.SetSelectedPreset(selectedPreset);

            CollectionAssert.AreEqual(selectedPreset.Data.ToList(), target.SelectPresetData.ToList());
        }

        [TestMethod()]
        [TestCategory("監視開始")]
        public async Task 監視開始_捕捉中()
        {
            var moq = new Mock<IMonitoring>();
            moq.Setup(x => x.GetMonitoringInfos())
                .Returns(new List<MonitoringInfo>()
                {
                    new MonitoringInfo(0, "テンプレート1"),
                    new MonitoringInfo(1, "テンプレート2")
                });
            var target = new MainWindowViewModel(moq.Object);
            await target.ReadPresetData();

            target.StartCommand.Execute(null);

            Assert.AreEqual("対象ウィンドウ捕捉中(参加者リスト要素をクリックしてください)", target.StatusDisplayString);
            Assert.AreEqual("一時停止", target.PauseButtonString);
            Assert.IsFalse(target.CanStart);
            Assert.IsTrue(target.CanStop);
            Assert.IsFalse(target.CanPauseAndResume);

            Assert.AreEqual("テンプレート1", target.MonitoringInfos[0].Name);
            Assert.IsFalse(target.MonitoringInfos[0].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[0].IsManual);
            Assert.AreEqual("テンプレート2", target.MonitoringInfos[1].Name);
            Assert.IsFalse(target.MonitoringInfos[1].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[1].IsManual);
        }

        [TestMethod()]
        [TestCategory("監視開始")]
        public async Task 監視開始_Zoomウィンドウ捕捉完了_監視中状態()
        {
            var moq = new Mock<IMonitoring>();
            var target = new MainWindowViewModel(moq.Object);
            SetMonitoringFacadeMock(ref moq, target);
            moq.Setup(x => x.StartMonitoring(It.IsAny<Action>()))
                .Callback<Action>(action =>
                {
                });
            await target.ReadPresetData();

            target.StartCommand.Execute(null);

            Assert.AreEqual("監視中……(参加：0、未参加：2)", target.StatusDisplayString);
            Assert.AreEqual("一時停止", target.PauseButtonString);
            Assert.IsFalse(target.CanStart);
            Assert.IsTrue(target.CanStop);
            Assert.IsTrue(target.CanPauseAndResume);

            Assert.AreEqual("テンプレート1", target.MonitoringInfos[0].Name);
            Assert.IsFalse(target.MonitoringInfos[0].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[0].IsManual);
            Assert.AreEqual("テンプレート2", target.MonitoringInfos[1].Name);
            Assert.IsFalse(target.MonitoringInfos[1].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[1].IsManual);
        }

        [TestMethod()]
        [TestCategory("監視停止")]
        public async Task 監視停止()
        {
            var moq = new Mock<IMonitoring>();
            var target = new MainWindowViewModel(moq.Object);
            SetMonitoringFacadeMock(ref moq, target);
            await target.ReadPresetData();

            target.StopCommand.Execute(null);

            Assert.AreEqual("未監視", target.StatusDisplayString);
            Assert.AreEqual("一時停止", target.PauseButtonString);
            Assert.IsTrue(target.CanStart);
            Assert.IsFalse(target.CanStop);
            Assert.IsFalse(target.CanPauseAndResume);

            moq.Verify(x => x.StopMonitoring(), Times.Once);

            Assert.AreEqual("テンプレート1", target.MonitoringInfos[0].Name);
            Assert.IsFalse(target.MonitoringInfos[0].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[0].IsManual);
            Assert.AreEqual("テンプレート2", target.MonitoringInfos[1].Name);
            Assert.IsFalse(target.MonitoringInfos[1].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[1].IsManual);
        }

        [TestMethod()]
        [TestCategory("一時停止/再開")]
        public async Task 監視中に一時停止にして再開させる()
        {
            var moq = new Mock<IMonitoring>();
            var target = new MainWindowViewModel(moq.Object);
            SetMonitoringFacadeMock(ref moq, target);
            await target.ReadPresetData();
            target.StartCommand.Execute(null);


            // 一時停止
            target.PauseCommand.Execute(null);

            Assert.AreEqual("一時停止中(参加：0、未参加：2)", target.StatusDisplayString);
            Assert.AreEqual("再開", target.PauseButtonString);
            Assert.IsFalse(target.CanStart);
            Assert.IsTrue(target.CanStop);
            Assert.IsTrue(target.CanPauseAndResume);

            moq.Verify(x => x.Pause(), Times.Once);

            // 再開
            target.PauseCommand.Execute(null);

            Assert.AreEqual("監視中……(参加：0、未参加：2)", target.StatusDisplayString);
            Assert.AreEqual("一時停止", target.PauseButtonString);
            Assert.IsFalse(target.CanStart);
            Assert.IsTrue(target.CanStop);
            Assert.IsTrue(target.CanPauseAndResume);

            moq.Verify(x => x.Resume(), Times.Once);
        }

        [TestMethod()]
        [TestCategory("参加状態手動切り替え")]
        public async Task 監視中_参加状態を手動で参加状態に切り替え()
        {
            var moq = new Mock<IMonitoring>();
            var target = new MainWindowViewModel(moq.Object);
            SetMonitoringFacadeMock(ref moq, target);
            await target.ReadPresetData();
            target.StartCommand.Execute(null);


            Assert.AreEqual("テンプレート1", target.MonitoringInfos[0].Name);
            Assert.IsFalse(target.MonitoringInfos[0].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[0].IsManual);
            Assert.AreEqual("テンプレート2", target.MonitoringInfos[1].Name);
            Assert.IsFalse(target.MonitoringInfos[1].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[1].IsManual);
            Assert.AreEqual("監視中……(参加：0、未参加：2)", target.StatusDisplayString);

            target.SwitchingParticipantStateCommand.Execute(1);

            Assert.AreEqual("テンプレート1", target.MonitoringInfos[0].Name);
            Assert.IsFalse(target.MonitoringInfos[0].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[0].IsManual);
            Assert.AreEqual("テンプレート2", target.MonitoringInfos[1].Name);
            Assert.IsTrue(target.MonitoringInfos[1].IsJoin);
            Assert.IsTrue(target.MonitoringInfos[1].IsManual);
            Assert.AreEqual("監視中……(参加：1、未参加：1)", target.StatusDisplayString);
        }

        [TestMethod()]
        [TestCategory("参加状態手動切り替え")]
        public async Task 一時停止中_参加状態を手動で参加状態に切り替え()
        {
            var moq = new Mock<IMonitoring>();
            var target = new MainWindowViewModel(moq.Object);
            SetMonitoringFacadeMock(ref moq, target);
            await target.ReadPresetData();
            target.StartCommand.Execute(null);
            target.PauseCommand.Execute(null);


            Assert.AreEqual("テンプレート1", target.MonitoringInfos[0].Name);
            Assert.IsFalse(target.MonitoringInfos[0].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[0].IsManual);
            Assert.AreEqual("テンプレート2", target.MonitoringInfos[1].Name);
            Assert.IsFalse(target.MonitoringInfos[1].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[1].IsManual);
            Assert.AreEqual("一時停止中(参加：0、未参加：2)", target.StatusDisplayString);

            target.SwitchingParticipantStateCommand.Execute(1);

            Assert.AreEqual("テンプレート1", target.MonitoringInfos[0].Name);
            Assert.IsFalse(target.MonitoringInfos[0].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[0].IsManual);
            Assert.AreEqual("テンプレート2", target.MonitoringInfos[1].Name);
            Assert.IsTrue(target.MonitoringInfos[1].IsJoin);
            Assert.IsTrue(target.MonitoringInfos[1].IsManual);
            Assert.AreEqual("一時停止中(参加：1、未参加：1)", target.StatusDisplayString);
        }

        [TestMethod()]
        [TestCategory("参加状態を自動に切り替え")]
        public async Task 参加状態を自動に切り替え()
        {
            var moq = new Mock<IMonitoring>();
            var target = new MainWindowViewModel(moq.Object);
            SetMonitoringFacadeMock(ref moq, target);
            await target.ReadPresetData();
            target.StartCommand.Execute(null);


            Assert.AreEqual("テンプレート1", target.MonitoringInfos[0].Name);
            Assert.IsFalse(target.MonitoringInfos[0].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[0].IsManual);
            Assert.AreEqual("テンプレート2", target.MonitoringInfos[1].Name);
            Assert.IsFalse(target.MonitoringInfos[1].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[1].IsManual);
            Assert.AreEqual("監視中……(参加：0、未参加：2)", target.StatusDisplayString);

            // 手動で参加にする
            target.SwitchingParticipantStateCommand.Execute(1);

            Assert.AreEqual("テンプレート1", target.MonitoringInfos[0].Name);
            Assert.IsFalse(target.MonitoringInfos[0].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[0].IsManual);
            Assert.AreEqual("テンプレート2", target.MonitoringInfos[1].Name);
            Assert.IsTrue(target.MonitoringInfos[1].IsJoin);
            Assert.IsTrue(target.MonitoringInfos[1].IsManual);
            Assert.AreEqual("監視中……(参加：1、未参加：1)", target.StatusDisplayString);

            // 自動に戻す
            target.SetParticipantAutoCommand.Execute(1);

            Assert.AreEqual("テンプレート1", target.MonitoringInfos[0].Name);
            Assert.IsFalse(target.MonitoringInfos[0].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[0].IsManual);
            Assert.AreEqual("テンプレート2", target.MonitoringInfos[1].Name);
            Assert.IsFalse(target.MonitoringInfos[1].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[1].IsManual);
            Assert.AreEqual("監視中……(参加：0、未参加：2)", target.StatusDisplayString);
        }

        [TestMethod()]
        [TestCategory("参加監視")]
        public async Task 全員参加()
        {
            var moq = new Mock<IMonitoring>();
            var target = new MainWindowViewModel(moq.Object);
            SetMonitoringFacadeMock(ref moq, target);
            await target.ReadPresetData();
            target.StartCommand.Execute(null);

            Assert.AreEqual("テンプレート1", target.MonitoringInfos[0].Name);
            Assert.IsFalse(target.MonitoringInfos[0].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[0].IsManual);
            Assert.AreEqual("テンプレート2", target.MonitoringInfos[1].Name);
            Assert.IsFalse(target.MonitoringInfos[1].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[1].IsManual);
            Assert.AreEqual("監視中……(参加：0、未参加：2)", target.StatusDisplayString);
            Assert.AreEqual("一時停止", target.PauseButtonString);
            Assert.IsFalse(target.CanStart);
            Assert.IsTrue(target.CanStop);
            Assert.IsTrue(target.CanPauseAndResume);

            target.SwitchingParticipantStateCommand.Execute(0);
            target.SwitchingParticipantStateCommand.Execute(1);

            Assert.AreEqual("テンプレート1", target.MonitoringInfos[0].Name);
            Assert.IsTrue(target.MonitoringInfos[0].IsJoin);
            Assert.IsTrue(target.MonitoringInfos[0].IsManual);
            Assert.AreEqual("テンプレート2", target.MonitoringInfos[1].Name);
            Assert.IsTrue(target.MonitoringInfos[1].IsJoin);
            Assert.IsTrue(target.MonitoringInfos[1].IsManual);

            Assert.AreEqual("対象者参加済み", target.StatusDisplayString);
            Assert.AreEqual("一時停止", target.PauseButtonString);
            Assert.IsTrue(target.CanStart);
            Assert.IsFalse(target.CanStop);
            Assert.IsFalse(target.CanPauseAndResume);
        }

        private void SetMonitoringFacadeMock(ref Mock<IMonitoring> moq, MainWindowViewModel target)
        {
            moq.Setup(x => x.SelectZoomParticipantElement(It.IsAny<MonitoringType.Target>(), It.IsAny<Action>()))
                .Callback<MonitoringType.Target, Action>((_, action) =>
                {
                    action();
                });
            moq.Setup(x => x.StartMonitoring(It.IsAny<Action>()))
                .Callback<Action>(action =>
                {
                    action();
                });
            moq.Setup(x => x.GetMonitoringInfos())
                .Returns(new List<MonitoringInfo>()
                {
                    new MonitoringInfo(0, "テンプレート1"),
                    new MonitoringInfo(1, "テンプレート2")
                });


            moq.Setup(x => x.SwitchingParticipantState(It.IsAny<int>()))
                .Callback<int>(targetId =>
                {
                    foreach (var item in target.MonitoringInfos)
                    {
                        if (item.Id == targetId)
                        {
                            item.SwitchJoinStateOfManual();
                        }
                    }
                });
            moq.Setup(x => x.SetParticipantAuto(It.IsAny<int>()))
                .Callback<int>(targetId =>
                {
                    foreach (var item in target.MonitoringInfos)
                    {
                        if (item.Id == targetId)
                        {
                            item.ResetJoinState();
                        }
                    }
                });
            moq.Setup(x => x.IsAllJoin())
                .Returns(() =>
                {
                    return target.MonitoringInfos.All(item => item.IsJoin);
                });
        }
    }
}