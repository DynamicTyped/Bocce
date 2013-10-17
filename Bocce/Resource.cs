using System.Globalization;

namespace Bocce
{
    internal class Resource
    {
        private CultureInfo _cultureInfo = CultureInfo.CurrentUICulture;
        public CultureInfo Culture
        {
            get { return _cultureInfo; }
            set { _cultureInfo = value; }
        }
        public string ResourceKey { get; set; }
        public string ResourceType { get; set; }
    }
}
