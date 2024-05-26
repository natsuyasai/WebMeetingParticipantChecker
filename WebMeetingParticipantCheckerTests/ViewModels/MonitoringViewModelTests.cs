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
using WebMeetingParticipantChecker.Models.UIAutomation;
using UIAutomationClient;

namespace WebMeetingParticipantChecker.ViewModels.Tests
{
    [TestClass()]
    public class MonitoringViewModelTests
    {
        private readonly Mock<IPresetProvider> _presetMoq;
        private readonly Mock<IKeyEventSender> _keyEventSender;
        private readonly Mock<IAutomationElementGetter> _elementGetter;
        private readonly MonitoringModel _monitoringModel = new MonitoringModel();

        public MonitoringViewModelTests()
        {
            _presetMoq = new Mock<IPresetProvider>();
            _keyEventSender = new Mock<IKeyEventSender>();

            _elementGetter = new Mock<IAutomationElementGetter>();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var moq = new Mock<IConfigurationRoot>();
            moq.SetupGet(x => x["MonitoringCycleMs"]).Returns("100");
            AppSettingsManager.Intialization(moq.Object);

            _presetMoq.Setup(x => x.GetCurrentPresetDataList())
                .Returns(new List<string>() { "テンプレート1", "テンプレート2" });

            _elementGetter.Setup(x => x.SubscribeToFocusChange(It.IsAny<Action>()))
                .Callback<Action>((action) =>
                {
                    action();
                });
            _elementGetter.Setup(x => x.GetTargetElement()).Returns(new Mock<IUIAutomationElement>().Object);

            _monitoringModel.RegisterMonitoringTargets(new List<string>() { "テンプレート1", "テンプレート2" });
        }

