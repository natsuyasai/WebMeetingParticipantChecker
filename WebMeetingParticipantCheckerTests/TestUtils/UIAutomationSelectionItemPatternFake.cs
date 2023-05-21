using System;
using UIAutomationClient;

namespace WebMeetingParticipantCheckerTests.TestUtils
{
    /// <summary>
    /// IUIAutomationSelectionItemPatternのFakeクラス
    /// </summary>
    internal class UIAutomationSelectionItemPatternFake : IUIAutomationSelectionItemPattern
    {
        public bool IsSelected { get; set; } = false;
        public Action? OnSelect;
        public void Select()
        {
            IsSelected = true;
            OnSelect?.Invoke();
        }

        public void AddToSelection()
        {
            throw new NotImplementedException();
        }

        public void RemoveFromSelection()
        {
            throw new NotImplementedException();
        }

        public int CurrentIsSelected => throw new NotImplementedException();

        public IUIAutomationElement CurrentSelectionContainer => throw new NotImplementedException();

        public int CachedIsSelected => throw new NotImplementedException();

        public IUIAutomationElement CachedSelectionContainer => throw new NotImplementedException();
    }
}
