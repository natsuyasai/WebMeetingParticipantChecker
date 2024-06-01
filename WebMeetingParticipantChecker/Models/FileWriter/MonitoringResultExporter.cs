using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMeetingParticipantChecker.Models.Monitoring;

namespace WebMeetingParticipantChecker.Models.FileWriter
{
    internal class MonitoringResultExporter : IMonitoringResultExportable
    {
        public bool Export(IReadOnlyCollection<UserState> userStates)
        {
            throw new NotImplementedException();
        }
    }
}