        [TestMethod()]
        [TestCategory("監視開始")]
        public async Task 監視開始すると捕捉中状態となること()
        {
            _elementGetter.Setup(x => x.SubscribeToFocusChange(It.IsAny<Action>()))
                .Callback<Action>((action) =>
                {
                });
            var target = new MonitoringViewModel(
                new IAutomationElementGetter[] { _elementGetter.Object, _elementGetter.Object },
                _monitoringModel, _keyEventSender.Object, _presetMoq.Object);

            target.StartCommand.Execute(null);
            await Task.Delay(100);

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
        public async Task 監視対象要素の捕捉完了後監視中状態となること()
        {
            var target = new MonitoringViewModel(
                new IAutomationElementGetter[] { _elementGetter.Object, _elementGetter.Object },
                _monitoringModel, _keyEventSender.Object, _presetMoq.Object);

            target.StartCommand.Execute(null);
            await Task.Delay(100);

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
        public async Task 監視停止を行うと初期状態に戻ること()
        {
            var target = new MonitoringViewModel(
                new IAutomationElementGetter[] { _elementGetter.Object, _elementGetter.Object },
                _monitoringModel, _keyEventSender.Object, _presetMoq.Object);

            target.StartCommand.Execute(null);
            await Task.Delay(100);

            Assert.AreEqual("監視中……(参加：0、未参加：2)", target.StatusDisplayString);

            target.StopCommand.Execute(null);
            await Task.Delay(100);

            Assert.AreEqual("未監視", target.StatusDisplayString);
            Assert.AreEqual("一時停止", target.PauseButtonString);
            Assert.IsTrue(target.CanStart);
            Assert.IsFalse(target.CanStop);
            Assert.IsFalse(target.CanPauseAndResume);

            Assert.AreEqual("テンプレート1", target.MonitoringInfos[0].Name);
            Assert.IsFalse(target.MonitoringInfos[0].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[0].IsManual);
            Assert.AreEqual("テンプレート2", target.MonitoringInfos[1].Name);
            Assert.IsFalse(target.MonitoringInfos[1].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[1].IsManual);
        }

        [TestMethod()]
        [TestCategory("一時停止/再開")]
        public async Task 監視中に一時停止とすると一時停止状態となりその後再開させると監視中状態に戻ること()
        {
            var target = new MonitoringViewModel(
                new IAutomationElementGetter[] { _elementGetter.Object, _elementGetter.Object },
                _monitoringModel, _keyEventSender.Object, _presetMoq.Object);

            target.StartCommand.Execute(null);
            await Task.Delay(100);

            // 一時停止
            target.PauseCommand.Execute(null);
            await Task.Delay(100);

            Assert.AreEqual("一時停止中(参加：0、未参加：2)", target.StatusDisplayString);
            Assert.AreEqual("再開", target.PauseButtonString);
            Assert.IsFalse(target.CanStart);
            Assert.IsTrue(target.CanStop);
            Assert.IsTrue(target.CanPauseAndResume);

            // 再開
            target.PauseCommand.Execute(null);
            await Task.Delay(100);

            Assert.AreEqual("監視中……(参加：0、未参加：2)", target.StatusDisplayString);
            Assert.AreEqual("一時停止", target.PauseButtonString);
            Assert.IsFalse(target.CanStart);
            Assert.IsTrue(target.CanStop);
            Assert.IsTrue(target.CanPauseAndResume);
        }

        [TestMethod()]
        [TestCategory("参加状態手動切り替え")]
        public async Task 監視中に参加状態を手動で参加状態に切り替えたとき切り替えた対象が参加状態となること()
        {
            var target = new MonitoringViewModel(
                new IAutomationElementGetter[] { _elementGetter.Object, _elementGetter.Object },
                _monitoringModel, _keyEventSender.Object, _presetMoq.Object);

            target.StartCommand.Execute(null);
            await Task.Delay(100);

            Assert.AreEqual("テンプレート1", target.MonitoringInfos[0].Name);
            Assert.IsFalse(target.MonitoringInfos[0].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[0].IsManual);
            Assert.AreEqual("テンプレート2", target.MonitoringInfos[1].Name);
            Assert.IsFalse(target.MonitoringInfos[1].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[1].IsManual);
            Assert.AreEqual("監視中……(参加：0、未参加：2)", target.StatusDisplayString);

            // 手動で切り替え
            target.SwitchingParticipantStateCommand.Execute(1);
            await Task.Delay(100);

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
        public async Task 一時停止中_参加状態を手動で参加状態に切り替えたとき切り替えた対象が参加状態となること()
        {
            var target = new MonitoringViewModel(
                new IAutomationElementGetter[] { _elementGetter.Object, _elementGetter.Object },
                _monitoringModel, _keyEventSender.Object, _presetMoq.Object);

            target.StartCommand.Execute(null);
            await Task.Delay(100);
            target.PauseCommand.Execute(null);
            await Task.Delay(100);

            Assert.AreEqual("テンプレート1", target.MonitoringInfos[0].Name);
            Assert.IsFalse(target.MonitoringInfos[0].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[0].IsManual);
            Assert.AreEqual("テンプレート2", target.MonitoringInfos[1].Name);
            Assert.IsFalse(target.MonitoringInfos[1].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[1].IsManual);
            Assert.AreEqual("一時停止中(参加：0、未参加：2)", target.StatusDisplayString);

            target.SwitchingParticipantStateCommand.Execute(1);
            await Task.Delay(100);

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
        public async Task 参加状態を自動に切り替えたとき手動で設定した状態がリセットされること()
        {
            var target = new MonitoringViewModel(
                new IAutomationElementGetter[] { _elementGetter.Object, _elementGetter.Object },
                _monitoringModel, _keyEventSender.Object, _presetMoq.Object);

            target.StartCommand.Execute(null);
            await Task.Delay(100);

            Assert.AreEqual("テンプレート1", target.MonitoringInfos[0].Name);
            Assert.IsFalse(target.MonitoringInfos[0].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[0].IsManual);
            Assert.AreEqual("テンプレート2", target.MonitoringInfos[1].Name);
            Assert.IsFalse(target.MonitoringInfos[1].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[1].IsManual);
            Assert.AreEqual("監視中……(参加：0、未参加：2)", target.StatusDisplayString);

            // 手動で参加にする
            target.SwitchingParticipantStateCommand.Execute(1);
            await Task.Delay(100);

            Assert.AreEqual("テンプレート1", target.MonitoringInfos[0].Name);
            Assert.IsFalse(target.MonitoringInfos[0].IsJoin);
            Assert.IsFalse(target.MonitoringInfos[0].IsManual);
            Assert.AreEqual("テンプレート2", target.MonitoringInfos[1].Name);
            Assert.IsTrue(target.MonitoringInfos[1].IsJoin);
            Assert.IsTrue(target.MonitoringInfos[1].IsManual);
            Assert.AreEqual("監視中……(参加：1、未参加：1)", target.StatusDisplayString);

            // 自動に戻す
            target.SetParticipantAutoCommand.Execute(1);
            await Task.Delay(100);

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
        public async Task 監視中に全員参加状態となったとき初期状態に戻ること()
        {
            var target = new MonitoringViewModel(
                new IAutomationElementGetter[] { _elementGetter.Object, _elementGetter.Object },
                _monitoringModel, _keyEventSender.Object, _presetMoq.Object);

            target.StartCommand.Execute(null);
            await Task.Delay(100);

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
            await Task.Delay(100);
            target.SwitchingParticipantStateCommand.Execute(1);
            await Task.Delay(100);

            Assert.AreEqual("テンプレート1", target.MonitoringInfos[0].Name);
            Assert.IsTrue(target.MonitoringInfos[0].IsJoin);
            Assert.IsTrue(target.MonitoringInfos[0].IsManual);
            Assert.AreEqual("テンプレート2", target.MonitoringInfos[1].Name);
            Assert.IsTrue(target.MonitoringInfos[1].IsJoin);
            Assert.IsTrue(target.MonitoringInfos[1].IsManual);

            await Task.Delay(100);
            Assert.AreEqual("未監視", target.StatusDisplayString);
            Assert.AreEqual("一時停止", target.PauseButtonString);
            Assert.IsTrue(target.CanStart);
            Assert.IsFalse(target.CanStop);
            Assert.IsFalse(target.CanPauseAndResume);
        }
    }
}