using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceOrganizers.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public int ModeratorId { get; set; }
        public int Day { get; set; }

        public Moderator Moderator { get; set; }
        public List<ActivityEvent> ActivityEvents { get; set; }
        public List<ActivityJury> ActivityJuries { get; set; }
    }
}
