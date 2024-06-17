using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ma3ak.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Service> Service { get; set; }
        public DbSet<MaintenanceCenter> MaintenanceCenters { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Apppicture> Apppictures { get; set; }
        public DbSet<UserRequest> UserRequests { get; set; }


        public DbSet<UserLocation> userLocations { get; set; }
        public DbSet<CarRequest> CarRequests { get; set; }

        //public DbSet<AcceptedRequest> AcceptedRequests { get; set; }
        //public DbSet<DistanceInfo> DistanceInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        // Make Email field unique
        modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique()
                .HasFilter("[Email] IS NOT NULL");

            modelBuilder.Entity<MaintenanceCenter>()
                .HasIndex(e => e.Email)
                .IsUnique()
                .HasFilter("[Email] IS NOT NULL");

            modelBuilder.Entity<Service>()
                .HasIndex(e => e.ServiceName)
                .IsUnique()
                .HasFilter("[ServiceName] IS NOT NULL");

            //modelBuilder.Entity<UserRequest>()
            //    .HasOne(r => r.AcceptedRequest)
            //    .WithOne(a => a.UserRequest)
            //    .HasForeignKey<AcceptedRequest>(a => a.UserRequestId);

            //modelBuilder.Entity<DistanceInfo>()
            //    .HasKey(d => new { d.UserRequestId, d.CenterId });

            modelBuilder.Entity<Worker>()
                .HasOne(w => w.MaintenanceCenter)
                .WithMany(c => c.Workers)
                .HasForeignKey(w => w.MaintenanceCenterId);
        }
    }
}
