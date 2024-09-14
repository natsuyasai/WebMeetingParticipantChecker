using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIAutomationClient;
using WebMeetingParticipantCheckerTests.TestUtils;
using Microsoft.Extensions.Configuration;
using Moq;
using WebMeetingParticipantChecker.Models.UIAutomation.Utils;
using WebMeetingParticipantChecker.Models.UIAutomation.UserNameGetter;

namespace WebMeetingParticipantCheckerTests.Models.UIAutomation.UserNameGetter
{
    [TestClass()]
    public class UserNameElementGetterForTeamsTests
    {
        private readonly Mock<IKeyEventSender> _keyEventMock = new();

        [TestMethod()]
        [TestCategory("名前情報更新")]
        public void 名前情報取得を実行したときに取得できる名前情報が全て取得できること()
        {
            // 試験対象生成
            var fakeRootElement = new UIAutomationElementFake();
            var automaitonMock = CreateUIAutomationMock();
            var target = new UserNameElementGetterForTeams(automaitonMock.Object, fakeRootElement, _keyEventMock.Object, 0);

            // ダミーの要素情報生成
            var elemArrayFake = new UIAutomationElementArrayFake();
            var actual = new Dictionary<string, string>();
            for (int i = 0; i < 10; i++)
            {
                // Teamsは空白区切りになる
                var fakeelement = new UIAutomationElementFake
                {
                    CurrentName = "ユーザ" + (i + 1) + " テスト テスト テスト テスト"
                };
                elemArrayFake.elements.Add(fakeelement);
                actual[fakeelement.CurrentName.Replace(" ", "")] = fakeelement.CurrentName;
            }
            var fakeelementEmpty = new UIAutomationElementFake
            {
                CurrentName = ""
            };
            elemArrayFake.elements.Add(fakeelementEmpty);
            fakeRootElement.UIAutomationElementArrayFake = elemArrayFake;

            // 実行
            var ret = target.GetNameList(false);
            CollectionAssert.AreEqual(actual.Keys, ret.Keys.ToList());
            UIAutomationElementFake lastItem = (UIAutomationElementFake)fakeRootElement.UIAutomationElementArrayFake.GetElement(fakeRootElement.UIAutomationElementArrayFake.Length - 1);
            Assert.AreEqual(false, lastItem.selectionItemPatternFake.IsSelected);
            _keyEventMock.Verify(x => x.SendWait(KeyCode.Down), Times.Never());
        }

        private Mock<CUIAutomation> CreateUIAutomationMock()
        {
            var automaitonMock = new Mock<CUIAutomation>();
            var walkerMock = new Mock<IUIAutomationTreeWalker>();
            var conditionMock = new Mock<IUIAutomationCondition>();
            automaitonMock
                .Setup(x => x.CreateTreeWalker(It.IsAny<IUIAutomationCondition>()))
                .Returns(walkerMock.Object);
            automaitonMock.Setup(x => x.CreatePropertyCondition(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(conditionMock.Object);
            walkerMock
                .Setup(x => x.GetFirstChildElement(It.IsAny<IUIAutomationElement>()))
                .Returns<IUIAutomationElement>(elem => elem);
            walkerMock.Setup(x => x.GetNextSiblingElement(It.IsAny<IUIAutomationElement>()))
                .Returns<IUIAutomationElement>(elem => elem);

            return automaitonMock;
        }
    }
}