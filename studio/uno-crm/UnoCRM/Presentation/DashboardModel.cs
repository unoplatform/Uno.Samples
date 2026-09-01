using UnoCRM.Presentation.Services;

namespace UnoCRM.Presentation;

/// <summary>
/// Backs <see cref="DashboardPage"/>. The whole overview is ONE request to
/// <see cref="ICrmService"/> — which is how a real dashboard loads — cached so every member below
/// shares it instead of re-fetching.
///
/// The KPI texts are scalar projections of that feed, so they bind straight to
/// <c>Text</c> (lesson 39's value path) with no XAML change. The funnel is exposed through
/// <see cref="Overview"/> rather than as a list feed on purpose: the page binds
/// <c>Funnel[0]</c>…<c>Funnel[4]</c> with XAML indexers, which resolve against the materialized
/// <see cref="DashboardData"/> the feed carries but would NOT resolve against an
/// <c>IListFeed</c>. The activity list, which genuinely has an empty state worth designing, is a
/// list feed rendered by a FeedView.
/// </summary>
public partial record DashboardModel(ICrmService Crm)
{
    // Cached: one request for the whole overview, shared by every projection below.
    private IFeed<DashboardData>? _overview;

    /// <summary>
    /// The overview payload. A SCALAR feed, so it is never None even if a collection inside it is
    /// empty — which is what makes <c>Overview.Funnel[0]</c> and the projections below safe with a
    /// plain <c>Select</c>. (Lesson 94 bites the list-feed form, not this one.)
    /// </summary>
    public IFeed<DashboardData> Overview => _overview ??= Feed.Async(Crm.GetDashboardAsync);

    public IFeed<string> TotalLeadsText => Overview.Select(d => d.TotalLeadsText);
    public IFeed<string> TotalLeadsDelta => Overview.Select(d => d.TotalLeadsDelta);
    public IFeed<string> ActiveDealsText => Overview.Select(d => d.ActiveDealsText);
    public IFeed<string> ActiveDealsDelta => Overview.Select(d => d.ActiveDealsDelta);
    public IFeed<string> RevenueText => Overview.Select(d => d.RevenueText);
    public IFeed<string> RevenueDelta => Overview.Select(d => d.RevenueDelta);
    public IFeed<string> ConversionRateText => Overview.Select(d => d.ConversionRateText);
    public IFeed<string> ConversionRateDelta => Overview.Select(d => d.ConversionRateDelta);

    // Its own request, rendered by a FeedView: "no recent activity" is a real state for a quiet
    // account, and a failed feed is worth showing rather than leaving a blank panel.
    private IListFeed<ActivityItem>? _activities;
    public IListFeed<ActivityItem> Activities =>
        _activities ??= ListFeed.Async(Crm.GetActivitiesAsync);
}
