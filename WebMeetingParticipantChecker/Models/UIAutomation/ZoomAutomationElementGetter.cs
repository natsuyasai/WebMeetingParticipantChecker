﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using UIAutomationClient;
using WebMeetingParticipantChecker.Models.Config;
using WebMeetingParticipantChecker.Models.Monitoring;

namespace WebMeetingParticipantChecker.Models.UIAutomation
{
    internal class ZoomAutomationElementGetter : AutomationElementGetter
    {
        public ZoomAutomationElementGetter() : base()
        {
        }

        protected override string GetTargetName()
        {
            return AppSettingsManager.ZoomParticipantListName;
        }

        protected override IUIAutomationCondition GetConditon()
        {
            return _automation.CreatePropertyCondition(UIAutomationIdDefine.UIA_ControlTypePropertyId, UIAutomationIdDefine.UIA_ListControlTypeId);
        }
    }
}
