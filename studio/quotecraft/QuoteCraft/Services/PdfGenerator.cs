using SkiaSharp;

namespace QuoteCraft.Services;

/// <summary>
/// The quote's PDF, drawn with SkiaSharp's PDF backend (<see cref="SKDocument"/>), which every head already ships with
/// Uno's Skia renderer (docs/DECISIONS-production-readiness.md, D12). Text is set in the app's own DM Sans: the browser
/// has no system fonts.
/// </summary>
public sealed class PdfGenerator : IPdfGenerator
{
    private readonly IBusinessProfileRepository _profileRepo;
    private readonly IFeatureGateService _featureGate;
    private readonly IPhotoService _photoService;
    private Task<PdfFonts>? _fonts;

    public PdfGenerator(IBusinessProfileRepository profileRepo, IFeatureGateService featureGate, IPhotoService photoService)
    {
        _profileRepo = profileRepo;
        _featureGate = featureGate;
        _photoService = photoService;
    }

    public async Task<byte[]> GenerateQuotePdfAsync(QuoteEntity quote)
    {
        var profile = await _profileRepo.GetAsync();
        var photos = _photoService.GetPhotos(quote.Id);
        PdfFonts fonts;
        try
        {
            fonts = await (_fonts ??= PdfFonts.LoadAsync(OpenPackagedFontAsync));
        }
        catch
        {
            _fonts = null;
            throw;
        }

        var watermark = _featureGate.HasPdfWatermark;
        return await Task.Run(() => QuotePdf.Render(quote, profile, photos, watermark, DateTimeOffset.UtcNow, fonts));
    }

    private static async Task<Stream> OpenPackagedFontAsync(string fileName)
    {
        var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/Fonts/{fileName}"));
        return await file.OpenStreamForReadAsync();
    }
}

/// <summary>The fonts the PDF is set in: DM Sans from the app's Assets/Fonts.</summary>
public sealed class PdfFonts
{
    private PdfFonts(SKTypeface regular, SKTypeface bold)
    {
        Regular = regular;
        Bold = bold;
    }

    public SKTypeface Regular { get; }
    public SKTypeface Bold { get; }

    /// <param name="open">Opens a font file of Assets/Fonts by name (the app reads its package; tests read files).</param>
    public static async Task<PdfFonts> LoadAsync(Func<string, Task<Stream>> open)
    {
        return new PdfFonts(await LoadOneAsync("DMSans-Regular.ttf"), await LoadOneAsync("DMSans-Bold.ttf"));

        async Task<SKTypeface> LoadOneAsync(string fileName)
        {
            using var buffer = new MemoryStream();
            await using (var stream = await open(fileName))
            {
                await stream.CopyToAsync(buffer);
            }
            using var data = SKData.CreateCopy(buffer.ToArray());
            return SKTypeface.FromData(data) ?? throw new InvalidOperationException($"{fileName} could not be read as a font.");
        }
    }
}

/// <summary>
/// The layout, on US Letter with 40 pt margins: a header (logo, the business's contact lines, the quote's number and
/// dates) and a footer (the custom footer, the watermark, the date) on every page; the title and client; the line items
/// with alternating rows, whose header row repeats after a page break; subtotal, tax and total as the app shows them
/// (<see cref="Money"/>); the notes; then a photo appendix.
/// </summary>
public static class QuotePdf
{
    private const float PageWidth = 612;
    private const float PageHeight = 792;
    private const float Margin = 40;
    private const float ContentWidth = PageWidth - 2 * Margin;
    private const float CellPadding = 6;

    // Printed-page colours, which do not follow the app's theme (PrintColors says why).
    private static readonly SKColor Brand = PrintColors.Brand;
    private static readonly SKColor Ink = PrintColors.Ink;
    private static readonly SKColor Rule = PrintColors.Rule;
    private static readonly SKColor Stripe = PrintColors.Stripe;
    private static readonly SKColor Muted = PrintColors.Muted;
    private static readonly SKColor Faint = PrintColors.Faint;

    // Line-item columns, 4 : 1 : 1 : 1 of the content width.
    private static readonly float[] ColumnWidths = [ContentWidth * 4 / 7, ContentWidth / 7, ContentWidth / 7, ContentWidth / 7];

