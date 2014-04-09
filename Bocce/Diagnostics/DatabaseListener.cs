using System;
using System.Diagnostics;
using System.Data.SqlClient;
using Bocce.Configuration;
using Dapper;

namespace Bocce.Diagnostics
{
    public class DatabaseListener : TraceListener
    {
        private readonly string _table;
        private static string Connectionstring { get { return DbResourceProviderSection.GetSection().ConnectionString; } }
               
        public DatabaseListener()
        {
        }

        public DatabaseListener(string table)
        {
            _table = table;
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            using(var connection = new SqlConnection(Connectionstring))
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
