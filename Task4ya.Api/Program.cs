using Microsoft.EntityFrameworkCore;
using Task4ya.Application.Board.Commands;
using Task4ya.Application.Board.Queries;
using Task4ya.Infrastructure.Data;
using Task4ya.Application.TaskItem.Commands;
using Task4ya.Application.TaskItem.Queries;
using Task4ya.Domain.Repositories;
using Task4ya.Infrastructure.Repositories;

namespace Task4ya.Api;

	public static class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
			}
			builder.Services.AddScoped<IBoardRepository, BoardRepository>();
			builder.Services.AddMediatR(ctg => ctg.RegisterServicesFromAssemblyContaining<BoardCommandHandler>());
			builder.Services.AddMediatR(ctg => ctg.RegisterServicesFromAssemblyContaining<BoardQueryHandler>());
			builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();
			builder.Services.AddMediatR(ctg => ctg.RegisterServicesFromAssemblyContaining<TaskItemCommandHandler>());
			builder.Services.AddMediatR(ctg => ctg.RegisterServicesFromAssemblyContaining<TaskItemsQueryHandler>());
			builder.Services.AddDbContext<Task4YaDbContext>(
				options => options.UseNpgsql(connectionString));
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			
			var app = builder.Build();
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI(options =>
				{
					options.SwaggerEndpoint("/swagger/v1/swagger.json", "Task4ya API v1");
					options.RoutePrefix = string.Empty;
				});
			}
			using (var scope = app.Services.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<Task4YaDbContext>();
				dbContext.Database.Migrate();
			}
			app.UseHttpsRedirection();
			app.UseRouting();
			app.MapControllers();
			app.Run();
		}
	}
