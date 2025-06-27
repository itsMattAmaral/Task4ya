using Microsoft.EntityFrameworkCore;
using Task4ya.Domain.Entities;

namespace Task4ya.Infrastructure.Data;

public class Task4YaDbContext : DbContext
{
	public Task4YaDbContext(DbContextOptions<Task4YaDbContext> options) : base(options)
	{
	}
	
	public DbSet<TaskItem> TaskItems => Set<TaskItem>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<TaskItem>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
			entity.Property(e => e.Description).HasMaxLength(1000);
			entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
			entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
			entity.Property(e => e.DueDate).IsRequired(false);
			entity.Property(e => e.Status).IsRequired();
			entity.Property(e => e.Priority).IsRequired();
		});
		modelBuilder.Entity<Board>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
			entity.HasMany(e => e.TaskGroup)
				.WithMany()
				.UsingEntity(j => j.ToTable("BoardTaskItems"));
		});
		base.OnModelCreating(modelBuilder);
	}
}