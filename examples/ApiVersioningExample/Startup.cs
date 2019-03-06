using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiVersioningExample.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PB.ITOps.AspNetCore.Versioning;

namespace ApiVersioningExample
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                
                // This is where we configure the new convention, and what api versions are available.
                // Example:
                // If we no longer want V1 to be available (e.g. no consumers of this version remain),
                // we can set the startVersion to 2.
                // If we want to introduce a new version, we can set currentVersion to 4.
                options.Conventions = new IntroducedApiVersionConventionBuilder(1, 3);
            });
            
            services.AddApiExplorer();
            services.AddSwagger(_configuration.GetSection("ApiDetails").Get<ApiDetails>());  
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint( $"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant() );
                }
            });
        }
    }
}