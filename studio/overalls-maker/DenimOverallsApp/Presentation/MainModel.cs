using Uno.Extensions.Navigation;
using Uno.Extensions.Reactive;

namespace DenimOverallsApp.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(true)]
public partial record MainModel(INavigator Navigator)
{
    // ── Option catalogs (bound by the option cards) ───────────────────────────
    public IReadOnlyList<OverallOption> LengthOptions { get; } = OverallCatalog.Lengths;
    public IReadOnlyList<OverallOption> BibOptions { get; } = OverallCatalog.Bibs;
    public IReadOnlyList<DenimColorOption> ColorOptions { get; } = OverallCatalog.Colors;
    public IReadOnlyList<OverallOption> PocketOptions { get; } = OverallCatalog.Pockets;

    public decimal BasePrice { get; } = OverallCatalog.BasePrice;

    // ── Live state ──────────────────────────────────────────────────────────

    /// <summary>The complete, priced configuration — the single source of truth for selection.</summary>
    public IState<OverallConfiguration> Configuration => State.Value(this, OverallCatalog.CreateDefault);

    /// <summary>Embroidery text, editable live from the View (no price impact). Mirrored into the configuration so it flows through to the order summary.</summary>
    public IState<string> CustomText => State.Value(this, () => string.Empty).ForEach(SyncCustomText);

    private ValueTask SyncCustomText(string? text, CancellationToken ct)
        => Configuration.UpdateAsync(c => c is null ? c : c with { CustomText = text ?? string.Empty }, ct);

    /// <summary>Whether the Uno Platform logo is shown on the bib (on by default). Mirrored into the configuration so the −$20 patch discount flows through to the summary.</summary>
    public IState<bool> ShowLogo => State.Value(this, () => true).ForEach(SyncLogo);

    private ValueTask SyncLogo(bool on, CancellationToken ct)
        => Configuration.UpdateAsync(c => c is null ? c : OverallCatalog.Reprice(c with { HasLogo = on }), ct);

    // ── Selection commands ─────────────────────────────────────────────────────
    public ValueTask SelectLength(OverallOption option)
        => Configuration.UpdateAsync(c => c is null ? c : OverallCatalog.Reprice(c with { LengthOption = option.Id }));

    public ValueTask SelectBib(OverallOption option)
        => Configuration.UpdateAsync(c => c is null ? c : OverallCatalog.Reprice(c with { BibType = option.Id }));

    public ValueTask SelectColor(DenimColorOption option)
        => Configuration.UpdateAsync(c => c is null ? c : OverallCatalog.Reprice(c with { DenimColor = option.Id, DenimColorHex = option.HexColor }));

    public ValueTask SelectPocket(OverallOption option)
        => Configuration.UpdateAsync(c => c is null ? c : OverallCatalog.Reprice(c with { PocketType = option.Id }));

    // ── Selectable lists (drive the option cards + their selected highlight) ────
    public IFeed<ImmutableList<SelectableOption>> Lengths =>
        Configuration.Select(c => LengthOptions.Select(o => new SelectableOption(o, o.Id == c.LengthOption)).ToImmutableList());

    public IFeed<ImmutableList<SelectableOption>> Bibs =>
        Configuration.Select(c => BibOptions.Select(o => new SelectableOption(o, o.Id == c.BibType)).ToImmutableList());

    public IFeed<ImmutableList<SelectableColor>> Colors =>
        Configuration.Select(c => ColorOptions.Select(o => new SelectableColor(o, o.Id == c.DenimColor)).ToImmutableList());

    public IFeed<ImmutableList<SelectableOption>> Pockets =>
        Configuration.Select(c => PocketOptions.Select(o => new SelectableOption(o, o.Id == c.PocketType)).ToImmutableList());

    // ── Header / footer price ─────────────────────────────────────────────────
    public IFeed<string> PriceLabel => Configuration.Select(c => OverallCatalog.Format(c.TotalPrice));

    // ── Preview helpers ─────────────────────────────────────────────────────
    public IFeed<string> SelectedColorHex  => Configuration.Select(c => c.DenimColorHex);

    // Raw selection ids that drive the shared OverallPreview control.
    public IFeed<string> LengthId => Configuration.Select(c => c.LengthOption);
    public IFeed<string> BibId    => Configuration.Select(c => c.BibType);
    public IFeed<string> PocketId => Configuration.Select(c => c.PocketType);

    public IFeed<string> SelectedColorName => Configuration.Select(c => OverallCatalog.ColorLabel(c.DenimColor));
    public IFeed<string> PreviewLengthLabel => Configuration.Select(c => OverallCatalog.LengthLabel(c.LengthOption));
    public IFeed<string> PreviewBibLabel    => Configuration.Select(c => OverallCatalog.BibLabel(c.BibType));
    public IFeed<string> PreviewPocketLabel => Configuration.Select(c => OverallCatalog.PocketLabel(c.PocketType));

    /// <summary>Leg length of the rendered overall — shorter for the "short" cut.</summary>
    public IFeed<double> LegHeight => Configuration.Select(c => c.LengthOption == "short" ? 96d : 208d);

    /// <summary>Discrete length variants for the single-coordinate ("SVG") figure: each length has its own continuous body geometry.</summary>
    public IFeed<bool> IsLongLength  => Configuration.Select(c => c.LengthOption == "long");
    public IFeed<bool> IsShortLength => Configuration.Select(c => c.LengthOption == "short");

    public IFeed<bool> ShowBibPanel          => Configuration.Select(c => c.BibType != "crossback");
    public IFeed<bool> ShowSingleChestPocket => Configuration.Select(c => c.BibType == "classic");
    public IFeed<bool> ShowDoubleChestPocket => Configuration.Select(c => c.BibType == "wide");
    public IFeed<bool> ShowCrossStraps       => Configuration.Select(c => c.BibType == "crossback");

    /// <summary>Distinct bib silhouettes for the "SVG" figure (classic / wide / scoop).</summary>
    public IFeed<bool> IsClassicBib => Configuration.Select(c => c.BibType == "classic");
    public IFeed<bool> IsWideBib    => Configuration.Select(c => c.BibType == "wide");
    public IFeed<bool> IsScoopBib   => Configuration.Select(c => c.BibType == "scoop");
    public IFeed<bool> ShowThighPockets      => Configuration.Select(c => c.PocketType is "patch" or "cargo");

    // ── Navigation ────────────────────────────────────────────────────────────
    /// <summary>Carries the current configuration to the order summary.</summary>
    public async ValueTask ReviewOrder(CancellationToken ct)
    {
        var configuration = await Configuration;
        if (configuration is not null)
        {
            await Navigator.NavigateRouteAsync(this, "Summary", data: configuration, cancellation: ct);
        }
    }
}
