using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebMeetingParticipantChecker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using System.Collections.Specialized;
using WebMeetingParticipantChecker.Models.Config;
using Microsoft.Extensions.Configuration;

namespace WebMeetingParticipantChecker.ViewModels.Tests
{
    [TestClass()]
    public class SettingDialogViewModelTests
    {
        [TestMethod()]
        [TestCategory("設定画面VM作成")]
        public void 設定画面VM作成()
        {
            var moq = new Mock<IConfigurationRoot>();
            moq.SetupGet(x => x["MonitoringCycleMs"]).Returns("100");
            AppSettingsManager.Intialization(moq.Object);

            var target = new SettingDialogViewModel();

            Assert.AreEqual("100", target.MonitoringCycleMs);
            Assert.AreEqual("", target.ExistsNotAppliedData);
        }

        [TestMethod()]
        [TestCategory("設定適用")]
        public void 設定適用()
        {
            var moq = new Mock<IConfigurationRoot>();
            moq.SetupGet(x => x["MonitoringCycleMs"]).Returns("100");
            AppSettingsManager.Intialization(moq.Object);

            var target = new SettingDialogViewModel();

            Assert.AreEqual("100", target.MonitoringCycleMs);
            Assert.AreEqual("", target.ExistsNotAppliedData);

            target.MonitoringCycleMs = "200";

            target.ApplyCommand.Execute(null);

            Assert.AreEqual("200", target.MonitoringCycleMs);
            Assert.AreEqual("※ 変更適用後再起動されていません。", target.ExistsNotAppliedData);
        }
    }
}