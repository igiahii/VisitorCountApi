using Microsoft.EntityFrameworkCore;
using VisitCountApi.Entities;

namespace VisitCountApi.Data
{
    public class VisitorCountContext : DbContext
    {
        public VisitorCountContext(DbContextOptions<VisitorCountContext> options) : base(options)
        {
        }

        public virtual DbSet<Visitor> Visitors{ get; set; }
    }
}
