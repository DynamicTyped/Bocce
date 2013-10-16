using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Bocce.Configuration;
using Bocce.Diagnostics;

namespace Bocce.Notifications
{
    internal class DiagnosticTrace
    {
        private static readonly TraceSource TraceSource = new TraceSource("Bocce");
        private readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();
        private readonly Dictionary<string, Dictionary<string, Resource>> _cache = new Dictionary<string, Dictionary<string, Resource>>();

        public void ResourceMiss(Resource resource)
        {
            if (!DoesCacheExist(TraceEventType.Error, "ResourceMiss", resource))
                Trace(1001, TraceEventType.Error, "ResourceMiss", string.Format("{0} - {1} - {2}", resource.ResourceType, resource.Culture.Name, resource.ResourceKey));
        }

        public void ResourceFellBack(Resource resource)
        {
            if (!DoesCacheExist(TraceEventType.Warning, "ResourceFellBack", resource))
                Trace(1002, TraceEventType.Warning, "ResourceFellBack", string.Format("{0} - {1} - {2}", resource.ResourceType, resource.Culture.Name, resource.ResourceKey));
        }

        public void ResourceCleared(string resourceType)
        {
            ClearCache(resourceType);
            if (!DoesCacheExist(TraceEventType.Information, "ResourceCleared", new Resource(){ResourceType = resourceType}))
                Trace(1003, TraceEventType.Information, "ResourceCleared", resourceType);
        }

        public void ResourceHit(Resource resource)
        {
            if (DbResourceProviderSection.GetSection().TraceMatches &&
                !DoesCacheExist(TraceEventType.Verbose, "ResourceHit", resource))
            {
                Trace(1004, TraceEventType.Verbose, "ResourceHit",
                      string.Format("{0} - {1} - {2}", resource.ResourceType, resource.Culture.Name, resource.ResourceKey));
            }
        }

        private static void Trace(int id, TraceEventType type, string identifier, string description)
        {
            TraceSource.TraceEvent(id, new TraceRecord(type, identifier, description));
        }

        /// <summary>
        /// Checks for existence of cache and adds if not found
        /// </summary>
        /// <param name="type"></param>
        /// <param name="identifier"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        private bool DoesCacheExist(TraceEventType type, string identifier, Resource resource)
        {
            _cacheLock.EnterUpgradeableReadLock();
            try
            {
                Dictionary<string, Resource> resources;
                // does trace type exits
                if (_cache.TryGetValue(identifier, out resources))
                {
                    Resource result;
                    // trace exists, look for this resource
                    if (resources.TryGetValue(resource.Key, out result))
                    {
                        return true;
                    }

                    // resource doesn't exits, add it
                    _cacheLock.EnterWriteLock();
                    try
                    {
                        resources.Add(resource.Key, resource);
                    }
                    finally
                    {
                        _cacheLock.ExitWriteLock();
                    }
                    return false;
                }
                else
                {
                    // trace type not found, add it and it's resource
                    _cacheLock.EnterWriteLock();
                    try
                    {
                        _cache.Add(identifier, new Dictionary<string, Resource>() {{resource.Key, resource}});
                    }
                    finally
                    {
                        _cacheLock.ExitWriteLock();
                    }
                    return false;
                }
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }
        }

        private void ClearCache(string resourceType)
        {
            _cacheLock.EnterWriteLock();
            try
            {
                foreach (var item in _cache.Values)
                {
                    foreach (var resource in item.Where(a => string.Equals(a.Value.ResourceType, resourceType, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        item.Remove(resource.Key);
                    }
                }
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }
    }
}
