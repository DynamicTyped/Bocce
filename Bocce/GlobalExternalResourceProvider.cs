using System;
using System.Resources;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Reflection;
using System.Web.Compilation;

namespace Bocce
{
    /// <summary>
    /// Resource provider for accessing external resources. 
    /// </summary>
    public class GlobalExternalResourceProvider : IResourceProvider
    {
        private readonly string _mClassKey;
        private readonly string _mAssemblyName;

        private ResourceManager _mResourceManager;

        /// <summary>
        /// Constructs an instance of the provider with the specified
        /// assembly name and resource type. 
        /// </summary>
        /// <param name="classKey">The assembly name and 
        /// resource type in the format [assemblyName]|
        /// [resourceType]</param>
        public GlobalExternalResourceProvider(string classKey)
        {
            if (classKey == null)
                throw new ArgumentNullException("classKey");

            Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "GlobalExternalResourceProvider({0})", classKey));

            if (classKey.IndexOf('|') > 0)
            {
                var textArray = classKey.Split('|');
                _mAssemblyName = textArray[0];
                _mClassKey = textArray[1];
            }
            else
            {
                throw new ArgumentException(String.Format(Thread.CurrentThread.CurrentUICulture, "Parameter invalid: {0}. Must provide a resource assembly name and resource type in the format '[AssemblyName]|[ResourceType]'.", classKey));
            }
        }

        /// <summary>
        /// Loads the resource assembly and creates a 
        /// ResourceManager instance to access its resources.
        /// If the assembly is already loaded, Load returns a reference
        /// to that assembly.
        /// </summary>
        private void EnsureResourceManager()
        {
            if (_mResourceManager == null)
            {
                lock (this)
                {
                    var asm = Assembly.Load(_mAssemblyName);
                    var rm = new ResourceManager(String.Format(CultureInfo.InvariantCulture, "{0}.{1}", _mAssemblyName, _mClassKey), asm);
                    _mResourceManager = rm;
                }
            }
        }

        #region IResourceProvider Members

        /// <summary>
        /// Retrieves resources using a ResourceManager instance
        /// for the assembly and resource key of this provider 
        /// instance. 
        /// </summary>
        /// <param name="resourceKey">The resource key to find.</param>
        /// <param name="culture">The culture to find.</param>
        /// <returns>The resource value if found.</returns>
        public object GetObject(string resourceKey, CultureInfo culture)
        {
            Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "GlobalExternalResourceProvider.GetObject({0}, {1})", resourceKey, culture));

            EnsureResourceManager();

            if (culture == null)
            {
                culture = CultureInfo.CurrentUICulture;
            }
            return _mResourceManager.GetObject(resourceKey, culture);

        }

        /// <summary>
        /// Implicit expressions are not supported by this provider 
        /// therefore a ResourceReader need not be implemented.
        /// Throws a NotSupportedException.
        /// </summary>
        public IResourceReader ResourceReader
        {
            get
            {
                Debug.WriteLine("GlobalExternalResourceProvider.get_ResourceReader()");

                throw new NotSupportedException();
            }

        }

        #endregion IResourceProvider Members
    }
}
