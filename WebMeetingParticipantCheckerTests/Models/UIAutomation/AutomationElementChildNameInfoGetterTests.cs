using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebMeetingParticipantChecker.Models.UIAutomation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIAutomationClient;
using WebMeetingParticipantCheckerTests.TestUtils;

namespace WebMeetingParticipantChecker.Models.UIAutomation.Tests
{
    [TestClass()]
    public class AutomationElementChildNameInfoGetterTests
    {
        [TestMethod()]
        [TestCategory("名前情報更新")]
        public void 名前情報更新_自動スクロール無効()
        {
            // 試験対象生成
            var fakeRootElement = new UIAutomationElementFake();
            var target = new AutomationElementChildNameInfoGetter(new CUIAutomation(), fakeRootElement, 0);

            // ダミーの要素情報生成
            var elemArrayFake = new UIAutomationElementArrayFake();
            var actual = new Dictionary<string, string>();
            for (int i = 0; i < 10; i++)
            {
                var fakeelement = new UIAutomationElementFake
                {
                    CurrentName = ("ユーザ" + (i + 1) + ",テスト,テスト,テスト,テスト")
                };
                elemArrayFake.elements.Add(fakeelement);
                actual["ユーザ" + (i + 1)] = fakeelement.CurrentName;
                actual["テスト"] = fakeelement.CurrentName;
            }
            var fakeelementEmpty = new UIAutomationElementFake
            {
                CurrentName = ""
            };
            elemArrayFake.elements.Add(fakeelementEmpty);
            fakeRootElement.UIAutomationElementArrayFake = elemArrayFake;

            // 実行
            var ret = target.UpdateNameListInfo(false);
            CollectionAssert.AreEqual(actual.Keys, ret.Keys.ToList());
            UIAutomationElementFake lastItem = (UIAutomationElementFake)fakeRootElement.UIAutomationElementArrayFake.GetElement(fakeRootElement.UIAutomationElementArrayFake.Length - 1);
            Assert.AreEqual(false, lastItem.selectionItemPatternFake.IsSelected);
        }

        [TestMethod()]
        [TestCategory("名前情報更新")]
        public void 名前情報更新_自動スクロール有効_1回_先頭一致()
        {
            // 試験対象生成
            var fakeRootElement = new UIAutomationElementFake();
            var target = new AutomationElementChildNameInfoGetter(new CUIAutomation(), fakeRootElement, 10);

            // ダミーの要素情報生成
            var elemArrayFake = new UIAutomationElementArrayFake();
            var actual = new Dictionary<string, string>();
            var fakeelementEmpty = new UIAutomationElementFake
            {
                CurrentName = ""
            };
            elemArrayFake.elements.Add(fakeelementEmpty);
            var lastFakeItem = new UIAutomationElementFake();
            for (int i = 0; i < 10; i++)
            {
                var fakeelement = new UIAutomationElementFake
                {
                    CurrentName = ("ユーザ" + (i + 1) + ",テスト,テスト,テスト,テスト")
                };
                elemArrayFake.elements.Add(fakeelement);
                actual["ユーザ" + (i + 1)] = fakeelement.CurrentName;
                actual["テスト"] = fakeelement.CurrentName;
                lastFakeItem = fakeelement;
            }
            fakeRootElement.UIAutomationElementArrayFake = elemArrayFake;

            // 末尾が選択されたら次の要素を用意
            var lastFakeItem2 = new UIAutomationElementFake();
            lastFakeItem.selectionItemPatternFake.OnSelect = () =>
            {
                var elemArrayFake2 = new UIAutomationElementArrayFake();
                for (int i = 10; i < 20; i++)
                {
                    var fakeelement = new UIAutomationElementFake
                    {
                        CurrentName = ("ユーザ" + (i + 1) + ",テスト,テスト,テスト,テスト")
                    };
                    elemArrayFake2.elements.Add(fakeelement);
                    actual["ユーザ" + (i + 1)] = fakeelement.CurrentName;
                    actual["テスト"] = fakeelement.CurrentName;
                    lastFakeItem2 = fakeelement;
                }
                fakeRootElement.UIAutomationElementArrayFake = elemArrayFake2;
                // 2週目の末尾選択で再度1週目の要素に戻す
                lastFakeItem2.selectionItemPatternFake.OnSelect = () =>
                {
                    fakeRootElement.UIAutomationElementArrayFake = elemArrayFake;
                };
            };

            // 実行
            var ret = target.UpdateNameListInfo(true);
            CollectionAssert.AreEqual(actual.Keys, ret.Keys.ToList());
            Assert.AreEqual(true, lastFakeItem.selectionItemPatternFake.IsSelected);
            Assert.AreEqual(true, lastFakeItem2.selectionItemPatternFake.IsSelected);
        }

