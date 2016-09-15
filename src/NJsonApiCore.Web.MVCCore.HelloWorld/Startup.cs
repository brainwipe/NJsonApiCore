using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NJsonApi.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Serialization;
using NJsonApi.Web.MVCCore.Serialization;

namespace NJsonApi.Web.MVCCore.HelloWorld
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            var nJsonApiConfig = NJsonApiConfiguration.BuildConfiguration();

            services.AddMvc(
                options =>
                {
                    options.Conventions.Add(new ApiExplorerVisibilityEnabledConvention());
                    options.Filters.Add(typeof(JsonApiResourceFilter));
                    options.Filters.Add(typeof(JsonApiActionFilter));
                    options.Filters.Add(typeof(JsonApiExceptionFilter));
                    options.OutputFormatters.Insert(0, new JsonApiOutputFormatter(nJsonApiConfig));
                    options.InputFormatters.OfType<JsonInputFormatter>().First().SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/vnd.api+json"));
                });
           
            services.AddSingleton<ILinkBuilder, LinkBuilder>();
            services.AddSingleton(nJsonApiConfig.GetJsonSerializer());
            services.AddSingleton<IJsonApiTransformer, JsonApiTransformer >();
            services.AddSingleton(nJsonApiConfig);
            services.AddSingleton<TransformationHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            //TODO resolve error with Static Files: Ensure the type is concrete and services are registered for all parameters of a public constructor.
            //app.UseMiddleware<StaticFileMiddleware>(new StaticFileOptions(new SharedOptions()));

            app.UseCors(builder =>
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod()
            );


            app.UseMvc();
        }

    }
}