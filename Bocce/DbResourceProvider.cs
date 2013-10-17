﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Web.Compilation;
using Bocce.Notifications;

namespace Bocce
{
	/// <summary>
	/// Resource provider accessing resources from the database.
	/// This type is thread safe.
	/// </summary>
	public class DbResourceProvider : IResourceProvider, IDisposable
	{
		// Static instance list and lock object
		private static readonly LinkedList<WeakReference> Instances = new LinkedList<WeakReference>();
		private static readonly object InstancesLock = new object();

		private readonly string _resourceType;
		private readonly IDbResourceAccessor _accessor;
		private readonly CultureInfo _defaultCulture = CultureInfo.InvariantCulture;

		// Resource cache and lock object
		private readonly Dictionary<CultureInfo, Dictionary<string, string>> _resourceCache = new Dictionary<CultureInfo, Dictionary<string, string>>();
		private readonly ReaderWriterLockSlim _resourceCacheLock = new ReaderWriterLockSlim();

	    private readonly IDiagnosticTrace _trace;

	    /// <summary>
	    /// Constructs this instance of the provider supplying a resource type for the instance. 
	    /// </summary>
	    /// <param name="resourceType">The resource type.</param>
	    /// <param name="defaultCulture"></param>
	    /// <param name="accessor"></param>
	    public DbResourceProvider(string resourceType, CultureInfo defaultCulture, IDbResourceAccessor accessor)
		{
			if (string.IsNullOrEmpty(resourceType)) { throw new ArgumentNullException("resourceType"); }
			if (defaultCulture == null) { throw new ArgumentNullException("defaultCulture"); }
			if (accessor == null) { throw new ArgumentNullException("accessor"); }

			_resourceType = resourceType;
			_defaultCulture = defaultCulture;
			_accessor = accessor;

			// Add a weak reference to the static instances list
			lock (InstancesLock)
			{
				Instances.AddLast(new WeakReference(this));
			}

            _trace = new DiagnosticTrace();
		}

        internal DbResourceProvider(string resourceType, CultureInfo defaultCulture, IDbResourceAccessor accessor,
                                    IDiagnosticTrace trace)
            : this(resourceType, defaultCulture, accessor)
        {
            _trace = trace;
        }

		#region IResourceProvider Members

		/// <summary>
		/// Retrieves a resource entry based on the specified culture and resource key. The resource type is based on this instance
		/// of the DBResourceProvider as passed to the constructor. To optimize performance, this function caches values in a
		/// dictionary per culture.
		/// </summary>
		/// <param name="resourceKey">The resource key to find.</param>
		/// <param name="culture">The culture to search with.</param>
		/// <returns>If found, the resource string is returned. Otherwise an empty string is returned.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="resourceKey"/> is null or an empty string.</exception>
		public object GetObject(string resourceKey, CultureInfo culture)
		{
			if (resourceKey == null)
			{
				throw new ArgumentNullException(string.Format(CultureInfo.InvariantCulture, "resourceKey ({0})", _resourceType));
			}

			// Default to the current UI culture if none is specified
			culture = culture ?? CultureInfo.CurrentUICulture;

		    var fellBack = false;

			foreach (var cultureInfo in GetFallbackCultures(culture))
			{
			    string resourceValue;
			    if (GetResources(cultureInfo).TryGetValue(resourceKey, out resourceValue))
				{
					if (fellBack)
					{
                        _trace.ResourceFellBack(new Resource(){Culture = culture,ResourceKey = resourceKey, ResourceType = _resourceType});
					}

                    _trace.ResourceHit(new Resource() { Culture = culture, ResourceKey = resourceKey, ResourceType = _resourceType });
					return resourceValue;
				}

				fellBack = true;
			}

            _trace.ResourceMiss(new Resource() { Culture = culture, ResourceKey = resourceKey, ResourceType = _resourceType });
            
			return null;
		}