        [TestMethod()]
        [TestCategory("名前情報更新")]
        public void 名前情報更新_自動スクロール有効_1回_末尾一致()
        {
            // 試験対象生成
            var fakeRootElement = new UIAutomationElementFake();
            var target = new AutomationElementChildNameInfoGetter(new CUIAutomation(), fakeRootElement, 10);

            // ダミーの要素情報生成
            var elemArrayFake = new UIAutomationElementArrayFake();
            var actual = new Dictionary<string, string>();
            var fakeelementEmpty = new UIAutomationElementFake
            {
                CurrentName = ""
            };
            elemArrayFake.elements.Add(fakeelementEmpty);
            var lastFakeItem = new UIAutomationElementFake();
            for (int i = 0; i < 10; i++)
            {
                var fakeelement = new UIAutomationElementFake
                {
                    CurrentName = ("ユーザ" + (i + 1) + ",テスト,テスト,テスト,テスト")
                };
                elemArrayFake.elements.Add(fakeelement);
                actual["ユーザ" + (i + 1)] = fakeelement.CurrentName;
                actual["テスト"] = fakeelement.CurrentName;
                lastFakeItem = fakeelement;
            }
            fakeRootElement.UIAutomationElementArrayFake = elemArrayFake;

            // 末尾が選択されたら次の要素を用意
            var lastFakeItem2 = new UIAutomationElementFake();
            lastFakeItem.selectionItemPatternFake.OnSelect = () =>
            {
                var elemArrayFake2 = new UIAutomationElementArrayFake();
                for (int i = 1; i < 10; i++)
                {
                    var fakeelement = new UIAutomationElementFake
                    {
                        CurrentName = ("ユーザ" + (i + 1) + ",テスト,テスト,テスト,テスト")
                    };
                    elemArrayFake2.elements.Add(fakeelement);
                    actual["ユーザ" + (i + 1)] = fakeelement.CurrentName;
                    actual["テスト"] = fakeelement.CurrentName;
                    lastFakeItem2 = fakeelement;
                }
                fakeRootElement.UIAutomationElementArrayFake = elemArrayFake2;
                // 2週目の末尾選択で再度1週目の要素に戻す
                lastFakeItem2.selectionItemPatternFake.OnSelect = () =>
                {
                    fakeRootElement.UIAutomationElementArrayFake = elemArrayFake;
                };
            };

            // 実行
            var ret = target.UpdateNameListInfo(true);
            CollectionAssert.AreEqual(actual.Keys, ret.Keys.ToList());
            Assert.AreEqual(true, lastFakeItem.selectionItemPatternFake.IsSelected);
        }


        [TestMethod()]
        [TestCategory("名前情報更新")]
        public void 名前情報更新_自動スクロール有効_カウント上限()
        {
            // 試験対象生成
            const int MaxCount = 10;
            var fakeRootElement = new UIAutomationElementFake();
            var target = new AutomationElementChildNameInfoGetter(new CUIAutomation(), fakeRootElement, MaxCount);

            // ダミーの要素情報生成
            var elemArrayFake = new UIAutomationElementArrayFake();
            var actual = new Dictionary<string, string>();
            var fakeelementEmpty = new UIAutomationElementFake
            {
                CurrentName = ""
            };
            elemArrayFake.elements.Add(fakeelementEmpty);
            var lastFakeItem = new UIAutomationElementFake();
            for (int i = 0; i < 10; i++)
            {
                var fakeelement = new UIAutomationElementFake
                {
                    CurrentName = ("ユーザ" + (i + 1) + ",テスト,テスト,テスト,テスト")
                };
                elemArrayFake.elements.Add(fakeelement);
                actual["ユーザ" + (i + 1)] = fakeelement.CurrentName;
                actual["テスト"] = fakeelement.CurrentName;
                lastFakeItem = fakeelement;
            }
            fakeRootElement.UIAutomationElementArrayFake = elemArrayFake;

            // 末尾が選択されたら次の要素を用意
            var lastFakeItem2 = new UIAutomationElementFake();
            var shiftCount = 1;
            void onSelect()
            {
                var elemArrayFake2 = new UIAutomationElementArrayFake();
                for (int i = (shiftCount * 10); i < (shiftCount * 10 + 10); i++)
                {
                    var fakeelement = new UIAutomationElementFake
                    {
                        CurrentName = ("ユーザ" + (i + 1) + ",テスト,テスト,テスト,テスト")
                    };
                    elemArrayFake2.elements.Add(fakeelement);
                    if (shiftCount != MaxCount)
                    {
                        actual["ユーザ" + (i + 1)] = fakeelement.CurrentName;
                        actual["テスト"] = fakeelement.CurrentName;
                    }
                    lastFakeItem2 = fakeelement;
                }
                shiftCount++;
                fakeRootElement.UIAutomationElementArrayFake = elemArrayFake2;
                lastFakeItem2.selectionItemPatternFake.OnSelect = onSelect;
            }
            lastFakeItem.selectionItemPatternFake.OnSelect = onSelect;

            // 実行
            var ret = target.UpdateNameListInfo(true);
            CollectionAssert.AreEqual(actual.Keys, ret.Keys.ToList());
            Assert.AreEqual(true, lastFakeItem.selectionItemPatternFake.IsSelected);
            Assert.AreEqual(false, lastFakeItem2.selectionItemPatternFake.IsSelected);
        }
    }
}