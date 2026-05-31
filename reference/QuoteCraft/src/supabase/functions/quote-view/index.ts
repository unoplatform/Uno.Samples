import { serve } from "https://deno.land/std@0.177.0/http/server.ts";
import { createClient } from "https://esm.sh/@supabase/supabase-js@2.38.0";

const supabaseUrl = Deno.env.get("SUPABASE_URL")!;
const supabaseServiceKey = Deno.env.get("SUPABASE_SERVICE_ROLE_KEY")!;

serve(async (req: Request) => {
  const url = new URL(req.url);
  const token = url.searchParams.get("token");

  if (!token) {
    return new Response("Missing token", { status: 400 });
  }

  const supabase = createClient(supabaseUrl, supabaseServiceKey);

  // Fetch quote by share token
  const { data: quote, error: quoteError } = await supabase
    .from("quotes")
    .select("*")
    .eq("share_token", token)
    .eq("is_deleted", false)
    .single();

  if (quoteError || !quote) {
    return new Response(renderErrorPage("Quote not found or has expired."), {
      headers: { "Content-Type": "text/html" },
      status: 404,
    });
  }

  // Handle POST actions (Accept / Decline)
  if (req.method === "POST") {
    const formData = await req.formData();
    const action = formData.get("action");

    if (action === "accept" || action === "decline") {
      const newStatus = action === "accept" ? "Accepted" : "Declined";

      await supabase
        .from("quotes")
        .update({ status: newStatus, updated_at: new Date().toISOString() })
        .eq("id", quote.id);

      // Record status history
      await supabase.from("status_history").insert({
        id: crypto.randomUUID(),
        quote_id: quote.id,
        status: newStatus,
        changed_at: new Date().toISOString(),
        changed_by: "client",
      });

      // Create notification for the contractor
      await supabase.from("notifications").insert({
        id: crypto.randomUUID(),
        user_id: quote.user_id,
        type: action === "accept" ? "QuoteAccepted" : "QuoteDeclined",
        title: `Quote ${action === "accept" ? "Accepted" : "Declined"}`,
        message: `${quote.client_name || "A client"} has ${action}ed quote #${quote.quote_number}`,
        quote_id: quote.id,
        is_read: false,
        created_at: new Date().toISOString(),
      });

      // Redirect back to the same page
      return new Response(null, {
        status: 303,
        headers: { Location: `${url.pathname}?token=${token}` },
      });
    }
  }

  // Mark as Viewed if currently Sent
  if (quote.status === "Sent") {
    await supabase
      .from("quotes")
      .update({ status: "Viewed", updated_at: new Date().toISOString() })
      .eq("id", quote.id);

    await supabase.from("status_history").insert({
      id: crypto.randomUUID(),
      quote_id: quote.id,
      status: "Viewed",
      changed_at: new Date().toISOString(),
      changed_by: "client",
    });

    await supabase.from("notifications").insert({
      id: crypto.randomUUID(),
      user_id: quote.user_id,
      type: "QuoteViewed",
      title: "Quote Viewed",
      message: `${quote.client_name || "A client"} viewed quote #${quote.quote_number}`,
      quote_id: quote.id,
      is_read: false,
      created_at: new Date().toISOString(),
    });

    quote.status = "Viewed";
  }

  // Fetch line items
  const { data: lineItems } = await supabase
    .from("line_items")
    .select("*")
    .eq("quote_id", quote.id)
    .order("sort_order", { ascending: true });

  // Fetch business profile
  const { data: profile } = await supabase
    .from("business_profile")
    .select("*")
    .eq("user_id", quote.user_id)
    .single();

  const html = renderQuotePage(quote, lineItems || [], profile);

  return new Response(html, {
    headers: { "Content-Type": "text/html" },
  });
});

