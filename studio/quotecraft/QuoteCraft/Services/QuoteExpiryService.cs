namespace QuoteCraft.Services;

/// <summary>
/// Expires Sent and Viewed quotes past their valid-until date (PRD F17.1) and adds one reminder notification per quote
/// in the 3 days before (F17.2). App.OnLaunched starts it; it checks at once and then every 5 minutes. The rules are
/// <see cref="QuoteStatusService"/>'s.
/// </summary>
public class QuoteExpiryService : IDisposable
{
    private readonly IQuoteRepository _quoteRepo;
    private readonly QuoteStatusService _status;
    private readonly INotificationService _notificationService;
    private readonly ILogger<QuoteExpiryService> _logger;
    private Timer? _timer;

    public QuoteExpiryService(
        IQuoteRepository quoteRepo,
        QuoteStatusService status,
        INotificationService notificationService,
        ILogger<QuoteExpiryService> logger)
    {
        _quoteRepo = quoteRepo;
        _status = status;
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Start periodic expiry checks every 5 minutes.
    /// </summary>
    public void Start()
    {
        _timer?.Dispose();
        _timer = new Timer(async _ => await CheckExpirationsAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
    }

    public void Stop()
    {
        _timer?.Dispose();
        _timer = null;
    }

    public async Task CheckExpirationsAsync()
    {
        try
        {
            var now = DateTimeOffset.UtcNow;

            foreach (var q in await _status.ExpireOverdueAsync(now))
            {
                _logger.LogInformation("Auto-expired quote {QuoteNumber}", q.QuoteNumber);
            }

            // One reminder per quote and valid-until date: the check runs every 5 minutes, and a quote reopened or given
            // a new date in the editor has a new one, so "a reminder since 3 days before this date" is the test.
            foreach (var q in await _quoteRepo.GetExpirableAsync(now.AddDays(3)))
            {
                if (q.ValidUntil is not { } validUntil || validUntil <= now
                    || await _notificationService.ExistsAsync(NotificationType.QuoteExpiring, q.Id, validUntil.AddDays(-3)))
                {
                    continue;
                }

                // Calendar days in UTC, as the date is stored and shown: a quote valid through Friday expires "in 2
                // days" on Wednesday, whatever the time.
                var daysLeft = (validUntil.UtcDateTime.Date - now.UtcDateTime.Date).Days;
                var when = daysLeft <= 0 ? "today" : $"in {daysLeft} day{(daysLeft != 1 ? "s" : "")}";
                await _notificationService.CreateAsync(
                    NotificationType.QuoteExpiring,
                    "Quote Expiring Soon",
                    $"Quote #{q.QuoteNumber} expires {when}.",
                    q.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Quote expiry check failed");
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