    public static byte[] Render(
        QuoteEntity quote,
        BusinessProfileEntity profile,
        IReadOnlyList<string> photoPaths,
        bool watermark,
        DateTimeOffset generatedAt,
        PdfFonts fonts)
    {
        var currencyCode = profile.CurrencyCode;
        string Amount(decimal value) => Money.Format(value, currencyCode);

        var metadata = SKDocumentPdfMetadata.Default;
        metadata.Title = $"Quote {quote.QuoteNumber}";
        metadata.Author = profile.BusinessName ?? string.Empty;
        metadata.Creator = "QuoteCraft";
        metadata.Creation = generatedAt.UtcDateTime;

        using var output = new MemoryStream();
        using (var document = SKDocument.CreatePdf(output, metadata))
        using (var logo = LoadImage(profile.LogoPath))
        {
            using var text = new TextStyles(fonts);
            var page = new PageFlow(document);

            // Quote pages
            var footerLines = new List<(string Text, SKFont Font, SKColor Color, bool Centered, float GapAbove)>();
            if (!string.IsNullOrEmpty(profile.CustomFooter))
            {
                foreach (var line in Wrap(profile.CustomFooter, text.Small, ContentWidth))
                {
                    footerLines.Add((line, text.Small, Muted, false, 0));
                }
            }
            if (watermark)
            {
                footerLines.Add(("Created with QuoteCraft - quotecraft.app", text.Small, Muted, true, footerLines.Count > 0 ? 4 : 0));
            }
            var generatedLine = $"Generated on {generatedAt:MMM dd, yyyy}";
            footerLines.Add((generatedLine, text.Tiny, Faint, true, footerLines.Count > 0 ? 2 : 0));

            page.Decorate = canvas =>
            {
                var top = DrawHeader(canvas, quote, profile, logo, generatedAt, text);
                var footerTop = PageHeight - Margin - footerLines.Sum(l => l.GapAbove + l.Font.Spacing);
                var y = footerTop;
                foreach (var (line, font, color, centered, gap) in footerLines)
                {
                    y += gap;
                    DrawLine(canvas, line, centered ? PageWidth / 2 : Margin, y, font, color, centered ? SKTextAlign.Center : SKTextAlign.Left);
                    y += font.Spacing;
                }
                return (top, footerTop - 12);
            };
            page.NewPage();

            // Title and client
            foreach (var line in Wrap(quote.Title, text.Title, ContentWidth))
            {
                page.Ensure(text.Title.Spacing);
                DrawLine(page.Canvas, line, Margin, page.Y, text.Title, Ink);
                page.Y += text.Title.Spacing;
            }
            page.Y += 8;
            if (!string.IsNullOrEmpty(quote.ClientName))
            {
                page.Ensure(text.Client.Spacing);
                DrawLine(page.Canvas, $"Client: {quote.ClientName}", Margin, page.Y, text.Client, Ink);
                page.Y += text.Client.Spacing + 12;
            }

            // Line items: the header row starts the table and every page the table continues on.
            string[] headers = ["Description", "Qty", "Unit Price", "Total"];
            var headerHeight = text.BodyBold.Spacing + 2 * CellPadding;
            page.Ensure(headerHeight + text.Body.Spacing + 2 * CellPadding);
            DrawTableHeader(page, headers, headerHeight, text);
            for (var i = 0; i < quote.LineItems.Count; i++)
            {
                var item = quote.LineItems[i];
                string[] cells = [item.Description, item.Quantity.ToString(), Amount(item.UnitPrice), Amount(item.LineTotal)];
                var wrapped = cells.Select((cell, c) => Wrap(cell, text.Body, ColumnWidths[c] - 2 * CellPadding)).ToArray();
                var rowHeight = wrapped.Max(lines => lines.Count) * text.Body.Spacing + 2 * CellPadding;
                if (page.Ensure(rowHeight))
                {
                    DrawTableHeader(page, headers, headerHeight, text);
                }

                using (var fill = Fill(i % 2 == 0 ? PrintColors.Paper : Stripe))
                {
                    page.Canvas.DrawRect(Margin, page.Y, ContentWidth, rowHeight, fill);
                }
                DrawRow(page.Canvas, wrapped, page.Y + CellPadding, text.Body, Ink);
                page.Y += rowHeight;
            }

            // Totals, kept together, right-aligned against a 120 pt amount column.
            var totals = new (string Label, string Value, SKFont LabelFont, SKFont ValueFont, SKColor Color, float GapAbove)[]
            {
                ("Subtotal:", $"{Amount(quote.Subtotal)} {currencyCode}", text.BodyBold, text.Body, Ink, 0),
                ($"Tax ({Money.FormatRate(quote.TaxRate)}%):", Amount(quote.TaxAmount), text.Body, text.Body, Ink, 0),
                ("Total:", $"{Amount(quote.Total)} {currencyCode}", text.TotalBold, text.TotalBold, Brand, 4),
            };
            page.Y += 12;
            page.Ensure(totals.Sum(t => t.GapAbove + t.LabelFont.Spacing));
            foreach (var (label, value, labelFont, valueFont, color, gap) in totals)
            {
                page.Y += gap;
                DrawLine(page.Canvas, label, PageWidth - Margin - 120 - 8, page.Y, labelFont, color, SKTextAlign.Right);
                DrawLine(page.Canvas, value, PageWidth - Margin, page.Y, valueFont, color, SKTextAlign.Right);
                page.Y += labelFont.Spacing;
            }

            // Notes, broken across pages line by line.
            if (!string.IsNullOrEmpty(quote.Notes))
            {
                page.Y += 20;
                page.Ensure(text.BodyBold.Spacing + 4 + text.Body.Spacing);
                DrawLine(page.Canvas, "Notes", Margin, page.Y, text.BodyBold, Ink);
                page.Y += text.BodyBold.Spacing + 4;
                foreach (var line in Wrap(quote.Notes, text.Body, ContentWidth))
                {
                    page.Ensure(text.Body.Spacing);
                    DrawLine(page.Canvas, line, Margin, page.Y, text.Body, Ink);
                    page.Y += text.Body.Spacing;
                }
            }

            // Photo appendix: two photos a row, each fitted to its cell
            var photos = photoPaths.Select(LoadImage).OfType<SKImage>().ToList();
            try
            {
                if (photos.Count > 0)
                {
                    page.Decorate = canvas =>
                    {
                        DrawLine(canvas, "Photo Attachments", Margin, Margin, text.Appendix, Brand);
                        var footerTop = PageHeight - Margin - text.Tiny.Spacing;
                        DrawLine(canvas, generatedLine, PageWidth / 2, footerTop, text.Tiny, Faint, SKTextAlign.Center);
                        return (Margin + text.Appendix.Spacing + 16, footerTop - 12);
                    };
                    page.NewPage();

                    var cellWidth = ContentWidth / 2;
                    var boxWidth = cellWidth - 2 * CellPadding;
                    var maxBoxHeight = (page.Bottom - page.Y) / 2 - 2 * CellPadding;
                    for (var i = 0; i < photos.Count; i += 2)
                    {
                        var row = photos.Skip(i).Take(2)
                            .Select(image => Fit(image, boxWidth, maxBoxHeight))
                            .ToList();
                        var rowHeight = row.Max(size => size.Height) + 2 * CellPadding;
                        page.Ensure(rowHeight);
                        for (var c = 0; c < row.Count; c++)
                        {
                            var rect = SKRect.Create(Margin + c * cellWidth + CellPadding, page.Y + CellPadding, row[c].Width, row[c].Height);
                            page.Canvas.DrawImage(photos[i + c], rect, new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));
                            using var border = new SKPaint { Color = Rule, Style = SKPaintStyle.Stroke, StrokeWidth = 1, IsAntialias = true };
                            page.Canvas.DrawRect(rect, border);
                        }
                        page.Y += rowHeight;
                    }
                }
            }
            finally
            {
                photos.ForEach(photo => photo.Dispose());
            }

            page.EndPage();
            document.Close();
        }

