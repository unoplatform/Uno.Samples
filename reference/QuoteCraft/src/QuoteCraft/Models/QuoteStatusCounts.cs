namespace QuoteCraft.Models;

public partial record QuoteStatusCounts(
    int All,
    int Draft,
    int Sent,
    int Viewed,
    int Accepted,
    int Declined,
    int Expired);
