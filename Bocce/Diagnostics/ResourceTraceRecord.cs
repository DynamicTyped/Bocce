using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

namespace Bocce.Diagnostics
{
    class ResourceTraceRecord : TraceRecord
    {
        public ResourceTraceRecord(TraceEventType severity, string traceIdentifier, string description)
			: base(severity, traceIdentifier, description) { }

        public Resource Resource { get; set; }

		protected override IEnumerable<XElement> GetExtendedData()
		{
			foreach (var item in base.GetExtendedData()) { yield return item; }

			yield return new XElement(
				"Notification",
				new XElement("CultureCode", Resource.Culture.Name),
				new XElement("ResourceType", Resource.ResourceType),
				new XElement("ResourceKey", Resource.ResourceKey));
		}
    }
}
