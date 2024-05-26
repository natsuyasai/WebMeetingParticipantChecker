using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMeetingParticipantChecker.Models.Theme
{
    internal class Theme
    {
        public int Id { get; set; }
        public string Name { get; private set; }

        public Theme(int id, string name)
        {
            Id = id;
            Name = name;
        }   
    }
}
