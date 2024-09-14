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
    public class UserNameElementGetterForZoomTests
    {
        private readonly Mock<IKeyEventSender> _keyEventMock = new();

        [TestMethod()]
        [TestCategory("名前情報更新")]
        public void 名前情報取得を実行したときに取得できる名前情報が全て取得できること()
        {
            // 試験対象生成
            var fakeRootElement = new UIAutomationElementFake();
            var target = new UserNameElementGetterForZoom(new CUIAutomation(), fakeRootElement, _keyEventMock.Object, 0);

            // ダミーの要素情報生成
            var elemArrayFake = new UIAutomationElementArrayFake();
            var expected = new Dictionary<string, string>();
            for (int i = 0; i < 10; i++)
            {
                var fakeelement = new UIAutomationElementFake
                {
                    CurrentName = "ユーザ" + (i + 1) + ",テスト,テスト,テスト,テスト"
                };
                elemArrayFake.elements.Add(fakeelement);
                expected["ユーザ" + (i + 1)] = fakeelement.CurrentName;
                expected["テスト"] = fakeelement.CurrentName;
            }
            var fakeelementEmpty = new UIAutomationElementFake
            {
                CurrentName = ""
            };
            elemArrayFake.elements.Add(fakeelementEmpty);
            fakeRootElement.UIAutomationElementArrayFake = elemArrayFake;

            // 実行
            var ret = target.GetNameList(false);
            CollectionAssert.AreEqual(expected.Keys, ret.Keys.ToList());
            UIAutomationElementFake lastItem = (UIAutomationElementFake)fakeRootElement.UIAutomationElementArrayFake.GetElement(fakeRootElement.UIAutomationElementArrayFake.Length - 1);
            Assert.AreEqual(false, lastItem.selectionItemPatternFake.IsSelected);
            _keyEventMock.Verify(x => x.SendWait(KeyCode.Down), Times.Never());
            _keyEventMock.Verify(x => x.SendWait(KeyCode.Up), Times.Never());
        }

        [TestMethod()]
        [TestCategory("名前情報更新")]
        public void 名前情報取得時に1度スクロールを実行して画面外の要素が取得しきれたあと最初の先頭要素に戻って終了すること()
        {
            // 試験対象生成
            var fakeRootElement = new UIAutomationElementFake();
            var target = new UserNameElementGetterForZoom(new CUIAutomation(), fakeRootElement, _keyEventMock.Object, 10);

            // ダミーの要素情報生成
            var elemArrayFake = new UIAutomationElementArrayFake();
            var expected = new Dictionary<string, string>();
            var lastFakeItem = new UIAutomationElementFake();
            for (int i = 0; i < 10; i++)
            {
                var fakeelement = new UIAutomationElementFake
                {
                    CurrentName = "ユーザ" + (i + 1) + ",テスト,テスト,テスト,テスト"
                };
                elemArrayFake.elements.Add(fakeelement);
                expected["ユーザ" + (i + 1)] = fakeelement.CurrentName;
                expected["テスト"] = fakeelement.CurrentName;
                lastFakeItem = fakeelement;
            }
            fakeRootElement.UIAutomationElementArrayFake = elemArrayFake;

            // 末尾が選択されたら次の要素を用意
            var lastFakeItem2 = new UIAutomationElementFake();
            lastFakeItem.selectionItemPatternFake.OnSelect = () =>
            {
                var elemArrayFake2 = new UIAutomationElementArrayFake();
                // 1度だけスクロール処理をするため、要素は0要素目がなくなって、11要素目が追加される
                for (int i = 1; i < 11; i++)
                {
                    var fakeelement = new UIAutomationElementFake
                    {
                        CurrentName = "ユーザ" + (i + 1) + ",テスト,テスト,テスト,テスト"
                    };
                    elemArrayFake2.elements.Add(fakeelement);
                    expected["ユーザ" + (i + 1)] = fakeelement.CurrentName;
                    expected["テスト"] = fakeelement.CurrentName;
                    lastFakeItem2 = fakeelement;
                }
                fakeRootElement.UIAutomationElementArrayFake = elemArrayFake2;
            };

            // 実行
            var ret = target.GetNameList(true);

            CollectionAssert.AreEqual(expected.Keys, ret.Keys.ToList());
            Assert.AreEqual(true, lastFakeItem.selectionItemPatternFake.IsSelected);
            Assert.AreEqual(true, lastFakeItem2.selectionItemPatternFake.IsSelected);
            _keyEventMock.Verify(x => x.SendWait(KeyCode.Down), Times.Exactly(2));
            // 要素数+補正値分上に戻す
            _keyEventMock.Verify(x => x.SendWait(KeyCode.Up), Times.Exactly(30 + 30 / 2));
        }


        [TestMethod()]
        [TestCategory("名前情報更新")]
        public void 名前要素取得時に初回位置を検出出来ず設定上限までスクロールを実施した場合はその時点で終了すること()
        {
            // 試験対象生成
            const int MaxCount = 10;
            var fakeRootElement = new UIAutomationElementFake();
            var target = new UserNameElementGetterForZoom(new CUIAutomation(), fakeRootElement, _keyEventMock.Object, MaxCount);

            // ダミーの要素情報生成
            var elemArrayFake = new UIAutomationElementArrayFake();
            var expected = new Dictionary<string, string>();
            var lastFakeItem = new UIAutomationElementFake();
            for (int i = 0; i < 10; i++)
            {
                var fakeelement = new UIAutomationElementFake
                {
                    CurrentName = "ユーザ" + (i + 1) + ",テスト,テスト,テスト,テスト"
                };
                elemArrayFake.elements.Add(fakeelement);
                expected["ユーザ" + (i + 1)] = fakeelement.CurrentName;
                expected["テスト"] = fakeelement.CurrentName;
                lastFakeItem = fakeelement;
            }
            fakeRootElement.UIAutomationElementArrayFake = elemArrayFake;

            // 末尾が選択されたら次の要素を用意
            var lastFakeItem2 = new UIAutomationElementFake();
            var shiftCount = 1;
            void onSelect()
            {
                var elemArrayFake2 = new UIAutomationElementArrayFake();
                for (int i = shiftCount * 10; i < shiftCount * 10 + 10; i++)
                {
                    var fakeelement = new UIAutomationElementFake
                    {
                        CurrentName = "ユーザ" + (i + 1) + ",テスト,テスト,テスト,テスト"
                    };
                    elemArrayFake2.elements.Add(fakeelement);
                    if (shiftCount < MaxCount)
                    {
                        expected["ユーザ" + (i + 1)] = fakeelement.CurrentName;
                        expected["テスト"] = fakeelement.CurrentName;
                    }
                    lastFakeItem2 = fakeelement;
                }
                shiftCount++;
                fakeRootElement.UIAutomationElementArrayFake = elemArrayFake2;
                lastFakeItem2.selectionItemPatternFake.OnSelect = onSelect;
            }
            lastFakeItem.selectionItemPatternFake.OnSelect = onSelect;

            // 実行
            var ret = target.GetNameList(true);

            CollectionAssert.AreEqual(expected.Keys, ret.Keys.ToList());
            Assert.AreEqual(true, lastFakeItem.selectionItemPatternFake.IsSelected);
            Assert.AreEqual(false, lastFakeItem2.selectionItemPatternFake.IsSelected);
            _keyEventMock.Verify(x => x.SendWait(KeyCode.Down), Times.Exactly(MaxCount));
            var upCount = MaxCount * 10;
            upCount += upCount / 2;
            _keyEventMock.Verify(x => x.SendWait(KeyCode.Up), Times.Exactly(upCount));
        }
    }
}