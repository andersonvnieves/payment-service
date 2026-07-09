using br.com.fiap.cloudgames.Payment.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace br.com.fiap.cloudgames.Payment.Infrastructure.Persistence.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }
  
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
    }

    public DbSet<Domain.Aggregates.Payment> Payments { get; set; }
}