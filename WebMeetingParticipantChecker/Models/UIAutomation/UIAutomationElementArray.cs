using System;
using System.Collections.Generic;
using System.Linq;
using UIAutomationClient;

namespace WebMeetingParticipantChecker.Models.UIAutomation
{
    internal class UIAutomationElementArray : IUIAutomationElementArray
    {
        private readonly IReadOnlyCollection<IUIAutomationElement> _elements;

        public UIAutomationElementArray(IEnumerable<IUIAutomationElement> elements)
        {
            _elements = elements.ToList();
        }

        public UIAutomationElementArray(IUIAutomationElementArray array)
        {
            var elementArray = new List<IUIAutomationElement>();
            for (int i = 0; i < array.Length; i++)
            {
                elementArray.Add(array.GetElement(i));
            }
            _elements = elementArray;
        }

        public IUIAutomationElement GetElement(int index)
        {
            if (_elements.Count < index)
            {
                throw new ArgumentException("", nameof(index));
            }
            return _elements.ElementAt(index);
        }

        public int Length => _elements.Count;
    }
}
