using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Task4ya.Api.Helpers;
using Task4ya.Domain.Repositories;
using Task4ya.Infrastructure.Data;
using Task4ya.Infrastructure.Repositories;

namespace Task4ya.Api.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services,
		IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("DefaultConnection");
		if (string.IsNullOrEmpty(connectionString))
		{
			throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
		}
		
		services.AddDbContext<Task4YaDbContext>(
			options => options.UseNpgsql(
				connectionString, 
				npgsqlOptions =>
				{
					npgsqlOptions.MigrationsAssembly("Task4ya.Infrastructure");
					npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");
				}));
		
		services.AddControllers();
		services.AddEndpointsApiExplorer();

		return services;
	}

	public static IServiceCollection AddRepositories(this IServiceCollection services)
	{
		services.AddScoped<AuthHelpers>();
		services.AddScoped<IBoardRepository, BoardRepository>();
		services.AddScoped<ITaskItemRepository, TaskItemRepository>();
		services.AddScoped<IUserRepository, UserRepository>();
		
		return services;
	}

	public static IServiceCollection AddMediatRServices(this IServiceCollection services)
	{
		var assemblyTypes = new[]
		{
			typeof(Application.Board.Commands.BoardCommandHandler),
			typeof(Application.Board.Queries.BoardQueryHandler),
			typeof(Application.TaskItem.Commands.TaskItemCommandHandler),
			typeof(Application.TaskItem.Queries.TaskItemsQueryHandler),
			typeof(Application.User.Commands.UserCommandHandler),
			typeof(Application.User.Queries.UserQueryHandler)
		};

		foreach (var type in assemblyTypes)
		{
			services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(type));
		}
		
		return services;
	}

	public static IServiceCollection AddAuthenticationServices(this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddAuthentication(config =>
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
						configuration["ApplicationSettings:JWT_Secret"] ?? string.Empty)
					),
				ValidateIssuer = false,
				ValidateAudience = false,
				ClockSkew = TimeSpan.Zero,
				RoleClaimType = "role"
			};
		});
		
		services.AddAuthorizationBuilder()
                    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
                    .AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"))
                    .AddPolicy("AnyUser", policy => policy.RequireRole("User", "Admin", "Manager"))
                    .AddPolicy("AdminOrManager", policy => policy.RequireRole("Admin", "Manager"));
		
		return services;
	}

	public static void AddSwaggerServices(this IServiceCollection services)
	{
		services.AddSwaggerGen(options =>
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

	}
}