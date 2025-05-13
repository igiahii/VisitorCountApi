using Microsoft.EntityFrameworkCore;
using VisitCountApi.Entities;

namespace VisitCountApi.Data
{
    public class VisitorCountContext : DbContext
    {
        public VisitorCountContext(DbContextOptions<VisitorCountContext> options) : base(options)
        {
        }

        public virtual DbSet<Visitor> Visitors { get; set; }
        public virtual DbSet<DailyVisit> DailyVisits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DailyVisit>(entity =>
                entity.HasKey(e => e.PersianDateID)
            );
            modelBuilder.Entity<Visitor>(entity =>
                entity.HasKey(e => e.VisitId)
            );
            base.OnModelCreating(modelBuilder);
        }
    }
}
