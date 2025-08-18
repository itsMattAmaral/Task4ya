namespace Task4ya.Api.Extensions;

public static class ApplicationBuilderExtensions
{
	public static WebApplication ConfigurePipeline(this WebApplication app, IHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint("/swagger/v1/swagger.json", "Task4ya API v1");
				options.RoutePrefix = string.Empty;
			});
		}
		app.UseHttpsRedirection();
		app.UseRouting();
		app.UseAuthentication();
		app.UseAuthorization();
		app.MapControllers();

		return app;
	}
}