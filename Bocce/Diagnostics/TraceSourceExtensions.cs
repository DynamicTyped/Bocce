using System;
using System.Diagnostics;
using System.Xml.XPath;

namespace Bocce.Diagnostics
{
    internal static class TraceSourceExtensions
    {
        public static void TraceEvent(this TraceSource traceSource, int id, TraceRecord trace)
        {
            if (traceSource == null) { throw new ArgumentNullException("traceSource"); }
            if (trace == null) { throw new ArgumentNullException("trace"); }

            traceSource.TraceData(trace.Severity, id, trace.ToXElement().CreateNavigator());
        }
    }
}
