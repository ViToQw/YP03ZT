using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceOrganizers.Models
{
    public class Participant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronimic { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
        public int CountryId { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Photo { get; set; }
        public string Gender { get; set; }

        public Country Country { get; set; }
        public List<Event> Events { get; set; }
    }
}
