using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceOrganizers.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime Date { get; set; }
        public int Days { get; set; }
        public int CityId { get; set; }
        public int? WinnerId { get; set; }
        public string Image {  get; set; }

        public string FormattedDate => Date.ToString("dd.MM.yyyy");
        public string CityName => City?.Name;

        public City City { get; set; }
        public Participant Winner { get; set; }
        public List<ActivityEvent> ActivityEvents { get; set; }
        public List<EventModerator> EventModerators { get; set; }
    }
}
