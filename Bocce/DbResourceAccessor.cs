using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Bocce
{
	public class DbResourceAccessor : IDbResourceAccessor
	{
		public string ConnectionString { get; set; }
	    public string Schema { get; set; }
	    public string Table { get; set; }

		private string _getResourcesCommand =
			@"SELECT resource_key, resource_value
				FROM @schema.@table
				WHERE resource_type = @resource_type
				AND culture_code = @culture_code";

		public string GetResourcesCommand
		{
			get { return _getResourcesCommand.Replace("@schema", Schema.QuoteSqlName()).Replace("@table", Table.QuoteSqlName()); }
			set { _getResourcesCommand = value; }
		}

		public DbResourceAccessor(string connectionString, string schema, string table)
		{
			if (string.IsNullOrEmpty(connectionString)) { throw new ArgumentNullException("connectionString"); }
            if (string.IsNullOrEmpty(schema)) { throw new ArgumentException("schema"); }
            if (string.IsNullOrEmpty(table)) { throw new ArgumentException("table"); }

			ConnectionString = connectionString;
		    Schema = schema;
		    Table = table;
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