using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebMeetingParticipantChecker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMeetingParticipantChecker.Utils.Tests
{
    [TestClass()]
    public class StringUtilsTests
    {
        [DataTestMethod]
        [TestCategory("空白除去")]
        [DataRow("", "")]
        [DataRow(" test ", "test")]
        [DataRow(" te st ", "test")]
        [DataRow(" te s　t ", "test")]
        public void 空白除去(string src, string expected)
        {
            Assert.AreEqual(expected, StringUtils.RemoveSpace(src));
        }

        [TestMethod()]
        [TestCategory("空白前後入れ替え")]
        [DataRow("", "")]
        [DataRow(" test ", " test ")]
        [DataRow(" te st ", "stte")]
        [DataRow(" ああ いい ", "いいああ")]
        [DataRow(" te s　t ", "s　tte")]
        public void 空白前後入れ替え(string src, string expected)
        {
            StringUtils.TrySwapBeforeAndAfterTheSpace(src, out var result);
            Assert.AreEqual(expected, result);
        }
    }
}