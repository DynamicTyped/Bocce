using System;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
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
                var localData = data.ToString();

                // Try and match data to a TraceRecord and so we an write better output. ToString() on data is putting the running app info on.
                // possible future enhancement of breaking it back to trace record and writing more discrete fields
                try
                {
                    var navigator = data as XPathNavigator;
                    if (navigator != null)
                    {
                        var xElement = navigator.UnderlyingObject as XElement;
                        
                        if (xElement != null)
                        { 
                            var singleOrDefault = xElement.Elements().SingleOrDefault(a => a.Name.LocalName == "Description");
                            if (singleOrDefault != null)
                            {
                                localData = singleOrDefault.Value;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //reset
                    localData = data.ToString();
                }

                connection.Execute(string.Format("INSERT INTO {0} ([name], event_type, [id], data) VALUES (@a, @b, @c, @d)", _table), new { a = source, b = eventType.ToString(), c = id, d = localData });
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
