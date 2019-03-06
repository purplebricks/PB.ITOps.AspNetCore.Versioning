using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiVersioningExample.DependencyInjection
{
    internal static class SwaggerServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, ApiDetails apiDetails)
        {
            services.AddSwaggerGen(options =>
            {
                // resolve the IApiVersionDescriptionProvider service
                // note: that we have to build a temporary service provider here because one has not been created yet
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description, apiDetails));
                }

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
                options.AddSecurityDefinition("Bearer", new ApiKeyScheme()
                {
                    Description = "Authorization header using Bearer scheme",
                    Name = "Authorization",
                    In = "header"
                });
                
                options.DocumentFilter<SwaggerSecurityRequirementsDocumentFilter>();
            });

            return services;
        }
        
        private static Info CreateInfoForApiVersion(ApiVersionDescription description, ApiDetails apiDetails)
        {
            var info = new Info()
            {
                Title = $"{apiDetails.Title} {description.ApiVersion} - {apiDetails.Owners}",
                Version = description.ApiVersion.ToString(),
                Description = $"### {apiDetails.Description}\n" +
                              $"### [{apiDetails.Title} - Git Repository]({apiDetails.GitRepoUrl})"
            };

            if (description.IsDeprecated)
            {
                info.Description = $"## Warning: This API version has been deprecated.\r\n{info.Description}";
            }

            return info;
        }

        private class SwaggerSecurityRequirementsDocumentFilter : IDocumentFilter
        {
            public void Apply(SwaggerDocument document, DocumentFilterContext context)
            {
                document.Security = new List<IDictionary<string, IEnumerable<string>>>()
                {
                    new Dictionary<string, IEnumerable<string>>()
                    {
                        { "Bearer", new string[]{ } } 
                    }
                };
            }
        }
    }
}