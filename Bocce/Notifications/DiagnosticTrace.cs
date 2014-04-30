using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Bocce.Configuration;
using System.Linq;
using Bocce.Diagnostics;

namespace Bocce.Notifications
{
	internal class DiagnosticTrace : IDiagnosticTrace
	{
		private static readonly TraceSource TraceSource = new TraceSource("Bocce");
		internal readonly ConcurrentDictionary<Tuple<string, string, string>, string> _cache = new ConcurrentDictionary<Tuple<string, string, string>, string>();

		public void ResourceMiss(Resource resource)
		{
			Trace(1001, TraceEventType.Error, "OnResourceMiss", string.Format("Resource Missed: {0} - {1} - {2}", resource.ResourceType, resource.Culture.Name, resource.ResourceKey), resource);
		}

		public void ResourceFellBack(Resource resource)
		{
			Trace(1002, TraceEventType.Warning, "OnResourceFellBack", string.Format("Resource Fell Back: {0} - {1} - {2}", resource.ResourceType, resource.Culture.Name, resource.ResourceKey), resource);
		}

		public void ResourceCleared(string resourceType)
		{
			ClearCache(resourceType);
			Trace(1003, TraceEventType.Information, "OnResourceCleared", string.Format("Resources cleared for {0}", resourceType), new Resource() { ResourceType = resourceType });
		}

		public void ResourceHit(Resource resource)
		{
			if (DbResourceProviderSection.GetSection().TraceMatches)
				Trace(1004, TraceEventType.Verbose, "OnResourceHit", string.Format("Resource Match found: {0} - {1} - {2}", resource.ResourceType, resource.Culture.Name, resource.ResourceKey), resource);
		}

		private void Trace(int id, TraceEventType type, string identifier, string description, Resource resource)
		{
			if (_cache.TryAdd(new Tuple<string, string, string>(resource.ResourceType, resource.Culture.Name, resource.ResourceKey), identifier))
				TraceSource.TraceEvent(id, new ResourceTraceRecord(type, identifier, description) { Resource = resource });
		}

		private void ClearCache(string resourceType)
		{
			foreach (var item in _cache.Where(a => string.Equals(a.Key.Item1, resourceType, StringComparison.InvariantCultureIgnoreCase)))
			{
				string resource;
				_cache.TryRemove(item.Key, out resource);
			}
		}
	}
}
