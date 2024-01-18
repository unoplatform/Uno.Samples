using System.Collections.Generic;

namespace ChatGPT.Extensions;

internal static class Extensions
{
	public static IImmutableList<T> AddOrUpdate<T>(this IImmutableList<T> list, T item, IEqualityComparer<T>? comparer)
	{
		var index = list.IndexOf(item, comparer);
		return index >= 0
			? list.RemoveAt(index).Insert(index, item)
			: list.Add(item);
	}

	public static async ValueTask UpdateAsync<T>(this IListState<T> listState, T item, CancellationToken ct = default)
		where T : IKeyEquatable<T>
	{
		await listState.Update(items =>
		{
			var index = items.IndexOf(item, KeyEqualityComparer.Find<T>());
			return index >= 0
				? items.RemoveAt(index).Insert(index, item)
				: items;
		}, ct);
	}
}
