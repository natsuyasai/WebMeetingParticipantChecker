using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMeetingParticipantChecker.Models.Message;
using WebMeetingParticipantChecker.Models.Monitoring;
using WebMeetingParticipantChecker.Views;

namespace WebMeetingParticipantChecker.Models.FileWriter
{
    internal class MonitoringResultExporter : IMonitoringResultExportable
    {
        public bool Export(string fileName, IEnumerable<UserState> userStates)
        {
            try
            {
                var delimiter = ",";
                var filename = $@"{fileName}_{DateTime.Now:yyMMdd_hhmmssfff}.csv";
                var invalidChars = Path.GetInvalidFileNameChars();
                var invalidCharsRemovedName = string.Concat(filename.Where(ch => !invalidChars.Contains(ch)));
                using var stream = new FileStream(invalidCharsRemovedName, FileMode.OpenOrCreate);
                using var writer = new StreamWriter(stream, System.Text.Encoding.UTF8);
                foreach (var item in userStates)
                {
                    writer.Write(item.Name);
                    writer.Write(delimiter);
                    writer.Write(item.IsJoin ? "参加" : "未参加");
                    writer.Write(delimiter);
                    writer.Write(item.IsManual ? "手動" : "自動");
                    writer.Write(delimiter);
                    writer.Write("\r\n");
                }
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
