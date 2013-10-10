using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace Bocce.Test
{
    public static class UnitTestHelper
    {
        public const string ResourceType = "DBResourceProviderTest";
        public static readonly IDbConnection _connection;

        static UnitTestHelper()
        {
            _connection =
                new SqlConnection(ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString);

            _connection.Open();
        }

        public static void CleanUpConnection()
        {
            if (null != _connection)
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();

                _connection.Dispose();
            }
            
        }

        public static void SetupTable()
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

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
            _connection.Execute(string.Format("INSERT INTO WebLocalization.Resources VALUES ('{0}','{1}','{2}','{3}')",
                                              resourceType, cultureCode, resourceKey, resourceValue));
        }

        public static void CleanUp()
        {
            CleanUpTable();
            Dispose();
        }

        public static void CleanUpTable()
        {
            _connection.Execute(string.Format("DELETE FROM WebLocalization.Resources WHERE resource_type = '{0}'", ResourceType));
        }

        private static void Dispose()
        {
            if (null != _connection)
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();

                _connection.Dispose();
            }
        }
    }
}
