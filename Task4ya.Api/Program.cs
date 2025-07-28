using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Task4ya.Api.Helpers;
using Task4ya.Application.Board.Commands;
using Task4ya.Application.Board.Queries;
using Task4ya.Infrastructure.Data;
using Task4ya.Application.TaskItem.Commands;
using Task4ya.Application.TaskItem.Queries;
using Task4ya.Application.User.Commands;
using Task4ya.Application.User.Queries;
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
			builder.Services.AddScoped<AuthHelpers>();
			builder.Services.AddScoped<IBoardRepository, BoardRepository>();
			builder.Services.AddMediatR(ctg => ctg.RegisterServicesFromAssemblyContaining<BoardCommandHandler>());
			builder.Services.AddMediatR(ctg => ctg.RegisterServicesFromAssemblyContaining<BoardQueryHandler>());
			builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();
			builder.Services.AddMediatR(ctg => ctg.RegisterServicesFromAssemblyContaining<TaskItemCommandHandler>());
			builder.Services.AddMediatR(ctg => ctg.RegisterServicesFromAssemblyContaining<TaskItemsQueryHandler>());
			builder.Services.AddScoped<IUserRepository, UserRepository>();
			builder.Services.AddMediatR(ctg => ctg.RegisterServicesFromAssemblyContaining<UserCommandHandler>());
			builder.Services.AddMediatR(ctg => ctg.RegisterServicesFromAssemblyContaining<UserQueryHandler>());
			builder.Services.AddDbContext<Task4YaDbContext>(
				options => options.UseNpgsql(
					connectionString, 
					npgsqlOptions => npgsqlOptions.MigrationsAssembly("Task4ya.Infrastructure")
					));

			builder.Services.AddAuthentication(config =>
			{
				config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{ 
				options.RequireHttpsMetadata = false;
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(
							builder.Configuration["ApplicationSettings:JWT_Secret"] ?? string.Empty)
						),
					ValidateIssuer = false,
					ValidateAudience = false,
					ClockSkew = TimeSpan.Zero
				};
			});
			builder.Services.AddAuthorization();
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
				{
					Title = "Task4ya API",
					Version = "v1",
					Description = "API for Task4ya application"
				});

				var jwtSecurityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
				{
					Scheme = "bearer",
					BearerFormat = "JWT",
					Name = "Authorization",
					In = Microsoft.OpenApi.Models.ParameterLocation.Header,
					Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
					Description = "Put **_only_** your JWT Bearer token below.",
					Reference = new Microsoft.OpenApi.Models.OpenApiReference
					{
						Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
						Id = JwtBearerDefaults.AuthenticationScheme
					}
				};
				
				options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
				options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
				{
					{ jwtSecurityScheme, Array.Empty<string>() }
				});
			});
			
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
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();
			app.Run();
		}
	}
