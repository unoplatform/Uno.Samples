using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.UI.Extensions;

namespace ChatGPT;

internal static class Extensions
{
	public static IImmutableList<T> AddOrUpdate<T>(this IImmutableList<T> list, T item, IEqualityComparer<T>? comparer)
	{
		var index = list.IndexOf(item, comparer);
		return index >= 0
			? list.RemoveAt(index).Insert(index, item)
			: list.Add(item);
	}
}
