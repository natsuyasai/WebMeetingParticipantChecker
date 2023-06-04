using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebMeetingParticipantChecker.Models.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using System.Collections.Specialized;
using WebMeetingParticipantChecker.Models.Config;
using WebMeetingParticipantChecker.Models.UIAutomation;
using Microsoft.Extensions.Configuration;

namespace WebMeetingParticipantChecker.Models.Monitoring.Tests
{
    [TestClass()]
    public class MonitoringModelTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            var moq = new Mock<IConfigurationRoot>();
            moq.SetupGet(x => x["MonitoringCycleMs"]).Returns("100");
            AppSettingsManager.Intialization(moq.Object);
        }

        [TestMethod()]
        [TestCategory("監視対象登録")]
        public void 監視対象登録_姓名入れ替えケース含む()
        {
            var target = new MonitoringModel();

            var targetUsers = new List<string>()
            {
                "ユーザ 1",
                "ユーザ 2",
                "ユーザ 3",
            };
            target.RegisterMonitoringTargets(targetUsers);

            var monitoringInfos = target.GetMonitoringInfos().ToList();
            Assert.AreEqual(3, monitoringInfos.Count());
            Assert.AreEqual(targetUsers[0], monitoringInfos[0].Name);
            Assert.AreEqual(targetUsers[1], monitoringInfos[1].Name);
            Assert.AreEqual(targetUsers[2], monitoringInfos[2].Name);

            //var privateObject = new PrivateObject(target);
            //var searchInfos = privateObject.GetFieldOrProperty("_searchInfos") as List<MonitoringInfo>;
            //Assert.AreEqual(6, searchInfos.Count);
            //Assert.AreEqual("ユーザ1", searchInfos[0].Name);
            //Assert.AreEqual("ユーザ2", searchInfos[1].Name);
            //Assert.AreEqual("ユーザ3", searchInfos[2].Name);
            //Assert.AreEqual("1ユーザ", searchInfos[3].Name);
            //Assert.AreEqual("2ユーザ", searchInfos[4].Name);
            //Assert.AreEqual("3ユーザ", searchInfos[5].Name);
        }

        [TestMethod()]
        [TestCategory("監視開始")]
        public async Task 監視開始_全員参加()
        {
            var target = new MonitoringModel();
            bool isCalled = false;
            void callback() { isCalled = true; }
            var retDict = new Dictionary<string, string>() {
                { "ユーザ1","" },
                { "ユーザ2","" },
                { "3ユーザ","" },
            };
            var moq = new Mock<IAutomationElementChildNameInfoGetter>();
            moq.Setup(x => x.UpdateNameListInfo(It.IsAny<bool>()))
                .Returns(retDict);

            var targetUsers = new List<string>()
            {
                "ユーザ 1",
                "ユーザ 2",
                "ユーザ 3",
            };
            target.RegisterMonitoringTargets(targetUsers);
            await target.StartMonitoring(callback, moq.Object);

            Assert.IsTrue(isCalled);
            Assert.AreEqual(true, target.IsAllJoin());
        }

        [TestMethod()]
        [TestCategory("参加状態切り替え")]
        public void 参加状態切り替え_全員参加状態に切り替え()
        {
            var target = new MonitoringModel();
            var targetUsers = new List<string>()
            {
                "ユーザ 1",
                "ユーザ 2",
                "ユーザ 3",
            };
            target.RegisterMonitoringTargets(targetUsers);

            target.SwitchingParticipantState(0);
            Assert.IsFalse(target.IsAllJoin());

            target.SwitchingParticipantState(1);
            target.SwitchingParticipantState(2);
            Assert.IsTrue(target.IsAllJoin());
        }

        [TestMethod()]
        [TestCategory("監視状態を自動に切り替え")]
        public void 監視状態を自動に切り替え_手動で全員参加後自動に戻して未参加状態に遷移()
        {
            var target = new MonitoringModel();
            var targetUsers = new List<string>()
            {
                "ユーザ 1",
                "ユーザ 2",
                "ユーザ 3",
            };
            target.RegisterMonitoringTargets(targetUsers);

            target.SwitchingParticipantState(0);
            target.SwitchingParticipantState(1);
            target.SwitchingParticipantState(2);
            Assert.IsTrue(target.IsAllJoin());

            target.SetParticipantAuto(0);
            Assert.IsFalse(target.IsAllJoin());
        }
    }
}