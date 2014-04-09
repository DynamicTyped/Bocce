using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Bocce.Configuration;
using Dapper;

namespace Bocce.Test
{
    public static class UnitTestHelper
    {
        public const string ResourceType = "DBResourceProviderTest";
        public static readonly IDbConnection Connection;
        private static string Schema { get; set; }
        private static string Table { get; set; }

        static UnitTestHelper()
        {
            var config = DbResourceProviderSection.GetSection();
            Schema = config.SchemaName;
            Table = config.TableName;

            Connection =
                new SqlConnection(ConfigurationManager.ConnectionStrings["testDatabase"].ConnectionString);

            Connection.Open();
        }

        public static void CleanUpConnection()
        {
            if (null != Connection)
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();

                Connection.Dispose();
            }
            
        }

        public static void SetupTable()
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            CleanUpTable();

            InsertTestRow(ResourceType, "fr-FR", "get_object_does_not_fallback", "w00t!");
            InsertTestRow(ResourceType, "fr-FR", "get_object_with_null_culture_info_uses_current_ui_culture", "w00t!");
            InsertTestRow(ResourceType, "fr", "get_object_does_not_fallback", "pwnd!");
            InsertTestRow(ResourceType, "fr", "get_object_falls_back_to_neutral_culture", "w00t!");
            InsertTestRow(ResourceType, "en-US", "get_object_does_not_fallback", "pwnd!");
            InsertTestRow(ResourceType, "en-US", "get_object_falls_back_to_default_culture", "w00t!");
            InsertTestRow(ResourceType, "en-US", "get_object_falls_back_to_neutral_culture", "pwnd!");
        }

        private static void InsertTestRow(string resourceType, string cultureCode, string resourceKey, string resourceValue)
        {
            Connection.Execute(string.Format("INSERT INTO {0}.{1} VALUES ('{2}','{3}','{4}','{5}')",
                                              Schema.QuoteSqlName(), Table.QuoteSqlName(), resourceType, cultureCode, resourceKey, resourceValue));
        }

        public static void CleanUp()
        {
            CleanUpTable();
            Dispose();
        }

        public static void CleanUpTable()
        {
            Connection.Execute(string.Format("DELETE FROM {0}.{1} WHERE resource_type = '{2}'", Schema.QuoteSqlName(), Table.QuoteSqlName(), ResourceType));
        }

        private static void Dispose()
        {
            if (null != Connection)
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();

                Connection.Dispose();
            }
        }
    }
}
