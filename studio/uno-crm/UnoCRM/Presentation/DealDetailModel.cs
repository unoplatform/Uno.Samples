namespace UnoCRM.Presentation;

/// <summary>
/// Backs <see cref="DealDetailPage"/>. Reached from a pipeline deal card via a
/// <c>DataViewMap&lt;DealDetailPage, DealDetailModel, Deal&gt;</c>, so Navigation injects the tapped
/// <see cref="Deal"/> as the constructor's data parameter. A pure projection (no reactive members),
/// so it opts out of the bindable generator; the page navigates back with the "-" qualifier.
/// </summary>
[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record DealDetailModel(Deal Deal);