	    /// <summary>
	    /// Gets the resource fallback path For a given <paramref name="requestedCulture"/>.
	    /// </summary>
	    /// <param name="requestedCulture">The culture for which a fallback path is to be determined.</param>
	    /// <returns>An <see cref="IEnumerable{CultureInfo}"/> that represents the resource fallback path.</returns>
	    /// <exception>
	    ///     <cref>ArgumentNull</cref>
	    ///     <paramref name="requestedCulture"/> is null.</exception>
	    private IEnumerable<CultureInfo> GetFallbackCultures(CultureInfo requestedCulture)
		{
			if (requestedCulture == null) { throw new ArgumentNullException("requestedCulture"); }

			// First, return the requested culture
			yield return requestedCulture;

			// If the requested culture is specific, return the parent (neutral) culture
			if (!requestedCulture.IsNeutralCulture)
			{
				yield return requestedCulture.Parent;
			}

			// Finally, return the default culture for the provider
			yield return _defaultCulture;
		}

	    /// <summary>
	    /// Gets all of the resources for a given culture.
	    /// </summary>
	    /// <param name="culture">The culture for which all of the resources are to be retrieved.</param>
	    /// <returns>A <see>
	    ///                <cref>Dictionary{string, string}</cref>
	    ///            </see>
	    ///     of all of the resources.</returns>
	    private Dictionary<string, string> GetResources(CultureInfo culture)
		{
			_resourceCacheLock.EnterUpgradeableReadLock();

			try
			{
				Dictionary<string, string> resources;

				// Try to get the resources for this culture from the cache. If we missed on the cache, then go get it.
				if (!_resourceCache.TryGetValue(culture, out resources))
				{
					// Upgrade to a writer lock and get the resources
					_resourceCacheLock.EnterWriteLock();

					try
					{
						// Make sure no one has added to the cache while we were upgrading our lock
						if (!_resourceCache.TryGetValue(culture, out resources))
						{
							// Create a new dictionary from the enumerable output of the accessor
							resources = _accessor.GetResources(_resourceType, culture.Name)
								.ToDictionary(a => a.Key, a => a.Value);

							_resourceCache.Add(culture, resources);
						}
					}
					finally
					{
						_resourceCacheLock.ExitWriteLock();
					}
				}

				return resources;
			}
			finally
			{
				_resourceCacheLock.ExitUpgradeableReadLock();
			}
		}

		/// <summary>
		/// Returns a resource reader.
		/// </summary>
		public IResourceReader ResourceReader
		{
			get { return new DbResourceReader(_accessor.GetResources(_resourceType, CultureInfo.CurrentUICulture.Name)); }

		}

		#endregion

		/// <summary>
		/// Clears all cached entries.
		/// </summary>
		public void ClearCache()
		{
			_resourceCacheLock.EnterWriteLock();

			try
			{
				_resourceCache.Clear();
                _trace.ResourceCleared(_resourceType);
			}
			finally
			{
				_resourceCacheLock.ExitWriteLock();
			}
		}
		
		/// <summary>
		/// Clears the cached resources in all instances.
		/// </summary>
		public static void ClearAll()
		{
			// Take a lock on the instance table
			lock (InstancesLock)
			{
				var node = Instances.First;

				// Iterate through each node
				while (node != null)
				{
					// Check to see if the weak reference is still alive
					if (node.Value.IsAlive)
					{
						// If it's alive, then clear the cache for the target instance and move to the next node.
						var provider = (DbResourceProvider)node.Value.Target;
						provider.ClearCache();
						node = node.Next;
					}
					else
					{
						// If it's not alive, then remove the node from our list so we don't hit it again.
						var nodeToRemove = node;
						node = node.Next;
						Instances.Remove(nodeToRemove);
					}
				}
			}
		}

		internal int CacheCount
		{
			get { return _resourceCache.Count; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Close()
		{
			_resourceCacheLock.Dispose();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Close();
			}
		}
	}
}
