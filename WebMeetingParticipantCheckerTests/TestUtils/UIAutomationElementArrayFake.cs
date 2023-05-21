using System.Collections.Generic;
using System.Linq;
using UIAutomationClient;

namespace WebMeetingParticipantCheckerTests.TestUtils
{
    /// <summary>
    /// IUIAutomationElementArrayのFakeクラス
    /// </summary>
    internal class UIAutomationElementArrayFake : IUIAutomationElementArray
    {
        public List<IUIAutomationElement> elements = new List<IUIAutomationElement>();
        public IUIAutomationElement GetElement(int index)
        {
            return elements[index];
        }

        public int Length => elements.Count();
    }
}
