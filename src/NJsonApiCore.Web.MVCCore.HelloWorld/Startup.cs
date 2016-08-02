using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NJsonApi.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using NJsonApi.Web.MVCCore.Serialization;

namespace NJsonApi.Web.MVCCore.HelloWorld
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            var nJsonApiConfig = NJsonApiConfiguration.BuildConfiguration();

            services.AddMvc(
                options =>
                {
                    options.Conventions.Add(new ApiExplorerVisibilityEnabledConvention());
                    options.Filters.Add(typeof(JsonApiActionFilter));
                    options.Filters.Add(typeof(JsonApiExceptionFilter));
                    options.OutputFormatters.Insert(0, new JsonApiOutputFormatter(nJsonApiConfig));
                });
            
            services.AddSingleton<ILinkBuilder, LinkBuilder>();
            services.AddSingleton(nJsonApiConfig.GetJsonSerializer());
            services.AddSingleton<IJsonApiTransformer, JsonApiTransformer>();
            services.AddSingleton(nJsonApiConfig);
            services.AddSingleton<TransformationHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }

    }
}