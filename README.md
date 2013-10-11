Bocce
=====

Bocce is a Database-driven resource provider.


'''csharp
<configSections>
    <section 
        name="dbResourceProvider" 
        type="Bocce.Configuration.DbResourceProviderSection, Bocce" />
</configSections>
<dbResourceProvider databaseName="SqlConnectionString" defaultCulture="en-US" schema="dbo" tableName="Localization" />
'''
