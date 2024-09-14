using System;
using UIAutomationClient;
using WebMeetingParticipantChecker.Models.UIAutomation.Define;

namespace WebMeetingParticipantCheckerTests.TestUtils
{
    /// <summary>
    /// IUIAutomationElementのFakeクラス
    /// </summary>
    internal class UIAutomationElementFake : IUIAutomationElement
    {
        public UIAutomationElementArrayFake UIAutomationElementArrayFake = default!;

        public void SetFocus()
        {
            throw new NotImplementedException();
        }

        public Array GetRuntimeId()
        {
            throw new NotImplementedException();
        }

        public IUIAutomationElement FindFirst(TreeScope scope, IUIAutomationCondition condition)
        {
            throw new NotImplementedException();
        }

        public IUIAutomationElementArray FindAll(TreeScope scope, IUIAutomationCondition condition)
        {
            return UIAutomationElementArrayFake;
        }

        public IUIAutomationElement FindFirstBuildCache(TreeScope scope, IUIAutomationCondition condition, IUIAutomationCacheRequest cacheRequest)
        {
            throw new NotImplementedException();
        }

        public IUIAutomationElementArray FindAllBuildCache(TreeScope scope, IUIAutomationCondition condition, IUIAutomationCacheRequest cacheRequest)
        {
            throw new NotImplementedException();
        }

        public IUIAutomationElement BuildUpdatedCache(IUIAutomationCacheRequest cacheRequest)
        {
            throw new NotImplementedException();
        }

        public dynamic GetCurrentPropertyValue(int propertyId)
        {
            throw new NotImplementedException();
        }

        public dynamic GetCurrentPropertyValueEx(int propertyId, int ignoreDefaultValue)
        {
            throw new NotImplementedException();
        }

        public dynamic GetCachedPropertyValue(int propertyId)
        {
            throw new NotImplementedException();
        }

        public dynamic GetCachedPropertyValueEx(int propertyId, int ignoreDefaultValue)
        {
            throw new NotImplementedException();
        }

        public nint GetCurrentPatternAs(int patternId, ref Guid riid)
        {
            throw new NotImplementedException();
        }

        public nint GetCachedPatternAs(int patternId, ref Guid riid)
        {
            throw new NotImplementedException();
        }

        public UIAutomationSelectionItemPatternFake selectionItemPatternFake = new UIAutomationSelectionItemPatternFake();
        public dynamic? GetCurrentPattern(int patternId)
        {
            if (patternId == UIAutomationIdDefine.UIA_SelectionPatternId)
            {
                return selectionItemPatternFake;
            }
            return null;
        }

        public dynamic GetCachedPattern(int patternId)
        {
            throw new NotImplementedException();
        }

        public IUIAutomationElement GetCachedParent()
        {
            throw new NotImplementedException();
        }

        public IUIAutomationElementArray GetCachedChildren()
        {
            throw new NotImplementedException();
        }

        public int GetClickablePoint(out tagPOINT clickable)
        {
            throw new NotImplementedException();
        }

        public int CurrentProcessId => throw new NotImplementedException();

        public int CurrentControlType => throw new NotImplementedException();

        public string CurrentLocalizedControlType => throw new NotImplementedException();

        public string? CurrentName
        {
            get;
            set;
        }

        public string CurrentAcceleratorKey => throw new NotImplementedException();

        public string CurrentAccessKey => throw new NotImplementedException();

        public int CurrentHasKeyboardFocus => throw new NotImplementedException();

        public int CurrentIsKeyboardFocusable => throw new NotImplementedException();

        public int CurrentIsEnabled => throw new NotImplementedException();

        public string CurrentAutomationId => throw new NotImplementedException();

        public string CurrentClassName => throw new NotImplementedException();

        public string CurrentHelpText => throw new NotImplementedException();

        public int CurrentCulture => throw new NotImplementedException();

        public int CurrentIsControlElement => throw new NotImplementedException();

        public int CurrentIsContentElement => throw new NotImplementedException();

        public int CurrentIsPassword => throw new NotImplementedException();

        public nint CurrentNativeWindowHandle => throw new NotImplementedException();

        public string CurrentItemType => throw new NotImplementedException();

        public int CurrentIsOffscreen => throw new NotImplementedException();

        public OrientationType CurrentOrientation => throw new NotImplementedException();

        public string CurrentFrameworkId => throw new NotImplementedException();

        public int CurrentIsRequiredForForm => throw new NotImplementedException();

        public string CurrentItemStatus => throw new NotImplementedException();

        public tagRECT CurrentBoundingRectangle => throw new NotImplementedException();

        public IUIAutomationElement CurrentLabeledBy => throw new NotImplementedException();

        public string CurrentAriaRole => throw new NotImplementedException();

        public string CurrentAriaProperties => throw new NotImplementedException();

        public int CurrentIsDataValidForForm => throw new NotImplementedException();

        public IUIAutomationElementArray CurrentControllerFor => throw new NotImplementedException();

        public IUIAutomationElementArray CurrentDescribedBy => throw new NotImplementedException();

        public IUIAutomationElementArray CurrentFlowsTo => throw new NotImplementedException();

        public string CurrentProviderDescription => throw new NotImplementedException();

        public int CachedProcessId => throw new NotImplementedException();

        public int CachedControlType => throw new NotImplementedException();

        public string CachedLocalizedControlType => throw new NotImplementedException();

        public string CachedName => throw new NotImplementedException();

        public string CachedAcceleratorKey => throw new NotImplementedException();

        public string CachedAccessKey => throw new NotImplementedException();

        public int CachedHasKeyboardFocus => throw new NotImplementedException();

        public int CachedIsKeyboardFocusable => throw new NotImplementedException();

        public int CachedIsEnabled => throw new NotImplementedException();

        public string CachedAutomationId => throw new NotImplementedException();

        public string CachedClassName => throw new NotImplementedException();

        public string CachedHelpText => throw new NotImplementedException();

        public int CachedCulture => throw new NotImplementedException();

        public int CachedIsControlElement => throw new NotImplementedException();

        public int CachedIsContentElement => throw new NotImplementedException();

        public int CachedIsPassword => throw new NotImplementedException();

        public nint CachedNativeWindowHandle => throw new NotImplementedException();

        public string CachedItemType => throw new NotImplementedException();

        public int CachedIsOffscreen => throw new NotImplementedException();

        public OrientationType CachedOrientation => throw new NotImplementedException();

        public string CachedFrameworkId => throw new NotImplementedException();

        public int CachedIsRequiredForForm => throw new NotImplementedException();

        public string CachedItemStatus => throw new NotImplementedException();

        public tagRECT CachedBoundingRectangle => throw new NotImplementedException();

        public IUIAutomationElement CachedLabeledBy => throw new NotImplementedException();

        public string CachedAriaRole => throw new NotImplementedException();

        public string CachedAriaProperties => throw new NotImplementedException();

        public int CachedIsDataValidForForm => throw new NotImplementedException();

        public IUIAutomationElementArray CachedControllerFor => throw new NotImplementedException();

        public IUIAutomationElementArray CachedDescribedBy => throw new NotImplementedException();

        public IUIAutomationElementArray CachedFlowsTo => throw new NotImplementedException();

        public string CachedProviderDescription => throw new NotImplementedException();
    }
}
