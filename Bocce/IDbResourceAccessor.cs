using System.Collections.Generic;

namespace Bocce
{
	public interface IDbResourceAccessor
	{
		string ConnectionString { get; set; }
		string GetResourcesCommand { get; set; }
		IEnumerable<KeyValuePair<string, string>> GetResources(string resourceType, string cultureCode);
	}
}
