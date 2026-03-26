using Microsoft.EntityFrameworkCore;
using OrderManagement.API.Domain.Entities;

namespace OrderManagement.API.Infrastructure.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	public DbSet<Order> Orders => Set<Order>();
	public DbSet<OrderItem> OrderItems => Set<OrderItem>();
	public DbSet<PaymentRecord> PaymentRecords => Set<PaymentRecord>();
	public DbSet<ShipmentRecord> ShipmentRecords => Set<ShipmentRecord>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Order>(entity =>
		{
			entity.HasKey(o => o.Id);
			entity.Property(o => o.Status).HasConversion<string>();
			entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");

			entity.HasMany(o => o.Items)
				  .WithOne()
				  .HasForeignKey(i => i.OrderId)
				  .OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(o => o.Payment)
				  .WithOne()
				  .HasForeignKey<PaymentRecord>(p => p.OrderId)
				  .OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(o => o.Shipment)
				  .WithOne()
				  .HasForeignKey<ShipmentRecord>(s => s.OrderId)
				  .OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<OrderItem>(entity =>
		{
			entity.HasKey(i => i.Id);
			entity.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
		});
	}
}