using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Bocce
{
	public class DbResourceAccessor : IDbResourceAccessor
	{
		public string ConnectionString { get; set; }

		private string _getResourcesCommand =
			@"SELECT resource_key, resource_value
				FROM WebLocalization.Resources
				WHERE resource_type = @resource_type
				AND culture_code = @culture_code";

		public string GetResourcesCommand
		{
			get { return _getResourcesCommand; }
			set { _getResourcesCommand = value; }
		}

		public DbResourceAccessor(string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString)) { throw new ArgumentNullException("connectionString"); }

			ConnectionString = connectionString;
		}

		public IEnumerable<KeyValuePair<string, string>> GetResources(string resourceType, string cultureCode)
		{
			using (var connection = new SqlConnection(ConnectionString))
			using (var command = connection.CreateCommand())
			{
				// Initialize the command
				command.CommandText = GetResourcesCommand;
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("resource_type", resourceType);
				command.Parameters.AddWithValue("culture_code", cultureCode);

				// Execute the command
				connection.Open();

				using (var reader = command.ExecuteReader(CommandBehavior.SingleResult))
				{
					while (reader.Read())
					{
						yield return new KeyValuePair<string, string>(reader.GetString(0), reader.GetString(1));
					}
				}
			}
		}
	}
}