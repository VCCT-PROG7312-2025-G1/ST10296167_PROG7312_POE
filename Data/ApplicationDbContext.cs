using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ST10296167_PROG7312_POE.Models;

namespace ST10296167_PROG7312_POE.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Issue> Issues { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<UploadedFile> UploadedFiles { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Issue entity
            builder.Entity<Issue>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Suburb).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Category).IsRequired();
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
            });

            // Configure UploadedFile entity
            builder.Entity<UploadedFile>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.FileName).IsRequired();
                entity.Property(e => e.MimeType).IsRequired();
                entity.Property(e => e.FilePath).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
                
                entity.HasOne(e => e.Issue)
                      .WithMany(e => e.DbFiles)
                      .HasForeignKey(e => e.IssueID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Feedback entity
            builder.Entity<Feedback>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Rating).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
            });

            // Configure Announcement entity
            builder.Entity<Announcement>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(30);
                entity.Property(e => e.Content).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Category).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
            });

            // Configure Event entity
            builder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(30);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Category).IsRequired();
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Location).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
            });
        }
    }
}
