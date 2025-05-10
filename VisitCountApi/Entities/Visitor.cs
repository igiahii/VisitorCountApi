using System.ComponentModel.DataAnnotations;

namespace VisitCountApi.Entities
{
    public class Visitor
    {
        [Key]
        public Guid VisitId { get; set; }
        public DateTime? InstertDateTime { get; set; }
        public DateTime? LastUpdateDateTime{ get; set; }
        public short VisitCount { get; set; }
    }
}
