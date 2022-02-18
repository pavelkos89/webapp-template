using Elements.Logging.Tracing;
using Elements.LoggingContract;
using Elements.Security.Token.Jwt;
using ${{ values.component_id }}.Configuration;
using ${{ values.component_id }}.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ${{ values.component_id }}
{
	public class Startup
	{
		public Startup()
		{ }

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IConfigProvider, ConfigProvider>();

			ConfigureAuth(services);

			services.AddControllers();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseMiddleware<CustomLoggingMiddleware>();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseMiddleware(typeof(ErrorHandlingMiddleware));
			}

			if (!string.IsNullOrWhiteSpace(ConfigProvider.GetPathBase()))
			{
				app.UsePathBase(ConfigProvider.GetPathBase());
			}

			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseCors(x => x
				.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader());

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}

		protected virtual void ConfigureAuth(IServiceCollection services)
		{
			using var tracer = Tracer.TraceMethod(LogLevel.Operation, $"{nameof(Startup)}.{nameof(ConfigureAuth)}");
			services.AddTransient<IValidationOptionsProvider, ValidationOptionsProvider>();
			services.AddElementsAuthentication();
		}
	}
}
