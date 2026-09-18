using CoursePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoursePlatform.Infrastructure.Persistence.Configurations
{
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            // Properties

            builder.Property(l => l.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(l => l.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(l => l.VideoUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(l => l.Order)
                .IsRequired();


            // Course → Lessons

            builder.HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);


            // Lesson → LessonProgress

            builder.HasMany(l => l.Progresses)
                .WithOne(lp => lp.Lesson)
                .HasForeignKey(lp => lp.LessonId)
                .OnDelete(DeleteBehavior.Restrict);


            // Index

            builder.HasIndex(l => new { l.CourseId, l.Order });
        }
    }
}