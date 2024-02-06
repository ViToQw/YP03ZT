using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceOrganizers.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Jury> Juries { get; set; }
        public List<Moderator> Moderators { get; set; }
    }
}
