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
}
