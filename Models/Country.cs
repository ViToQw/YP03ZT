using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceOrganizers.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Acronym { get; set; }
        public string Name { get; set; }
        public string NameEng { get; set; }

        public List<Jury> Juries { get; set; }
        public List<Moderator> Moderators { get; set; }
        public List<Organizer> Organizers { get; set; }
        public List<Participant> Participants { get; set; }
    }
}
