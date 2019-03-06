using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace PB.ITOps.AspNetCore.Versioning
{
    internal class ApiVersions
    {
        private readonly ApiVersion _currentVersion;
        
        internal IReadOnlyList<ApiVersion> AllVersions { get; }

        internal ApiVersions(ushort startApiVersion, ushort currentApiVersion)
        {
            if (currentApiVersion < startApiVersion) throw new ArgumentException($"{nameof(currentApiVersion)} must be >= {nameof(startApiVersion)}");

            _currentVersion = new ApiVersion(currentApiVersion, 0);
            var allVersions = new List<ApiVersion>();
            
            for (var i = startApiVersion; i <= currentApiVersion; i++)
            {
                allVersions.Add(new ApiVersion(i, 0));
            }

            AllVersions = allVersions;
        }
        
        internal ApiVersion[] GetSupportedVersions(ApiVersion introducedIn, ApiVersion removedAsOf = null)
        {
            if (introducedIn > _currentVersion)
                return new ApiVersion[0];
            
            if (removedAsOf is null)
                return new[] {_currentVersion};
            
            if (introducedIn > removedAsOf)
                throw new InvalidOperationException($"Cannot remove an API version ({removedAsOf}) before it has been introduced ({introducedIn}).");
            
            if (introducedIn == removedAsOf)
                throw new InvalidOperationException($"Cannot remove an API version ({removedAsOf}) in the same version it has been introduced ({introducedIn}).");
                        
            if (removedAsOf > _currentVersion)
                return new[] {_currentVersion};
            
            return new ApiVersion[0];
        }
        
        internal ApiVersion[] GetDeprecatedVersions(ApiVersion introducedIn, ApiVersion removedAsOf = null)
        {
            if (introducedIn == null)
                throw new ArgumentException($"{nameof(introducedIn)} cannot be null.");
            
            if (removedAsOf == null)
            {
                return AllVersions.Where(v => 
                    v >= introducedIn
                    && v < _currentVersion).ToArray();
            }

            return AllVersions.Where(v =>
                v >= introducedIn
                && v < removedAsOf).ToArray();
        }
    }
}