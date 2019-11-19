using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
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
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "Authorization header using Bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header
                });
                
                options.DocumentFilter<SwaggerSecurityRequirementsDocumentFilter>();
            });

            return services;
        }
        
        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description, ApiDetails apiDetails)
        {
            var info = new OpenApiInfo()
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
            public void Apply(OpenApiDocument document, DocumentFilterContext context)
            {
                document.SecurityRequirements = new List<OpenApiSecurityRequirement>()
                {
                    new OpenApiSecurityRequirement()
                    {
                        { new OpenApiSecurityScheme(), new List<string>() } 
                    }
                };
            }
        }
    }
}