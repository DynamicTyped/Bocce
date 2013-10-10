using System.Configuration;

namespace Bocce.Configuration
{
	public class DbResourceProviderSection : ConfigurationSection
	{
		public const string SectionName = "dbResourceProvider";

		public static DbResourceProviderSection GetSection()
		{
			return (DbResourceProviderSection)ConfigurationManager.GetSection(SectionName);
		}

		private const string _databaseNameProperty = "databaseName";

		[ConfigurationProperty(_databaseNameProperty, IsRequired = true)]
		public string DatabaseName
		{
			get { return (string)this[_databaseNameProperty]; }
			set { this[_databaseNameProperty] = value; }
		}

		private const string _getResourcesCommandProperty = "getResourcesCommand";

		[ConfigurationProperty(_getResourcesCommandProperty, IsRequired = false)]
		public string GetResourcesCommand
		{
			get { return (string)this[_getResourcesCommandProperty]; }
			set { this[_getResourcesCommandProperty] = value; }
		}

		private const string _defaultCultureProperty = "defaultCulture";

		[ConfigurationProperty(_defaultCultureProperty, IsRequired = false)]
		public string DefaultCulture
		{
			get { return (string)this[_defaultCultureProperty]; }
			set { this[_defaultCultureProperty] = value; }
		}

		internal string ConnectionString
		{
			get { return ConfigurationManager.ConnectionStrings[DatabaseName].ConnectionString; }
		}
	}
}
