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
    public class MonitoringViewModelTests
    {
        private readonly Mock<IPresetProvider> _presetMoq;

        public MonitoringViewModelTests()
        {
            _presetMoq = new Mock<IPresetProvider>();
            _presetMoq.Setup(x => x.GetCurrentPresetDataList())
                .Returns(new List<string>() { "テンプレート1", "テンプレート2" });
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var moq = new Mock<IConfigurationRoot>();
            moq.SetupGet(x => x["MonitoringCycleMs"]).Returns("100");
            AppSettingsManager.Intialization(moq.Object);
        }

        [TestMethod()]
        [TestCategory("監視開始")]
        public void 監視開始_捕捉中()
        {
            var moq = new Mock<IMonitoring>();
            moq.Setup(x => x.GetMonitoringInfos())
                .Returns(new List<MonitoringInfo>()
                {
                    new(0, "テンプレート1"),
                    new(1, "テンプレート2")
                });

            var target = new MonitoringViewModel(moq.Object, _presetMoq.Object);

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
        public void 監視開始_Zoomウィンドウ捕捉完了_監視中状態()
        {
            var moq = new Mock<IMonitoring>();
            var target = new MonitoringViewModel(moq.Object, _presetMoq.Object);
            SetMonitoringFacadeMock(ref moq, target);
            moq.Setup(x => x.StartMonitoring(It.IsAny<Action>()))
                .Callback<Action>(action =>
                {
                });

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
        public void 監視停止()
        {
            var moq = new Mock<IMonitoring>();
            var target = new MonitoringViewModel(moq.Object, _presetMoq.Object);
            SetMonitoringFacadeMock(ref moq, target);

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
        public void 監視中に一時停止にして再開させる()
        {
            var moq = new Mock<IMonitoring>();
            var target = new MonitoringViewModel(moq.Object, _presetMoq.Object);
            SetMonitoringFacadeMock(ref moq, target);
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
        public void 監視中_参加状態を手動で参加状態に切り替え()
        {
            var moq = new Mock<IMonitoring>();
            var target = new MonitoringViewModel(moq.Object, _presetMoq.Object);
            SetMonitoringFacadeMock(ref moq, target);
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
        public void 一時停止中_参加状態を手動で参加状態に切り替え()
        {
            var moq = new Mock<IMonitoring>();
            var target = new MonitoringViewModel(moq.Object, _presetMoq.Object);
            SetMonitoringFacadeMock(ref moq, target);
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
        public void 参加状態を自動に切り替え()
        {
            var moq = new Mock<IMonitoring>();
            var target = new MonitoringViewModel(moq.Object, _presetMoq.Object);
            SetMonitoringFacadeMock(ref moq, target);
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
            var target = new MonitoringViewModel(moq.Object, _presetMoq.Object);
            SetMonitoringFacadeMock(ref moq, target);
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

            // 監視完了直後は"対象者参加済み"状態だが、すぐに未監視に更新される
            // Task内で実行されるため、テストのタイミング次第でどちらかの状態になってしまう
            // そのため一時的にテストで待ちを発生させることで、確実に未監視状態になってから検証を行うようにする
            await Task.Delay(0);
            Assert.AreEqual("未監視", target.StatusDisplayString);
            Assert.AreEqual("一時停止", target.PauseButtonString);
            Assert.IsTrue(target.CanStart);
            Assert.IsFalse(target.CanStop);
            Assert.IsFalse(target.CanPauseAndResume);
        }

        private void SetMonitoringFacadeMock(ref Mock<IMonitoring> moq, MonitoringViewModel target)
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
                    new(0, "テンプレート1"),
                    new(1, "テンプレート2")
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