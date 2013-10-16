using System.Diagnostics;
using Bocce.Diagnostics;

namespace Bocce.Notifications
{
    internal class DiagnosticTrace
    {
        private static readonly TraceSource TraceSource = new TraceSource("Bocce");

        public void ResourceMiss(string cultureCode, string key, string resourceType)
        {
            Trace(1001, TraceEventType.Error, "ResourceMiss" ,string.Format("{0} - {1} - {2}", resourceType, cultureCode, key));
        }

        public void ResourceFellBack(string cultureCode, string key, string resourceType)
        {
            Trace(1001, TraceEventType.Warning, "ResourceFellBack", string.Format("{0} - {1} - {2}", resourceType, cultureCode, key));
        }

        public void ResourceCleared(string resourceType)
        {
            Trace(1001, TraceEventType.Information, "ResourceCleared", resourceType);
        }

        public void ResourceHit(string cultureCode, string key, string resourceType)
        {
            Trace(1001, TraceEventType.Verbose, "ResourceMiss", string.Format("{0} - {1} - {2}", resourceType, cultureCode, key));
        }

        private static void Trace(int id, TraceEventType type, string identifier, string description)
        {
            TraceSource.TraceEvent(id, new TraceRecord(type, identifier, description));
        }

    }
}
