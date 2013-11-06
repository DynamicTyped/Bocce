using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;

namespace Bocce.Diagnostics
{
    public class DatabaseListener : TraceListener
    {
        private string _table;
        private string _connectionstring { get { return ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString; } }
               
        public DatabaseListener()
        {
        }

        public DatabaseListener(string table)
        {
            _table = table;
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            using(var connection = new SqlConnection(_connectionstring))
            {
                connection.Execute(string.Format("INSERT INTO {0} ([name], event_type, [id], data) VALUES (@a, @b, @c, @d)", _table), new { a = source, b = eventType.ToString(), c = id, d = data.ToString() });
            }
        }

        public override void Write(string message)
        {
            throw new NotImplementedException();
        }

        public override void WriteLine(string message)
        {
            throw new NotImplementedException();
        }
    }
}
