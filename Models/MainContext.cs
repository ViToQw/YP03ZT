using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceOrganizers.Models
{
    public class MainContext : DbContext
    {
        private readonly string connectionString = "Data source=DESKTOP-J021N6I;Initial catalog=Conference;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True;encrypt=False";
        private readonly string connectionStringHomeVika = "Data source=WIN-P8QG83K3N0U;Initial catalog=Conference;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True;encrypt=False";


        //connectionString="data source=192.168.227.14;initial catalog=Conference;persist security info=True;user id=user02;password=02;MultipleActiveResultSets=True;App=EntityFramework"

        public MainContext()
        {
            Database.EnsureCreated();
        }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityEvent> ActivityEvents { get; set; }
        public DbSet<ActivityJury> ActivityJuries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Jury> Juries { get; set; }
        public DbSet<Moderator> Moderators { get; set; }
        public DbSet<Organizer> Organizers { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<EventModerator> EventModerators { get; set; }
        public DbSet<ActivityModerator> ActivityModerators { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionStringHomeVika);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityEvent>()
                .HasOne(ae => ae.Event)
                .WithMany(e => e.ActivityEvents)
                .HasForeignKey(ae => ae.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ActivityJury>()
               .HasOne(aj => aj.Jury)
               .WithMany(j => j.ActivityJuries)
               .HasForeignKey(aj => aj.JuryId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ActivityModerator>()
                .HasOne(am => am.Moderator)
                .WithMany(m => m.ActivityModerators)
                .HasForeignKey(am => am.ModeratorId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
