using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VisitCountApi.Entities
{
    public class DailyVisit
    {
        public int PersianDateID { get; set; }
        public int TotalVisits { get; set; }
        public DateTime? InsertDateTime { get; set; }
    }
}
