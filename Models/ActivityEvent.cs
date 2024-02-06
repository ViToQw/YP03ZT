using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceOrganizers.Models
{
    public class ActivityEvent
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public int EventId { get; set; }

        public Activity Activity { get; set; }
        public Event Event { get; set; }
    }
}
