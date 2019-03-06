using System.Runtime.CompilerServices;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

[assembly:InternalsVisibleTo("PB.ITOps.AspNetCore.Versioning.Tests")]
namespace PB.ITOps.AspNetCore.Versioning.Extensions
{
    internal static class CommonModelExtensions
    {
        internal static ApiVersion GetIntroducedVersion(this ICommonModel model)
        {
            return model.Attributes
                .OfType<IntroducedInApiVersionAttribute>()
                .Select(a => a.Version)
                .SingleOrDefault();
        }
        
        internal static ApiVersion GetRemovedVersion(this ICommonModel model)
        {
            return model.Attributes
                .OfType<RemovedAsOfApiVersionAttribute>()
                .Select(a => a.Version)
                .SingleOrDefault();
        }
    }
}