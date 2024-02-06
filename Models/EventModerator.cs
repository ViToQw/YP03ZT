using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceOrganizers.Models
{
    public class EventModerator
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int ModeratorId { get; set; }

        public Event Event { get; set; }
        public Moderator Moderator { get; set; }
    }
}
