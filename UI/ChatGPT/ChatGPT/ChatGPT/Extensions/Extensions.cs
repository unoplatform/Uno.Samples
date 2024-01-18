using System.Collections.Generic;

namespace ChatGPT.Extensions;

internal static class Extensions
{
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
