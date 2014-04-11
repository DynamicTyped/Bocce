Bocce - Database-driven resource provider.
===== 

Uses a database to retrive resources. Has the ability to fall back. For example if you ask for fr-FR and it doesn't exist but the same key
does exist for fr, then it will use that value.  To optimize performance, this function caches values in a dictionary per culture 

Table Definition
----------------
```sql
CREATE TABLE [dbo].[Localization](
	[resource_type] [nvarchar](256) NOT NULL,
	[culture_code] [nvarchar](10) NOT NULL,
	[resource_key] [nvarchar](128) NOT NULL,
	[resource_value] [nvarchar](4000) NOT NULL,
 CONSTRAINT [PK_ResourcesResources] PRIMARY KEY CLUSTERED 
(
	[resource_type] ASC,
	[culture_code] ASC,
	[resource_key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
```

Config
------
```csharp
<configSections>
    <section 
        name="dbResourceProvider" 
        type="Bocce.Configuration.DbResourceProviderSection, Bocce" />
</configSections>
<dbResourceProvider databaseName="SqlConnectionString" defaultCulture="en-US" schema="dbo" tableName="Localization" traceMatches="false" />
```

Usage
------
```csharp
var resourceType = "HelpPage"
var provider = new DbResourceProvider(
				resourceType,
				CultureInfo.InstalledUICulture,
                new DbResourceAccessor("Data Source=.;Initial Catalog=TestDb;Integrated Security=SSPI", "dbo", "Localization"));
                
var cultureInfo = new CultureInfo("fr-FR");
var resourceKey = "keyName";
var resourceValue = dBResourceProvider.GetObject(resourceKey, cultureInfo);                
                
```

If your data changes but items are cached you have the ability to clear out the dictionaires.

To wipe the entire set:
```csharp
DbResourceProvider.ClearAll();
```

For a given provider:
```csharp
provider.Clear();
```

To use in a web project a few more configs are needed and some DI, we like [ninject](http://www.ninject.org/).

Config
```csharp
<globalization uiCulture="auto" culture="auto" resourceProviderFactoryType="Bocce.DbResourceProviderFactory, Bocce, Version=1.0.0.0, Culture=neutral" />
```

DI
```csharp
var globalizationSection = (GlobalizationSection)ConfigurationManager.GetSection("system.web/globalization") ?? new GlobalizationSection();
kernel.Bind<ResourceProviderFactory>().To(Type.GetType(globalizationSection.ResourceProviderFactoryType));
```

We then made a string extention
```csharp
public static string Localize(this string resourceKey, string resourceType, string culture, HttpContextBase context = null)
{
	var cultureInfo = CultureInfo.GetCultureInfo(culture);
	var value = context == null ?
		HttpContext.GetGlobalResourceObject(resourceType, resourceKey, cultureInfo) as string
		: context.GetLocalResourceObject(resourceType, resourceKey, cultureInfo).ToString();

	return value;

}
```

And then in the site we can do:
```csharp
var text ="Some Text To localization".Localize();
```

Finally an HTML helper:
```csharp
public static MvcHtmlString Localize(this HtmlHelper htmlHelper, string resourceType, string resourceKey)
{
	// Get the resource using HttpContext, which uses the ResourceProviderFactory specified in the configuration file
	return new MvcHtmlString(resourceKey.Localize(resourceType));
}
```
Used in a view:
```csharp
// Localize a string using current culture
@Html.Localize("resourceType", "resourceKey")
// Localize a string specifying a culture
@Html.Localize("resourceType", "resourceKey", "en-GB")
```

Tracing
-------
You have the ability to log various events for analysis. You set a [listener](http://msdn.microsoft.com/en-us/library/1txedc80(v=vs.110).aspx).

The switch value will determine what gets written. 

* Error - Resource miss
* Warning - Resource fallback and miss
* Information - Resource clear, fallback, and misses
* Verbose - Resource hit (only when tracematches = true), clear, fallback, and miss

Bocce also has a datbase listener to log to a database.

Table creation scrip below. Schema and table name are up to you.

```sql
CREATE TABLE [dbo].[ResourceTrace](
	[trace_key] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[event_type] [nvarchar](50) NOT NULL,
	[id] [varchar](50) NOT NULL,
	[data] [nvarchar](2000) NOT NULL,
	[insert_date] [datetime] NOT NULL,
 CONSTRAINT [PK_Trace] PRIMARY KEY CLUSTERED 
(
	[trace_key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [WebLocalization].[ResourceTrace] ADD  CONSTRAINT [DF_Trace_insert_date]  DEFAULT (getdate()) FOR [insert_date]
GO
```

Config:

```csharp
<system.diagnostics>
	<sources>
	    <source name="Bocce" switchValue="Verbose">
	        <listeners>
	            <add name="databaseListner" type="Bocce.Diagnostics.DatabaseListener, Bocce" initializeData="[dbo].[ResourceTrace]"/>
	        </listeners>
	    </source>
	</sources>
	<trace autoflush="true"/>
</system.diagnostics>
```
The initializeData filed is where you set the name of your table including the schema. It will use the same connection string defined for your resource table.
