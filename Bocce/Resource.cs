using System.Globalization;

namespace Bocce
{
    internal class Resource
    {
        public CultureInfo Culture { get; set; }
        public string ResourceKey { get; set; }
        public string ResourceType { get; set; }
        public string Key
        {
            get { return Culture == null ? CultureInfo.CurrentUICulture.Name : Culture.Name + ResourceKey + ResourceType; }
        }
    }
}
