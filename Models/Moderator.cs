using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceOrganizers.Models
{
    public class Moderator
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronimic { get; set; }
        public int CourseId { get; set; }
        public string Email { get; set; }
        public int CountryId { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Photo { get; set; }
        public string Gender { get; set; }
        public DateTime? Birthday { get; set; }

        public Course Course { get; set; }
        public Country Country { get; set; }
        public List<Activity> Activities { get; set; }
        public List<EventModerator> EventModerators { get; set; }
        public List<ActivityModerator> ActivityModerators { get; set; }
    }
}
