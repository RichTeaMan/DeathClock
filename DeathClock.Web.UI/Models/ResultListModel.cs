using DeathClock.Persistence;

namespace DeathClock.Web.UI.Models
{
    public class ResultListModel
    {
        public string[] DatasetNames { get; set; }

        public BasePerson[] PersonList { get; set; }
        public string DatasetName { get; set; }
        public int? DeathYear { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
    }
}
