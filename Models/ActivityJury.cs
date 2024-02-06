using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceOrganizers.Models
{
    public class ActivityJury
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public int JuryId { get; set; }

        public Activity Activity { get; set; }
        public Jury Jury { get; set; }
    }
}
