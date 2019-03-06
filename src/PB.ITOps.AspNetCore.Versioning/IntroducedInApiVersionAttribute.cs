using System;
using Microsoft.AspNetCore.Mvc;

namespace PB.ITOps.AspNetCore.Versioning
{
    public class IntroducedInApiVersionAttribute : Attribute
    {
        public ApiVersion Version { get; }

        /// <summary>
        /// Apply this attribute to a controller or action to determine
        /// which API version it was introduced.
        /// </summary>
        /// <remarks>
        /// This attribute must be applied to a controller, before it can
        /// be applied to an attribute.
        /// </remarks>
        /// <param name="majorVersion">Version API introduced</param>
        public IntroducedInApiVersionAttribute(ushort majorVersion)
        {
            Version = new ApiVersion(majorVersion, 0);
        }
    }
}