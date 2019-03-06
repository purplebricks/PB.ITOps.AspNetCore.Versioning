using System;
using Microsoft.AspNetCore.Mvc;

namespace PB.ITOps.AspNetCore.Versioning
{
    public class RemovedAsOfApiVersionAttribute : Attribute
    {
        public ApiVersion Version { get; }

        /// <summary>
        /// Apply this attribute to a controller or action to determine
        /// which API version it was removed as of.
        /// </summary>
        /// <remarks>
        /// The `IntroducedInApiVersionAttribute` must be applied to the controller,
        /// before this attribute can be applied.
        /// </remarks>
        /// <param name="majorVersion">Version API introduced</param>
        public RemovedAsOfApiVersionAttribute(ushort majorVersion)
        {
            Version = new ApiVersion(majorVersion, 0);
        }
    }
}