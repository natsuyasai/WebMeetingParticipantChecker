using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMeetingParticipantChecker.Models.Config
{
    internal class ConfigrationParameter
    {
        public string? MonitoringCycleMs { get; set; }
        public string? KyedownMaxCount { get; set; }
        public string? ZoomParticipantListName { get; set; }
        public string? TeamsParticipantListName { get; set; }
    }
}
