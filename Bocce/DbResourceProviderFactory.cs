using System.Globalization;
using System.Web.Compilation;
using Bocce.Configuration;

namespace Bocce
{
	public class DbResourceProviderFactory : ResourceProviderFactory
	{
		public override IResourceProvider CreateGlobalResourceProvider(string classKey)
		{
			var config = DbResourceProviderSection.GetSection();
			var accessor = new DbResourceAccessor(config.ConnectionString);
			CultureInfo defaultCulture = CultureInfo.InvariantCulture;

			if (config.DefaultCulture != null)
			{
				defaultCulture = CultureInfo.GetCultureInfo(config.DefaultCulture);
			}

			return new DbResourceProvider(classKey, defaultCulture, accessor);
		}

		public override IResourceProvider CreateLocalResourceProvider(string virtualPath)
		{
			// we should always get a path from the runtime
			var classKey = virtualPath;

			// TODO: Figure out why they're doing this.
			if (!string.IsNullOrEmpty(virtualPath))
			{
				virtualPath = virtualPath.Remove(0, 1);
				classKey = virtualPath.Remove(0, virtualPath.IndexOf('/') + 1);
			}

			return CreateGlobalResourceProvider(classKey);
		}
	}
}