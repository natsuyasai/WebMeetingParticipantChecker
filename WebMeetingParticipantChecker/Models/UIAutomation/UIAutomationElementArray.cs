using System;
using System.Collections.Generic;
using System.Linq;
using UIAutomationClient;

namespace WebMeetingParticipantChecker.Models.UIAutomation
{
    internal class UIAutomationElementArray : IUIAutomationElementArray
    {
        private readonly IReadOnlyCollection<IUIAutomationElement> _elements;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="elements"></param>
        public UIAutomationElementArray(IEnumerable<IUIAutomationElement> elements)
        {
            _elements = elements.ToList();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="array"></param>
        public UIAutomationElementArray(IUIAutomationElementArray array)
        {
            var elementArray = new List<IUIAutomationElement>();
            for (int i = 0; i < array.Length; i++)
            {
                elementArray.Add(array.GetElement(i));
            }
            _elements = elementArray;
        }

        /// <summary>
        /// 要素取得
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IUIAutomationElement GetElement(int index)
        {
            if (_elements.Count < index)
            {
                throw new ArgumentException("", nameof(index));
            }
            return _elements.ElementAt(index);
        }

        /// <summary>
        /// サイズ取得
        /// </summary>
        public int Length => _elements.Count;

        /// <summary>
        /// 指定の名前の要素が含まれているか
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return _elements.Any(item => item.CurrentName == name);
        }

    }
}
