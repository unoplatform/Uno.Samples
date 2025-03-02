namespace TelerikApp.Business.Models;

public record FinancialDataItem(string Date, double Open, double High, double Low, double Close)
{
    public DateTime DateCategory
    {
        get
        {
            return DateTime.ParseExact(this.Date, "dd-MM-yyyy", null);
        }
    }
}