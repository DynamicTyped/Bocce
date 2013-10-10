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
        private string m_classKey;
        private string m_assemblyName;

        private ResourceManager m_resourceManager;

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
                string[] textArray = classKey.Split('|');
                this.m_assemblyName = textArray[0];
                this.m_classKey = textArray[1];
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
            if (this.m_resourceManager == null)
            {
                lock (this)
                {
                    Assembly asm = Assembly.Load(this.m_assemblyName);
                    ResourceManager rm = new ResourceManager(String.Format(CultureInfo.InvariantCulture, "{0}.{1}", this.m_assemblyName, this.m_classKey), asm);
                    this.m_resourceManager = rm;
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
        public object GetObject(string resourceKey, System.Globalization.CultureInfo culture)
        {
            Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "GlobalExternalResourceProvider.GetObject({0}, {1})", resourceKey, culture));

            this.EnsureResourceManager();

            if (culture == null)
            {
                culture = CultureInfo.CurrentUICulture;
            }
            return this.m_resourceManager.GetObject(resourceKey, culture);

        }

        /// <summary>
        /// Implicit expressions are not supported by this provider 
        /// therefore a ResourceReader need not be implemented.
        /// Throws a NotSupportedException.
        /// </summary>
        public System.Resources.IResourceReader ResourceReader
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
