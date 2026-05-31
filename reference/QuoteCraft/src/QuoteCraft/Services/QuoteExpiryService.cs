namespace QuoteCraft.Services;

/// <summary>
/// Background service that periodically checks for quotes past their valid_until date
/// and auto-expires them. Also sends reminder notifications 3 days before expiry.
/// </summary>
public class QuoteExpiryService : IDisposable
{
    private readonly IQuoteRepository _quoteRepo;
    private readonly INotificationService _notificationService;
    private readonly IStatusHistoryRepository _statusHistoryRepo;
    private readonly ILogger<QuoteExpiryService> _logger;
    private Timer? _timer;

    public QuoteExpiryService(
        IQuoteRepository quoteRepo,
        INotificationService notificationService,
        IStatusHistoryRepository statusHistoryRepo,
        ILogger<QuoteExpiryService> logger)
    {
        _quoteRepo = quoteRepo;
        _notificationService = notificationService;
        _statusHistoryRepo = statusHistoryRepo;
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

            // Targeted query: only quotes past valid_until (no full table scan)
            var expirable = await _quoteRepo.GetExpirableAsync(now);

            foreach (var q in expirable)
            {
                q.Status = QuoteStatus.Expired;
                q.UpdatedAt = DateTimeOffset.UtcNow;
                await _quoteRepo.SaveAsync(q);
                await _statusHistoryRepo.RecordAsync(q.Id, "Expired", "system");

                _logger.LogInformation("Auto-expired quote {QuoteNumber}", q.QuoteNumber);
            }

            // Send reminder notifications for quotes expiring within 3 days
            // Reuse targeted query: quotes that will expire between now and now+3days
            var expiringIn3Days = await _quoteRepo.GetExpirableAsync(now.AddDays(3));
            expiringIn3Days = expiringIn3Days.Where(q =>
                q.ValidUntil.HasValue && q.ValidUntil.Value > now).ToList();

            foreach (var q in expiringIn3Days)
            {
                var daysLeft = (int)Math.Ceiling((q.ValidUntil!.Value - now).TotalDays);
                await _notificationService.CreateAsync(
                    NotificationType.QuoteExpiring,
                    "Quote Expiring Soon",
                    $"Quote #{q.QuoteNumber} expires in {daysLeft} day{(daysLeft != 1 ? "s" : "")}.",
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
