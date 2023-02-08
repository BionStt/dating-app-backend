using Matching.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matching.Application.Infrastructure.Persistence.Configurations
{
    public class MatchConfiguration : IEntityTypeConfiguration<Match>
    {
        public void Configure(EntityTypeBuilder<Match> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id).HasConversion(
                    s => s.Value,
                    s => new MatchId(s)
                );


            builder.Property(s => s.PartnerOneId).IsRequired();
            
            builder.Property(s => s.PartnerTwoId).IsRequired();

            builder.Property(s => s.CreatedAt).IsRequired();

            builder.HasIndex(s => new { s.PartnerOneId, s.PartnerTwoId }).IsUnique();

            builder.HasIndex(s => s.CreatedAt);

        }
    }
}
