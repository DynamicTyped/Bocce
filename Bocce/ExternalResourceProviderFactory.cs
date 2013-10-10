using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Web.Compilation;

namespace Bocce
{
    /// <summary>
    /// Provider factory implementation for external resources. Only supports
    /// global resources. 
    /// </summary>
    public class ExternalResourceProviderFactory : ResourceProviderFactory
    {
        public override IResourceProvider CreateGlobalResourceProvider(string classKey)
        {
            Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "ExternalResourceProviderFactory.CreateGlobalResourceProvider({0})", classKey));

            return new GlobalExternalResourceProvider(classKey);
        }

        public override IResourceProvider CreateLocalResourceProvider(string virtualPath)
        {
            throw new NotSupportedException(String.Format(Thread.CurrentThread.CurrentUICulture, "{0} does not support local resources.", "ExternalResourceProviderFactory"));
        }
    }
}
