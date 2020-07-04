using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Catalog.API.Infrastructure.Filters.Catalog.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Catalog.API
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			services
				//	.AddAppInsight(Configuration)
				.AddGrpc().Services
				.AddCustomMVC(Configuration)
				//	.AddCustomDbContext(Configuration)
				//	.AddCustomOptions(Configuration)
				//	.AddIntegrationServices(Configuration)
				//	.AddEventBus(Configuration)
				.AddSwagger(Configuration);
			//.AddCustomHealthCheck(Configuration);

			var container = new ContainerBuilder();
			container.Populate(services);

			return new AutofacServiceProvider(container.Build());
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			var pathBase = Configuration["PATH_BASE"];

			if (!string.IsNullOrEmpty(pathBase))
			{
				loggerFactory.CreateLogger<Startup>().LogDebug("Using PATH BASE '{pathBase}'", pathBase);
				app.UsePathBase(pathBase);
			}

			app.UseSwagger()
			 .UseSwaggerUI(c =>
			 {
				 c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "Catalog.API V1");
			 });

			app.UseCors("CorsPolicy");
			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
				endpoints.MapControllers();
				//endpoints.MapGet("/_proto/", async ctx =>
				//{
				//	ctx.Response.ContentType = "text/plain";
				//	using var fs = new FileStream(Path.Combine(env.ContentRootPath, "Proto", "catalog.proto"), FileMode.Open, FileAccess.Read);
				//	using var sr = new StreamReader(fs);
				//	while (!sr.EndOfStream)
				//	{
				//		var line = await sr.ReadLineAsync();
				//		if (line != "/* >>" || line != "<< */")
				//		{
				//			await ctx.Response.WriteAsync(line);
				//		}
				//	}

				//});
			});
		}

	}
	public static class CustomExtensionMethods
	{
		public static IServiceCollection AddAppInsight(this IServiceCollection services, IConfiguration configuration)
		{
			//services.AddApplicationInsightsTelemetry(configuration);
			//services.AddApplicationInsightsKubernetesEnricher();

			return services;
		}

		public static IServiceCollection AddCustomMVC(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddControllers(options =>
			{
				options.Filters.Add(typeof(HttpGlobalExceptionFilter));
			}).AddNewtonsoftJson();

			services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy",
					builder => builder
					.SetIsOriginAllowed((host) => true)
					.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowCredentials());
			});

			return services;
		}


		public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddSwaggerGen(options =>
			{
				options.DescribeAllEnumsAsStrings();
				options.SwaggerDoc("v1", new OpenApiInfo
				{
					Title = "e-commerce - Catalog HTTP API",
					Version = "v1",
					Description = "The Catalog Microservice HTTP API. This is a Data-Driven/CRUD microservice"
				});
			});

			return services;

		}

	}
}

