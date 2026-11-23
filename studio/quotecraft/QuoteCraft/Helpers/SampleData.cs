#if DEBUG
namespace QuoteCraft.Helpers;

/// <summary>
/// Debug-only sample data for the screenshot matrix (<c>--qc-seed=sample</c>): a business profile, five clients and
/// seven quotes, one per status plus a long title, with line items. Runs once, on an empty database.
/// </summary>
public static class SampleData
{
    /// <summary>
    /// Runs before the host starts, on its own connection, so the first pages already load the sample data.
    /// </summary>
    public static async Task SeedAsync()
    {
        var db = new AppDatabase();
        var quotes = new QuoteRepository(db, new Services.PhotoService());
        if ((await quotes.GetAllAsync()).Count > 0)
        {
            VerificationArgs.Log("seed=skipped (database not empty)");
            return;
        }

        var profiles = new BusinessProfileRepository(db);
        var profile = await profiles.GetAsync();
        profile.BusinessName = "Northside Plumbing & Heating";
        profile.Phone = "(555) 010-2030";
        profile.Email = "office@northside.example";
        profile.Address = "48 Foundry Lane, Riverton";
        profile.DefaultTaxRate = 13m;
        profile.QuoteValidDays = 30;
        await profiles.SaveAsync(profile);

        var clients = new ClientRepository(db);
        ClientEntity[] people =
        [
            new() { Name = "Bob Johnson", Email = "bob@johnson.example", Phone = "(555) 234-5678", Address = "12 Elm Street\nRiverton 40210" },
            new() { Name = "Sarah Williams", Email = "sarah.w@example.com", Phone = "(555) 345-6789", Address = "7 Harbour Road, Lakeside" },
            new() { Name = "Mike Chen", Email = "mike@chen.example", Phone = "(555) 456-7890", Address = "301 Mill Avenue, Riverton" },
            new() { Name = "Lisa Martinez", Email = "lisa@martinez.example", Phone = "(555) 567-8901", Address = "88 Orchard Close, Westfield" },
            new() { Name = "Greenfield Property Management Cooperative", Email = "maintenance@greenfield.example", Phone = "(555) 678-9012", Address = "1200 Commerce Boulevard, Suite 400, Riverton" },
        ];
        foreach (var client in people)
        {
            await clients.SaveAsync(client);
        }

        var now = DateTimeOffset.UtcNow;
        (string Title, int Client, QuoteStatus Status, int AgeDays, int ValidDays, (string, decimal, int)[] Items)[] seeds =
        [
            ("Kitchen Renovation", 0, QuoteStatus.Accepted, 40, 30, [("Sink Installation", 350m, 1), ("Faucet Replacement", 150m, 2), ("Copper Pipe (per ft)", 12.5m, 40), ("Standard Labor (per hr)", 95m, 8)]),
            ("Drain Cleaning Service", 1, QuoteStatus.Sent, 3, 27, [("Drain Snake Service", 185m, 1), ("Camera Inspection", 225m, 1)]),
            ("Office Plumbing Fit-out", 0, QuoteStatus.Sent, 6, 24, [("Toilet Installation", 420m, 3), ("Standard Labor (per hr)", 95m, 12)]),
            ("Emergency Pipe Repair", 2, QuoteStatus.Draft, 0, 30, [("Emergency Call-out", 250m, 1), ("PEX Pipe (per ft)", 4.75m, 25)]),
            ("Water Heater Install", 3, QuoteStatus.Declined, 20, 10, [("50 Gal Water Heater", 1150m, 1), ("Haul-away & Disposal", 75m, 1)]),
            ("Annual Boiler Service and Full Heating System Safety Inspection for All Units", 4, QuoteStatus.Expired, 75, 30, [("Boiler Service", 320m, 4), ("Safety Inspection", 140m, 4)]),
            ("Bathroom Remodel", 1, QuoteStatus.Sent, 12, 18, [("Vanity Installation", 480m, 1), ("Tile Work (per sq ft)", 14m, 60)]),
        ];

        var number = 1;
        foreach (var (title, clientIndex, status, ageDays, validDays, items) in seeds)
        {
            var client = people[clientIndex];
            var created = now.AddDays(-ageDays);
            var quote = new QuoteEntity
            {
                Title = title,
                ClientId = client.Id,
                ClientName = client.Name,
                Status = status,
                TaxRate = profile.DefaultTaxRate,
                QuoteNumber = $"QC-{now.Year}-{number++:0000}",
                CreatedAt = created,
                SentAt = status is QuoteStatus.Draft ? null : created.AddHours(2),
                ValidUntil = created.AddDays(ageDays + validDays),
                Notes = status is QuoteStatus.Draft ? null : "Price includes materials and clean-up. Access to the work area is needed on the day.",
            };
            if (status is QuoteStatus.Expired)
            {
                quote.ValidUntil = created.AddDays(30);
            }
            await quotes.SaveAsync(quote);

            foreach (var (description, price, quantity) in items)
            {
                await quotes.SaveLineItemAsync(new LineItemEntity
                {
                    QuoteId = quote.Id,
                    Description = description,
                    UnitPrice = price,
                    Quantity = quantity,
                });
            }
        }

        await new Services.OnboardingService().MarkCompleteAsync();
        VerificationArgs.Log($"seed=sample clients={people.Length} quotes={seeds.Length}");
    }
}
#endif
