﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIAutomationClient;

namespace WebMeetingParticipantChecker.Models.UIAutomation
{
    internal interface IAutomationElementGetter
    {
        bool DetectiParticipantElement();
        IUIAutomationElement? GetTargetElement();
    }
}
