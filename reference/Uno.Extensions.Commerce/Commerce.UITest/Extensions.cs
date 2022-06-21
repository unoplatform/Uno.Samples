using System;
using Uno.UITest;
using Uno.UITest.Helpers.Queries;

namespace Uno.Gallery.UITests;

public static class Extensions
{
	public static Func<IAppQuery, IAppQuery> WaitThenTap(this IApp app, Func<IAppQuery, IAppQuery> query, TimeSpan? timeout = null)
	{
		app.WaitForElement(query, timeout: timeout);
		app.Tap(query);

		return query;
	}

	public static Func<IAppQuery, IAppQuery> WaitThenTap(this IApp app, string marked, TimeSpan? timeout = null)
		=> WaitThenTap(app, q => q.All().Marked(marked), timeout);

	public static QueryEx ToQueryEx(this Func<IAppQuery, IAppQuery> query) => new QueryEx(query);
}
