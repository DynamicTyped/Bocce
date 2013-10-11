using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bocce
{
    internal static class StringExtensions
    {
        public static string QuoteSqlName(this string name)
        {
            return name == null ? null : string.Format("[{0}]", name.Replace("]", "]]"));
        }
    }
}
