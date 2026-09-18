using CoursePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoursePlatform.Infrastructure.Persistence.Configurations
{
    public class LessonProgressConfiguration
    : IEntityTypeConfiguration<LessonProgress>
    {
        public void Configure(EntityTypeBuilder<LessonProgress> builder)
        {
            // Properties

            builder.Property(lp => lp.EnrollmentId)
                .IsRequired();

            builder.Property(lp => lp.LessonId)
                .IsRequired();

            builder.Property(lp => lp.IsCompleted)
                .IsRequired();

            builder.Property(lp => lp.CompletedAt)
                .IsRequired(false);


            // Prevent duplicate progress

            builder.HasIndex(lp => new
            {
                lp.EnrollmentId,
                lp.LessonId
            })
            .IsUnique();


            // Enrollment → LessonProgress

            builder.HasOne(lp => lp.Enrollment)
                .WithMany(e => e.LessonProgresses)
                .HasForeignKey(lp => lp.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);


            // Lesson → LessonProgress

            builder.HasOne(lp => lp.Lesson)
                .WithMany(l => l.Progresses)
                .HasForeignKey(lp => lp.LessonId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
