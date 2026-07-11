using carequeue.CQ.API.Models.Entites;
using carequeue.CQ.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace carequeue.CQ.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; } // Added DbSet for DoctorSchedules
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<OtpVerification> OtpVerifications { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }

        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // PostgreSQL extension for case-insensitive column queries
            modelBuilder.HasPostgresExtension("citext");

            // I. Soft delete filter for the main entities
            modelBuilder.Entity<User>().HasQueryFilter(u => u.IsActive);
            modelBuilder.Entity<Customer>().HasQueryFilter(c => c.IsActive);

            // II. Propagate the filter to dependent entities to fix the warnings
            modelBuilder.Entity<Appointment>().HasQueryFilter(a => a.Customer!.IsActive);
            modelBuilder.Entity<OtpVerification>().HasQueryFilter(o => o.Customer!.IsActive);
            modelBuilder.Entity<Notification>().HasQueryFilter(n => n.Customer!.IsActive);

            // 1. Hospital Defaults
            modelBuilder.Entity<Hospital>(entity =>
            {
                entity.HasQueryFilter(h => h.IsActive); // Soft Delete Filter
                entity.Property(h => h.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
                entity.Property(h => h.IsActive).HasDefaultValue(true);
                entity.Property(h => h.HospitalName).HasMaxLength(150);
            });

            // 2. User Defaults & Unique constraints
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasQueryFilter(u => u.IsActive); // Soft Delete Filter
                entity.Property(u => u.Email).HasColumnType("citext").HasMaxLength(255).IsRequired();
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
                entity.Property(u => u.IsActive).HasDefaultValue(true);
                entity.Property(u => u.IsEmailVerified).HasDefaultValue(false);
                entity.HasOne(u => u.Hospital).WithMany().HasForeignKey(u => u.HospitalId).OnDelete(DeleteBehavior.Restrict);
                entity.Property(u => u.Role).HasConversion<string>().HasMaxLength(30).IsRequired();
                entity.Property(u => u.Name).HasMaxLength(100).IsRequired();
            });

            // 3. Customer Configuration
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");
                entity.HasQueryFilter(c => c.IsActive); // Soft Delete Filter
                entity.Property(c => c.Email).HasColumnType("citext").HasMaxLength(255).IsRequired();
                entity.HasIndex(c => c.Email).IsUnique();
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
                entity.Property(c => c.IsActive).HasDefaultValue(true);
            });

            // 4. Doctor Configurations
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasQueryFilter(d => d.IsActive); // Soft Delete Filter
                entity.Property(d => d.IsAvailable).HasDefaultValue(true);
                entity.Property(d => d.IsActive).HasDefaultValue(true);
                entity.Property(d => d.HospitalId).IsRequired();
                entity.HasOne(d => d.Hospital).WithMany().HasForeignKey(d => d.HospitalId).OnDelete(DeleteBehavior.Restrict);
                entity.Property(d => d.ConsultationFee).HasPrecision(10, 2);
                entity.Property(d => d.Name).HasMaxLength(100);
                entity.Property(d => d.Specialization).HasMaxLength(100);
            });

            // 4b. Doctor Schedule Configurations
            modelBuilder.Entity<DoctorSchedule>(entity =>
            {
                entity.ToTable("DoctorSchedules");
                entity.HasQueryFilter(ds => ds.IsActive); // Soft Delete Filter
                entity.Property(ds => ds.IsActive).HasDefaultValue(true);
                entity.Property(ds => ds.DayOfWeek).HasConversion<string>().HasMaxLength(15).IsRequired();
                entity.HasOne(ds => ds.Doctor).WithMany().HasForeignKey(ds => ds.DoctorId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(ds => new { ds.DoctorId, ds.DayOfWeek });
            });

            // 5. Patient Configurations
            modelBuilder.Entity<Patient>(entity =>
            {
                // Assuming Patient also has an IsActive property for soft deletes
                entity.HasQueryFilter(p => p.IsActive); // Soft Delete Filter
                entity.Property(p => p.IsActive).HasDefaultValue(true);

                entity.Property(p => p.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
                entity.HasOne(p => p.Hospital).WithMany().HasForeignKey(p => p.HospitalId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(p => p.Customer).WithMany(c => c.Patients).HasForeignKey(p => p.CustomerId).OnDelete(DeleteBehavior.Cascade);
                entity.Property(p => p.Name).HasMaxLength(100);
                entity.Property(p => p.Email).HasMaxLength(255);
                entity.Property(p => p.Phone).HasMaxLength(10);
            });

            // 6. Appointment Constraints & Enums
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("Appointments");

                entity.Property(a => a.CreatedAt).HasDefaultValueSql("timezone('utc', now())");

                // Foreign Key Relationships
                entity.HasOne(a => a.Hospital).WithMany().HasForeignKey(a => a.HospitalId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(a => a.Patient).WithMany().HasForeignKey(a => a.PatientId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(a => a.Doctor).WithMany().HasForeignKey(a => a.DoctorId).OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Customer)
                    .WithMany()
                    .HasForeignKey(a => a.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Prevent double-booking for the same doctor at the same slot
                entity.HasIndex(a => new
                {
                    a.DoctorId,
                    a.AppointmentDate,
                    a.AppointmentTime
                }).IsUnique();

                entity.Property(a => a.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20) 
                    .IsRequired();
            });

            // 7. Notification Configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notifications");

                entity.Property(n => n.CreatedAt).HasDefaultValueSql("timezone('utc', now())");

                entity.Property(n => n.Channel)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(n => n.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(n => n.Recipient).HasMaxLength(250).IsRequired();
                entity.Property(n => n.Subject).HasMaxLength(250);

                // Foreign Key Relationships
                entity.HasOne(n => n.Hospital).WithMany().HasForeignKey(n => n.HospitalId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(n => n.Patient).WithMany().HasForeignKey(n => n.PatientId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(n => n.Template).WithMany().HasForeignKey(n => n.TemplateId).OnDelete(DeleteBehavior.Restrict);

                // Updated: Shifted from User -> Customer
                entity.HasOne(n => n.Customer)
                    .WithMany()
                    .HasForeignKey(n => n.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(n => n.Appointment)
                    .WithMany()
                    .HasForeignKey(n => n.AppointmentId)
                    .OnDelete(DeleteBehavior.SetNull); // Keeping this SetNull is smart; if an appointment is canceled/deleted, the log of the notification stays intact.
            });

            // 8. OTP Verifications & Purpose Enums
            modelBuilder.Entity<OtpVerification>(entity =>
            {
                entity.ToTable("OtpVerifications");

                entity.Property(o => o.CreatedAt).HasDefaultValueSql("timezone('utc', now())");

                entity.Property(o => o.Purpose)
                    .HasConversion<string>()
                    .HasMaxLength(30)
                    .IsRequired();

                entity.HasOne(o => o.Customer)
                    .WithMany()
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade); // If a consumer account is entirely deleted, drop their pending/historical OTP hashes too
            });

            // 9. Email Logging Configuration
            modelBuilder.Entity<EmailLog>(entity =>
            {
                entity.Property(el => el.SentAt).HasDefaultValueSql("timezone('utc', now())");
                entity.HasOne(el => el.Notification).WithMany().HasForeignKey(el => el.NotificationId).OnDelete(DeleteBehavior.Cascade);
            });

            // 10. Notification Templates & Type Enums
            modelBuilder.Entity<NotificationTemplate>(entity =>
            {
                entity.Property(nt => nt.Type).HasConversion<string>().HasMaxLength(50).IsRequired();
                entity.Property(nt => nt.IsActive).HasDefaultValue(true);
                entity.Property(nt => nt.Name).HasMaxLength(100).IsRequired();
                entity.Property(nt => nt.Subject).HasMaxLength(200).IsRequired();
                entity.Property(nt => nt.Body).IsRequired();
            });

            // 11. Payments
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments");

                // Indexes for fast querying when webhooks hit your API
                entity.HasIndex(p => p.RazorpayOrderId).IsUnique();
                entity.HasIndex(p => p.RazorpayPaymentId).IsUnique();

                entity.Property(p => p.Amount)
                    .HasPrecision(10, 2)
                    .IsRequired();

                entity.Property(p => p.Currency)
                    .HasDefaultValue("INR")
                    .HasMaxLength(10);

                // Map Enum to String for clean PostgreSQL tables
                entity.Property(p => p.Status)
                    .HasConversion<string>()
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(p => p.CreatedAt).HasDefaultValueSql("timezone('utc', now())");

                // Relationships
                entity.HasOne(p => p.Hospital)
                    .WithMany()
                    .HasForeignKey(p => p.HospitalId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Customer)
                    .WithMany()
                    .HasForeignKey(p => p.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Ensure 1:1 or 1:Many rule depending on if an appointment can have multiple retry payments
                entity.HasOne(p => p.Appointment)
                    .WithMany()
                    .HasForeignKey(p => p.AppointmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}