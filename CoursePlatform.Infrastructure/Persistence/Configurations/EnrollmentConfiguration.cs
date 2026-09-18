using CoursePlatform.Domain.Entities;
using CoursePlatform.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoursePlatform.Infrastructure.Persistence.Configurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            // Properties

            builder.Property(e => e.StudentId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(e => e.CourseId)
                .IsRequired();

            builder.Property(e => e.Status)
                .IsRequired();

            builder.Property(e => e.EnrolledAt)
                .IsRequired();


            // Student → Enrollments

            builder.HasOne<ApplicationUser>()
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);


            // Enrollment → LessonProgress

            builder.HasMany(e => e.LessonProgresses)
                .WithOne(lp => lp.Enrollment)
                .HasForeignKey(lp => lp.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);


            // Prevent duplicate enrollment

            builder.HasIndex(e => new { e.StudentId, e.CourseId })
                .IsUnique();
        }
    }
}
