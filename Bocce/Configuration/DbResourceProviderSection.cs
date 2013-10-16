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

		private const string DatabaseNameProperty = "databaseName";

		[ConfigurationProperty(DatabaseNameProperty, IsRequired = true)]
		public string DatabaseName
		{
			get { return (string)this[DatabaseNameProperty]; }
			set { this[DatabaseNameProperty] = value; }
		}

		private const string GetResourcesCommandProperty = "getResourcesCommand";

		[ConfigurationProperty(GetResourcesCommandProperty, IsRequired = false)]
		public string GetResourcesCommand
		{
			get { return (string)this[GetResourcesCommandProperty]; }
			set { this[GetResourcesCommandProperty] = value; }
		}

		private const string DefaultCultureProperty = "defaultCulture";

		[ConfigurationProperty(DefaultCultureProperty, IsRequired = false)]
		public string DefaultCulture
		{
			get { return (string)this[DefaultCultureProperty]; }
			set { this[DefaultCultureProperty] = value; }
		}

		internal string ConnectionString
		{
			get { return ConfigurationManager.ConnectionStrings[DatabaseName].ConnectionString; }
		}

	    private const string SchemaNameProperty = "schema";
	    [ConfigurationProperty(SchemaNameProperty, IsRequired = true)]
	    public string SchemaName
	    {
            get { return (string) this[SchemaNameProperty]; }
            set { this[SchemaNameProperty] = value; }
	    }

	    private const string TableNameProperty = "tableName";
	    [ConfigurationProperty(TableNameProperty, IsRequired = true)]
	    public string TableName
	    {
            get { return (string)this[TableNameProperty]; }
            set { this[TableNameProperty] = value; }
	    }

	    private const string TraceMatchesProperty = "traceMatches";
	    [ConfigurationProperty(TraceMatchesProperty, DefaultValue = false)]
	    public bool TraceMatches
	    {
            get { return (bool) this[TraceMatchesProperty]; }
            set { this[TraceMatchesProperty] = value; }
	    }
	}
}