function renderQuotePage(quote: any, lineItems: any[], profile: any): string {
  const subtotal = lineItems.reduce(
    (sum: number, li: any) => sum + li.unit_price * li.quantity,
    0
  );
  const taxAmount = subtotal * (quote.tax_rate / 100);
  const total = subtotal + taxAmount;
  const currency = profile?.currency_code || "USD";

  const formatMoney = (amount: number) =>
    new Intl.NumberFormat("en-US", { style: "currency", currency }).format(amount);

  const isActionable = quote.status === "Viewed" || quote.status === "Sent";
  const isResolved = quote.status === "Accepted" || quote.status === "Declined";

  const statusColors: Record<string, string> = {
    Draft: "#6B7280",
    Sent: "#3B82F6",
    Viewed: "#8B5CF6",
    Accepted: "#10B981",
    Declined: "#EF4444",
    Expired: "#9CA3AF",
  };

  return `<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Quote #${quote.quote_number} — ${profile?.business_name || "QuoteCraft"}</title>
  <style>
    * { margin: 0; padding: 0; box-sizing: border-box; }
    body { font-family: 'DM Sans', -apple-system, BlinkMacSystemFont, sans-serif; background: #f5f5f5; color: #1a1a1a; }
    .container { max-width: 800px; margin: 40px auto; background: white; border-radius: 12px; box-shadow: 0 2px 12px rgba(0,0,0,0.08); overflow: hidden; }
    .header { background: #2B4C7E; color: white; padding: 32px; }
    .header h1 { font-size: 24px; margin-bottom: 4px; }
    .header p { opacity: 0.85; font-size: 14px; }
    .accent-bar { height: 3px; background: #F59E0B; }
    .content { padding: 32px; }
    .meta { display: flex; justify-content: space-between; margin-bottom: 24px; flex-wrap: wrap; gap: 16px; }
    .meta-group { }
    .meta-label { font-size: 12px; font-weight: 600; color: #6B7280; text-transform: uppercase; letter-spacing: 0.5px; margin-bottom: 4px; }
    .meta-value { font-size: 14px; }
    .status-badge { display: inline-block; padding: 4px 12px; border-radius: 20px; font-size: 12px; font-weight: 600; color: white; background: ${statusColors[quote.status] || "#6B7280"}; }
    table { width: 100%; border-collapse: collapse; margin: 24px 0; }
    th { background: #f5f5f5; padding: 12px 16px; text-align: left; font-size: 12px; font-weight: 600; color: #6B7280; text-transform: uppercase; }
    td { padding: 12px 16px; border-bottom: 1px solid #f0f0f0; font-size: 14px; }
    .text-right { text-align: right; }
    .totals { margin-left: auto; width: 280px; }
    .totals-row { display: flex; justify-content: space-between; padding: 8px 0; font-size: 14px; }
    .totals-row.total { border-top: 2px solid #2B4C7E; font-weight: 700; font-size: 18px; color: #2B4C7E; padding-top: 12px; margin-top: 4px; }
    .actions { display: flex; gap: 12px; margin-top: 32px; justify-content: center; }
    .btn { display: inline-block; padding: 12px 32px; border-radius: 8px; font-size: 16px; font-weight: 600; border: none; cursor: pointer; text-decoration: none; }
    .btn-accept { background: #10B981; color: white; }
    .btn-accept:hover { background: #059669; }
    .btn-decline { background: #EF4444; color: white; }
    .btn-decline:hover { background: #DC2626; }
    .resolved { text-align: center; padding: 24px; font-size: 18px; font-weight: 600; }
    .footer { padding: 24px 32px; border-top: 1px solid #f0f0f0; text-align: center; font-size: 12px; color: #9CA3AF; }
    .notes { background: #FEFCE8; border-left: 3px solid #F59E0B; padding: 16px; border-radius: 4px; margin: 16px 0; font-size: 14px; }
    @media (max-width: 600px) { .container { margin: 0; border-radius: 0; } .content { padding: 20px; } .meta { flex-direction: column; } .totals { width: 100%; } }
  </style>
</head>
<body>
  <div class="container">
    <div class="header">
      <h1>${profile?.business_name || "QuoteCraft"}</h1>
      ${profile?.phone ? `<p>${profile.phone}</p>` : ""}
      ${profile?.email ? `<p>${profile.email}</p>` : ""}
    </div>
    <div class="accent-bar"></div>
    <div class="content">
      <div class="meta">
        <div class="meta-group">
          <div class="meta-label">Quote</div>
          <div class="meta-value">#${quote.quote_number}</div>
        </div>
        <div class="meta-group">
          <div class="meta-label">Date</div>
          <div class="meta-value">${new Date(quote.created_at).toLocaleDateString("en-US", { year: "numeric", month: "short", day: "numeric" })}</div>
        </div>
        ${quote.valid_until ? `<div class="meta-group"><div class="meta-label">Valid Until</div><div class="meta-value">${new Date(quote.valid_until).toLocaleDateString("en-US", { year: "numeric", month: "short", day: "numeric" })}</div></div>` : ""}
        <div class="meta-group">
          <div class="meta-label">Status</div>
          <div class="meta-value"><span class="status-badge">${quote.status}</span></div>
        </div>
      </div>

      ${quote.client_name ? `<div class="meta-group" style="margin-bottom: 16px;"><div class="meta-label">Prepared For</div><div class="meta-value">${quote.client_name}</div></div>` : ""}

      <h2 style="font-size: 18px; margin-bottom: 8px;">${quote.title}</h2>

      <table>
        <thead><tr><th>Description</th><th class="text-right">Qty</th><th class="text-right">Price</th><th class="text-right">Total</th></tr></thead>
        <tbody>
          ${lineItems.map((li: any) => `<tr><td>${li.description}</td><td class="text-right">${li.quantity}</td><td class="text-right">${formatMoney(li.unit_price)}</td><td class="text-right">${formatMoney(li.unit_price * li.quantity)}</td></tr>`).join("")}
        </tbody>
      </table>

      <div class="totals">
        <div class="totals-row"><span>Subtotal</span><span>${formatMoney(subtotal)}</span></div>
        ${quote.tax_rate > 0 ? `<div class="totals-row"><span>Tax (${quote.tax_rate}%)</span><span>${formatMoney(taxAmount)}</span></div>` : ""}
        <div class="totals-row total"><span>Total</span><span>${formatMoney(total)}</span></div>
      </div>

      ${quote.notes ? `<div class="notes"><strong>Notes:</strong> ${quote.notes}</div>` : ""}

      ${isActionable ? `
      <form method="POST" class="actions">
        <button type="submit" name="action" value="accept" class="btn btn-accept">Accept Quote</button>
        <button type="submit" name="action" value="decline" class="btn btn-decline">Decline</button>
      </form>
      ` : ""}

      ${isResolved ? `<div class="resolved">${quote.status === "Accepted" ? "This quote has been accepted." : "This quote has been declined."}</div>` : ""}
    </div>
    <div class="footer">
      ${profile?.custom_footer ? `<p style="margin-bottom: 8px;">${profile.custom_footer}</p>` : ""}
      <p>All amounts in ${currency} &middot; Powered by QuoteCraft</p>
    </div>
  </div>
</body>
</html>`;
}

function renderErrorPage(message: string): string {
  return `<!DOCTYPE html>
<html lang="en">
<head><meta charset="UTF-8"><meta name="viewport" content="width=device-width, initial-scale=1.0"><title>Quote Not Found</title>
<style>body{font-family:-apple-system,sans-serif;display:flex;align-items:center;justify-content:center;height:100vh;background:#f5f5f5;color:#6B7280;text-align:center;}.msg{padding:48px;background:white;border-radius:12px;box-shadow:0 2px 12px rgba(0,0,0,0.08);}</style>
</head><body><div class="msg"><h1 style="font-size:24px;color:#1a1a1a;margin-bottom:8px;">Quote Not Found</h1><p>${message}</p></div></body></html>`;
}