        return output.ToArray();
    }

    /// <summary>Draws the page header; returns where the content starts.</summary>
    private static float DrawHeader(SKCanvas canvas, QuoteEntity quote, BusinessProfileEntity profile, SKImage? logo,
        DateTimeOffset generatedAt, TextStyles text)
    {
        var left = Margin;
        if (logo is not null)
        {
            var size = Fit(logo, 60, 60);
            canvas.DrawImage(logo, SKRect.Create(Margin, Margin, size.Width, size.Height), new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));
            left += 60 + 12;
        }

        // The business and the quote share the width left of the logo, one half each.
        var columnWidth = (PageWidth - Margin - left) / 2;
        var y = Margin;
        foreach (var line in Wrap(profile.BusinessName ?? "My Business", text.Business, columnWidth))
        {
            DrawLine(canvas, line, left, y, text.Business, Brand);
            y += text.Business.Spacing;
        }
        foreach (var contact in new[] { profile.Phone, profile.Email, profile.Address })
        {
            if (string.IsNullOrEmpty(contact))
            {
                continue;
            }
            foreach (var line in Wrap(contact, text.Body, columnWidth))
            {
                DrawLine(canvas, line, left, y, text.Body, Ink);
                y += text.Body.Spacing;
            }
        }

        var right = PageWidth - Margin;
        var ry = Margin;
        DrawLine(canvas, "QUOTE", right, ry, text.Heading, Brand, SKTextAlign.Right);
        ry += text.Heading.Spacing;
        var details = new List<string> { $"#{quote.QuoteNumber}", $"Date: {generatedAt:MMM dd, yyyy}" };
        if (quote.ValidUntil.HasValue)
        {
            details.Add($"Valid Until: {quote.ValidUntil.Value:MMM dd, yyyy}");
        }
        foreach (var line in details)
        {
            DrawLine(canvas, line, right, ry, text.Body, Ink, SKTextAlign.Right);
            ry += text.Body.Spacing;
        }

        var bottom = Math.Max(Math.Max(y, ry), logo is null ? Margin : Margin + 60) + 8;
        using (var rule = new SKPaint { Color = Rule, StrokeWidth = 1, IsAntialias = true })
        {
            canvas.DrawLine(Margin, bottom, PageWidth - Margin, bottom, rule);
        }
        return bottom + 1 + 8;
    }

    private static void DrawTableHeader(PageFlow page, string[] headers, float height, TextStyles text)
    {
        using (var fill = Fill(Brand))
        {
            page.Canvas.DrawRect(Margin, page.Y, ContentWidth, height, fill);
        }
        DrawRow(page.Canvas, headers.Select(h => new List<string> { h }).ToArray(), page.Y + CellPadding, text.BodyBold, PrintColors.OnBrand);
        page.Y += height;
    }

    /// <summary>One table row: the first column left-aligned, the numbers right-aligned.</summary>
    private static void DrawRow(SKCanvas canvas, IReadOnlyList<List<string>> cells, float top, SKFont font, SKColor color)
    {
        var x = Margin;
        for (var c = 0; c < cells.Count; c++)
        {
            var y = top;
            foreach (var line in cells[c])
            {
                if (c == 0)
                {
                    DrawLine(canvas, line, x + CellPadding, y, font, color);
                }
                else
                {
                    DrawLine(canvas, line, x + ColumnWidths[c] - CellPadding, y, font, color, SKTextAlign.Right);
                }
                y += font.Spacing;
            }
            x += ColumnWidths[c];
        }
    }

    /// <summary>One line of text whose box starts at <paramref name="top"/>.</summary>
    private static void DrawLine(SKCanvas canvas, string line, float x, float top, SKFont font, SKColor color,
        SKTextAlign align = SKTextAlign.Left)
    {
        using var paint = new SKPaint { Color = color, IsAntialias = true };
        canvas.DrawText(Printable(line, font), x, top - font.Metrics.Ascent, align, font, paint);
    }

    /// <summary>
    /// Spaces DM Sans has no glyph for print as plain spaces: amounts in some currencies group digits with a narrow
    /// no-break space ("1 250,00 €").
    /// </summary>
    private static string Printable(string line, SKFont font)
    {
        if (font.ContainsGlyphs(line))
        {
            return line;
        }
        return string.Concat(line.Select(c => char.IsWhiteSpace(c) && !font.ContainsGlyph(c) ? ' ' : c));
    }

    /// <summary>Breaks text into lines no wider than <paramref name="width"/>, at spaces where it can.</summary>
    private static List<string> Wrap(string text, SKFont font, float width)
    {
        var lines = new List<string>();
        foreach (var paragraph in text.Replace("\r\n", "\n").Split('\n'))
        {
            var line = string.Empty;
            foreach (var word in paragraph.Split(' '))
            {
                var candidate = line.Length == 0 ? word : $"{line} {word}";
                if (font.MeasureText(candidate) <= width)
                {
                    line = candidate;
                    continue;
                }

                if (line.Length > 0)
                {
                    lines.Add(line);
                }

                // A word wider than the line is broken where it has to be.
                var rest = word;
                while (rest.Length > 1 && font.MeasureText(rest) > width)
                {
                    var fit = Math.Max(1, font.BreakText(rest, width));
                    lines.Add(rest[..fit]);
                    rest = rest[fit..];
                }
                line = rest;
            }
            lines.Add(line);
        }
        return lines;
    }

    private static SKSize Fit(SKImage image, float maxWidth, float maxHeight)
    {
        var scale = Math.Min(maxWidth / image.Width, maxHeight / image.Height);
        return new SKSize(image.Width * scale, image.Height * scale);
    }

    private static SKImage? LoadImage(string? path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            return null;
        }
        try
        {
            return SKImage.FromEncodedData(File.ReadAllBytes(path));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static SKPaint Fill(SKColor color) => new() { Color = color, Style = SKPaintStyle.Fill };

    /// <summary>The type sizes of the layout.</summary>
    private sealed class TextStyles(PdfFonts fonts) : IDisposable
    {
        public SKFont Body { get; } = new(fonts.Regular, 10);
        public SKFont BodyBold { get; } = new(fonts.Bold, 10);
        public SKFont Client { get; } = new(fonts.Regular, 11);
        public SKFont Title { get; } = new(fonts.Bold, 14);
        public SKFont TotalBold { get; } = new(fonts.Bold, 14);
        public SKFont Appendix { get; } = new(fonts.Bold, 16);
        public SKFont Business { get; } = new(fonts.Bold, 18);
        public SKFont Heading { get; } = new(fonts.Bold, 24);
        public SKFont Small { get; } = new(fonts.Regular, 8);
        public SKFont Tiny { get; } = new(fonts.Regular, 7);

        public void Dispose()
        {
            foreach (var font in new[] { Body, BodyBold, Client, Title, TotalBold, Appendix, Business, Heading, Small, Tiny })
            {
                font.Dispose();
            }
        }
    }

    /// <summary>
    /// The page being drawn and the pen's position on it. <see cref="Ensure"/> starts a new page when the next block
    /// does not fit; <see cref="Decorate"/> draws a new page's header and footer and returns its content area.
    /// </summary>
    private sealed class PageFlow(SKDocument document)
    {
        private bool _open;
        private float _top;
        private SKCanvas? _canvas;

        public Func<SKCanvas, (float Top, float Bottom)> Decorate { get; set; } = _ => (Margin, PageHeight - Margin);
        /// <summary>The open page's canvas; <see cref="NewPage"/> opens the first.</summary>
        public SKCanvas Canvas => _canvas ?? throw new InvalidOperationException("No page is open.");
        public float Y { get; set; }
        public float Bottom { get; private set; }

        public void NewPage()
        {
            EndPage();
            _canvas = document.BeginPage(PageWidth, PageHeight);
            _open = true;
            var (top, bottom) = Decorate(Canvas);
            Y = _top = top;
            Bottom = bottom;
        }

        /// <summary>True when a new page had to start for a block this tall (never for an empty page).</summary>
        public bool Ensure(float height)
        {
            if (Y + height <= Bottom || Y <= _top)
            {
                return false;
            }
            NewPage();
            return true;
        }

        public void EndPage()
        {
            if (_open)
            {
                document.EndPage();
                _open = false;
            }
        }
    }
}
