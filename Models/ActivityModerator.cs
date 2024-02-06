using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceOrganizers.Models
{
    public class ActivityModerator
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public int ModeratorId { get; set; }

        public Activity Activity { get; set; }
        public Moderator Moderator { get; set; }
    }
}
