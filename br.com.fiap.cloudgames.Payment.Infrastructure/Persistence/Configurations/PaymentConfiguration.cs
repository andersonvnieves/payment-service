using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace br.com.fiap.cloudgames.Payment.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Domain.Aggregates.Payment>
    {
        public void Configure(EntityTypeBuilder<Domain.Aggregates.Payment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(u => u.OrderId)
               .IsRequired();
            builder.Property(u => u.UserId)
               .IsRequired();
            builder.OwnsOne(u => u.Amount, price =>
            {
                price.Property(e => e.PriceValue)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("Amount");
            });
            builder.Property(u => u.Status)
                .HasConversion<string>()
                .IsRequired();
            builder.Property(u => u.CreatedAt)
                .IsRequired();
            builder.Property(u => u.UpdatedAt)
                .IsRequired();
        }
    }
}
