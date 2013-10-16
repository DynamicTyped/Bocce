using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace Bocce.Diagnostics
{
    class TraceRecord
    {
// ReSharper disable InconsistentNaming
        private static readonly XNamespace _defaultNamespace = "http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord";
// ReSharper restore InconsistentNaming

        public static XNamespace DefaultNamespace { get { return _defaultNamespace; } }

        public TraceRecord(TraceEventType severity, string traceIdentifier, string description)
        {
            Severity = severity;
            TraceIdentifier = traceIdentifier;
            Description = description;
        }

        public TraceEventType Severity { get; set; }
        public string TraceIdentifier { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public Exception Exception { get; set; }

        public virtual XElement ToXElement()
        {
            var element = new XElement(
                _defaultNamespace + "TraceRecord",
                new XAttribute("Severity", Severity),
                new XElement(_defaultNamespace + "TraceIdentifier", TraceIdentifier),
                new XElement(_defaultNamespace + "Description", Description),
                new XElement(_defaultNamespace + "AppDomain", AppDomain.CurrentDomain.FriendlyName));

            if (Source != null)
            {
                element.SetElementValue(_defaultNamespace + "Source", Source);
            }

            var extendedData = GetExtendedData();

            if (extendedData != null)
            {
                element.Add(new XElement(_defaultNamespace + "ExtendedData", extendedData));
            }

            var exception = ConvertException(_defaultNamespace + "Exception", Exception);

            if (exception != null)
            {
                element.Add(exception);
            }

            return element;
        }

        protected virtual IEnumerable<XElement> GetExtendedData()
        {
            return _extendedData.IsValueCreated
                ? _extendedData.Value
                : XElement.EmptySequence;
        }

        private readonly Lazy<Collection<XElement>> _extendedData = new Lazy<Collection<XElement>>(() => new Collection<XElement>());

        public Collection<XElement> ExtendedData { get { return _extendedData.Value; } }

        private static XElement ConvertException(XName name, Exception exception)
        {
            if (exception == null) return null;

            var win32Exception = exception as Win32Exception;

            var element = new XElement(
                name,
                new XElement(_defaultNamespace + "ExceptionType", exception.GetType().AssemblyQualifiedName),
                new XElement(_defaultNamespace + "Message", exception.Message),
                new XElement(_defaultNamespace + "StackTrace", exception.StackTrace),
                new XElement(_defaultNamespace + "ExceptionString", exception.ToString()),
                win32Exception != null ? new XElement(_defaultNamespace + "NativeErrorCode", win32Exception.NativeErrorCode) : null,
                exception.Data != null
                    ? from DictionaryEntry x in exception.Data
                      select new XElement(
                          _defaultNamespace + "Data",
                          new XElement(_defaultNamespace + "Key", x.Key),
                          new XElement(_defaultNamespace + "Value", x.Value))
                    : null,
                ConvertException(_defaultNamespace + "InnerException", exception.InnerException));

            return element;
        }
    }
}
