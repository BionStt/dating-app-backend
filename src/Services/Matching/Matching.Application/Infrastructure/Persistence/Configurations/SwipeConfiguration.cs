using Matching.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matching.Application.Infrastructure.Persistence.Configurations
{
    public class SwipeConfiguration : IEntityTypeConfiguration<Swipe>
    {
        public void Configure(EntityTypeBuilder<Swipe> builder)
        {
            builder.ToTable("Swipes");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id).HasConversion(
                    s => s.Value,
                    s => new SwipeId(s)
                );

            builder.Property(s => s.FromUserId).IsRequired();

            builder.Property(s => s.ToUserId).IsRequired();

            builder.Property(s => s.Type)
                .HasConversion(
                    s => s.ToString(),
                    s => (SwipeType)Enum.Parse(typeof(SwipeType), s)
                )
                .IsRequired();

            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.HasIndex(s => new { s.FromUserId, s.ToUserId }).IsUnique();

            builder.HasIndex(s => s.CreatedAt);

        }
    }
}
