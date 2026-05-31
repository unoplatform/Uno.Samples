import { serve } from "https://deno.land/std@0.177.0/http/server.ts";
import Stripe from "https://esm.sh/stripe@13.6.0?target=deno";
import { createClient } from "https://esm.sh/@supabase/supabase-js@2.38.0";

const stripe = new Stripe(Deno.env.get("STRIPE_SECRET_KEY")!, {
  apiVersion: "2023-10-16",
});

const supabaseUrl = Deno.env.get("SUPABASE_URL")!;
const supabaseServiceKey = Deno.env.get("SUPABASE_SERVICE_ROLE_KEY")!;

const PRICE_IDS: Record<string, Record<string, string>> = {
  pro: {
    monthly: Deno.env.get("STRIPE_PRO_MONTHLY_PRICE_ID") || "price_pro_monthly",
    annual: Deno.env.get("STRIPE_PRO_ANNUAL_PRICE_ID") || "price_pro_annual",
  },
  business: {
    monthly: Deno.env.get("STRIPE_BUSINESS_MONTHLY_PRICE_ID") || "price_business_monthly",
    annual: Deno.env.get("STRIPE_BUSINESS_ANNUAL_PRICE_ID") || "price_business_annual",
  },
};

serve(async (req: Request) => {
  try {
    // Verify JWT
    const authHeader = req.headers.get("Authorization");
    if (!authHeader) {
      return new Response(JSON.stringify({ error: "Missing authorization" }), {
        status: 401,
      });
    }

    const supabase = createClient(supabaseUrl, supabaseServiceKey);
    const token = authHeader.replace("Bearer ", "");
    const { data: { user }, error: authError } = await supabase.auth.getUser(token);

    if (authError || !user) {
      return new Response(JSON.stringify({ error: "Unauthorized" }), { status: 401 });
    }

    const body = await req.json();
    const tier = body.tier || "pro"; // "pro" or "business"
    const interval = body.interval || "monthly"; // "monthly" or "annual"

    const priceId = PRICE_IDS[tier]?.[interval];
    if (!priceId) {
      return new Response(JSON.stringify({ error: "Invalid tier or interval" }), {
        status: 400,
      });
    }

    // Check if user already has a Stripe customer
    const { data: subscription } = await supabase
      .from("subscriptions")
      .select("stripe_customer_id")
      .eq("user_id", user.id)
      .single();

    let customerId = subscription?.stripe_customer_id;

    if (!customerId) {
      const customer = await stripe.customers.create({
        email: user.email,
        metadata: { supabase_user_id: user.id },
      });
      customerId = customer.id;

      // Store the customer ID
      await supabase
        .from("subscriptions")
        .update({ stripe_customer_id: customerId })
        .eq("user_id", user.id);
    }

    // Create checkout session
    const session = await stripe.checkout.sessions.create({
      customer: customerId,
      mode: "subscription",
      line_items: [{ price: priceId, quantity: 1 }],
      success_url: `${body.success_url || "https://quotecraft.app/success"}?session_id={CHECKOUT_SESSION_ID}`,
      cancel_url: body.cancel_url || "https://quotecraft.app/cancel",
      metadata: {
        supabase_user_id: user.id,
        tier,
        interval,
      },
    });

    return new Response(JSON.stringify({ url: session.url }), {
      headers: { "Content-Type": "application/json" },
    });
  } catch (error) {
    console.error("Error creating checkout session:", error);
    return new Response(JSON.stringify({ error: error.message }), {
      status: 500,
    });
  }
});
