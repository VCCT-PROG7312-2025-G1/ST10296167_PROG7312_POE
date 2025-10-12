namespace ST10296167_PROG7312_POE.Models
{
    public class EventsAnnouncementsViewModel
    {
        public List<Event> Events { get; set; } = new List<Event>();

        public List<Announcement> Announcements { get; set; } = new List<Announcement>();

        public List<Event> RecommendedEvents { get; set; } = new List<Event>();

        public List<String> Categories { get; set; } = new List<String>();

        SearchQuery CurrentSearch { get; set; } = new SearchQuery();

        // Selected filters
        public string SelectedCategory { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
