using Microsoft.EntityFrameworkCore;
using Task4ya.Domain.Entities;

namespace Task4ya.Infrastructure.Data;

public class Task4YaDbContext(DbContextOptions<Task4YaDbContext> options) : DbContext(options)
{
	public DbSet<TaskItem> TaskItems => Set<TaskItem>();
	public DbSet<Board> Boards => Set<Board>();
	
	public DbSet<User> Users => Set<User>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<TaskItem>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
			entity.Property(e => e.Description).HasMaxLength(2000);
			entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
			entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
			entity.Property(e => e.DueDate).IsRequired(false);
			entity.Property(e => e.Status).IsRequired();
			entity.Property(e => e.Priority).IsRequired();
			entity.Property(e => e.AssigneeToId);
			entity.Property(e => e.BoardId);
			entity.HasOne<User>()
				.WithMany()
				.HasForeignKey(e => e.AssigneeToId)
				.OnDelete(DeleteBehavior.SetNull);
			entity.HasOne<Board>()
				.WithMany()
				.HasForeignKey(e => e.BoardId)
				.OnDelete(DeleteBehavior.Cascade);
			entity.HasIndex(e => e.BoardId);
			entity.HasIndex(e => e.AssigneeToId);
		});
		modelBuilder.Entity<Board>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
			entity.HasMany(e => e.TaskGroup)
				.WithMany()
				.UsingEntity(j => j.ToTable("BoardTaskItems"));
		});
		modelBuilder.Entity<User>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
			entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
			entity.Property(e => e.Password).IsRequired().HasMaxLength(200);
			entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
			entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
			entity.HasIndex(e => e.Email).IsUnique();
		});
		
		base.OnModelCreating(modelBuilder);
	}
}