using System;
using System.Collections.Generic;
using System.Linq;

namespace gLhb.Parser
{
	public static class EnumerableExtensions
	{
		public static string Join<T>(this IEnumerable<T> source, Func<T, string> transformer, string separator)
		{
			return string.Join(separator, source.Select(transformer));
		}

		public static string Join(this IEnumerable<string> source, string separator)
		{
			return string.Join(separator, source);
		}
	}
}