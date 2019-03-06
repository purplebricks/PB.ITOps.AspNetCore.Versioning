using System;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ApiVersioningExample.DependencyInjection
{
    internal static class ApiVersionServiceCollectionExtensions
    {
        public static IServiceCollection AddApiExplorer( this IServiceCollection services)
        {
            services.AddMvcCore().AddApiExplorer();
            services.TryAddSingleton<IApiVersionDescriptionProvider, DefaultApiVersionDescriptionProvider>();
            services.TryAddEnumerable(ServiceDescriptor.Transient<IApiDescriptionProvider, VersionedApiDescriptionProvider>());
            services.Configure<ApiExplorerOptions>(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}