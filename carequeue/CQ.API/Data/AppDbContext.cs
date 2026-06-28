using carequeue.CQ.API.Models.Entites;
using Microsoft.EntityFrameworkCore;

namespace carequeue.CQ.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<OtpVerification> OtpVerifications { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Enable PostgreSQL extension for case-insensitive column queries
            modelBuilder.HasPostgresExtension("citext");

            // 1. Hospital Defaults
            modelBuilder.Entity<Hospital>(entity =>
            {
                entity.Property(h => h.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
                entity.Property(h => h.IsActive).HasDefaultValue(true);
                entity.Property(h => h.HospitalName).HasMaxLength(150);
            });

            // 2. User Defaults & Unique constraints
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Email)
                    .HasColumnType("citext") // Enforces unique checks like admin@care.com == Admin@care.com
                    .HasMaxLength(255)
                    .IsRequired();

                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
                entity.Property(u => u.IsActive).HasDefaultValue(true);
                entity.Property(u => u.IsEmailVerified).HasDefaultValue(false);
                entity.HasOne(u => u.Hospital).WithMany().HasForeignKey(u => u.HospitalId).OnDelete(DeleteBehavior.Restrict);
                entity.Property(u => u.Role).HasConversion<string>().HasMaxLength(30).IsRequired();
                entity.Property(u => u.Name).HasMaxLength(100).IsRequired();
            });

            // 3. Doctor Configurations
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.Property(d => d.IsAvailable).HasDefaultValue(true);
                entity.Property(d => d.IsActive).HasDefaultValue(true);
                entity.HasOne(d => d.Hospital).WithMany().HasForeignKey(d => d.HospitalId).OnDelete(DeleteBehavior.Restrict);
                entity.Property(d => d.ConsultationFee).HasPrecision(10, 2);
                entity.Property(d => d.Name).HasMaxLength(100);
                entity.Property(d => d.Specialization).HasMaxLength(100);
            });

            // 4. Patient Configurations
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
                entity.HasOne(p => p.Hospital).WithMany().HasForeignKey(p => p.HospitalId).OnDelete(DeleteBehavior.Restrict);
                entity.Property(p => p.Name).HasMaxLength(100);
                entity.Property(p => p.Email).HasMaxLength(255);
                entity.Property(p => p.Phone).HasMaxLength(10);
            });

            // 5. Appointment Constraints & Enums
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.Property(a => a.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
                entity.HasOne(a => a.Hospital).WithMany().HasForeignKey(a => a.HospitalId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(a => a.Patient).WithMany().HasForeignKey(a => a.PatientId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(a => a.Doctor).WithMany().HasForeignKey(a => a.DoctorId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(a => a.CreatedByUser).WithMany().HasForeignKey(a => a.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);

                // Prevent double-booking
                entity.HasIndex(a => new
                {
                    a.DoctorId,
                    a.AppointmentDate,
                    a.AppointmentTime
                }).IsUnique();

                entity.Property(a => a.Status)
                    .HasConversion<string>()
                    .HasMaxLength(30)
                    .IsRequired();
            });

            // 6. Notification Channels & Status Enums
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(n => n.Channel)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(n => n.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();

                entity.HasOne(n => n.Hospital).WithMany().HasForeignKey(n => n.HospitalId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(n => n.User).WithMany().HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(n => n.Patient).WithMany().HasForeignKey(n => n.PatientId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(n => n.Appointment).WithMany().HasForeignKey(n => n.AppointmentId).OnDelete(DeleteBehavior.SetNull);
            });

            // 7. OTP Verifications & Purpose Enums
            modelBuilder.Entity<OtpVerification>(entity =>
            {
                entity.Property(o => o.CreatedAt).HasDefaultValueSql("timezone('utc', now())");

                entity.Property(o => o.Purpose)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .IsRequired();

                entity.HasOne(o => o.User).WithMany().HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            // 8. Email Logging Configuration
            modelBuilder.Entity<EmailLog>(entity =>
            {
                entity.Property(el => el.SentAt).HasDefaultValueSql("timezone('utc', now())");
                entity.HasOne(el => el.Notification).WithMany().HasForeignKey(el => el.NotificationId).OnDelete(DeleteBehavior.Cascade);
            });

            // 9. Notification Templates & Type Enums
            modelBuilder.Entity<NotificationTemplate>(entity =>
            {
                entity.Property(nt => nt.Type)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(nt => nt.IsActive).HasDefaultValue(true);
                entity.Property(nt => nt.Name).HasMaxLength(100).IsRequired();
                entity.Property(nt => nt.Subject).HasMaxLength(200).IsRequired();
                entity.Property(nt => nt.Body).IsRequired();
            });
        }
    }
}