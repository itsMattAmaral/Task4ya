using Microsoft.EntityFrameworkCore;
using Task4ya.Api.Extensions;
using Task4ya.Infrastructure.Data;


namespace Task4ya.Api;

	public static class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddApplicationConfiguration(builder.Configuration)
				.AddRepositories()
				.AddMediatRServices()
				.AddCachingServices(builder.Configuration)
				.AddAuthenticationServices(builder.Configuration)
				.AddSwaggerServices();
			
			var app = builder.Build();

			app = app.ConfigurePipeline(app.Environment);
			
			using (var scope = app.Services.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<Task4YaDbContext>();
				dbContext.Database.Migrate();
			}
			
			app.Run();
		}
	}
