-- QuoteCraft: Row Level Security policies
-- Each user can only access their own data

ALTER TABLE public.user_profiles ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.subscriptions ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.clients ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.quotes ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.line_items ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.business_profile ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.catalog_items ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.status_history ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.notifications ENABLE ROW LEVEL SECURITY;

-- ─── User Profiles ─────────────────────────────────────────────────────────
CREATE POLICY "Users can view own profile"
    ON public.user_profiles FOR SELECT
    USING (auth.uid() = id);

CREATE POLICY "Users can update own profile"
    ON public.user_profiles FOR UPDATE
    USING (auth.uid() = id);

CREATE POLICY "Users can insert own profile"
    ON public.user_profiles FOR INSERT
    WITH CHECK (auth.uid() = id);

-- ─── Subscriptions ─────────────────────────────────────────────────────────
CREATE POLICY "Users can view own subscription"
    ON public.subscriptions FOR SELECT
    USING (auth.uid() = user_id);

-- Subscriptions are managed by the stripe-webhook edge function (service role)
-- No INSERT/UPDATE/DELETE policies for regular users

-- ─── Clients ───────────────────────────────────────────────────────────────
CREATE POLICY "Users can CRUD own clients"
    ON public.clients FOR ALL
    USING (auth.uid() = user_id)
    WITH CHECK (auth.uid() = user_id);

-- ─── Quotes ────────────────────────────────────────────────────────────────
CREATE POLICY "Users can CRUD own quotes"
    ON public.quotes FOR ALL
    USING (auth.uid() = user_id)
    WITH CHECK (auth.uid() = user_id);

-- Public access for client-facing quote view (by share_token)
CREATE POLICY "Anyone can view shared quotes"
    ON public.quotes FOR SELECT
    USING (share_token IS NOT NULL);

-- ─── Line Items ────────────────────────────────────────────────────────────
CREATE POLICY "Users can CRUD own line items"
    ON public.line_items FOR ALL
    USING (auth.uid() = user_id)
    WITH CHECK (auth.uid() = user_id);

-- ─── Business Profile ──────────────────────────────────────────────────────
CREATE POLICY "Users can CRUD own profile"
    ON public.business_profile FOR ALL
    USING (auth.uid() = user_id)
    WITH CHECK (auth.uid() = user_id);

-- ─── Catalog Items ─────────────────────────────────────────────────────────
CREATE POLICY "Users can CRUD own catalog items"
    ON public.catalog_items FOR ALL
    USING (auth.uid() = user_id)
    WITH CHECK (auth.uid() = user_id);

-- ─── Status History ────────────────────────────────────────────────────────
CREATE POLICY "Users can view status history for own quotes"
    ON public.status_history FOR SELECT
    USING (
        EXISTS (
            SELECT 1 FROM public.quotes
            WHERE quotes.id = status_history.quote_id
            AND quotes.user_id = auth.uid()
        )
    );

CREATE POLICY "Users can insert status history for own quotes"
    ON public.status_history FOR INSERT
    WITH CHECK (
        EXISTS (
            SELECT 1 FROM public.quotes
            WHERE quotes.id = status_history.quote_id
            AND quotes.user_id = auth.uid()
        )
    );

-- ─── Notifications ─────────────────────────────────────────────────────────
CREATE POLICY "Users can view own notifications"
    ON public.notifications FOR SELECT
    USING (auth.uid() = user_id);

CREATE POLICY "Users can update own notifications"
    ON public.notifications FOR UPDATE
    USING (auth.uid() = user_id);

-- Notifications are created by edge functions (service role)

-- ─── Storage Policies ──────────────────────────────────────────────────────
CREATE POLICY "Users can upload own logos"
    ON storage.objects FOR INSERT
    WITH CHECK (bucket_id = 'logos' AND auth.uid()::text = (storage.foldername(name))[1]);

CREATE POLICY "Anyone can view logos"
    ON storage.objects FOR SELECT
    USING (bucket_id = 'logos');

CREATE POLICY "Users can upload own photos"
    ON storage.objects FOR INSERT
    WITH CHECK (bucket_id = 'photos' AND auth.uid()::text = (storage.foldername(name))[1]);

CREATE POLICY "Users can view own photos"
    ON storage.objects FOR SELECT
    USING (bucket_id = 'photos' AND auth.uid()::text = (storage.foldername(name))[1]);

CREATE POLICY "Users can delete own photos"
    ON storage.objects FOR DELETE
    USING (bucket_id = 'photos' AND auth.uid()::text = (storage.foldername(name))[1]);

-- ─── Auto-create profile on signup ─────────────────────────────────────────
CREATE OR REPLACE FUNCTION public.handle_new_user()
RETURNS trigger AS $$
BEGIN
    INSERT INTO public.user_profiles (id, email, display_name)
    VALUES (new.id, new.email, COALESCE(new.raw_user_meta_data->>'display_name', split_part(new.email, '@', 1)));

    INSERT INTO public.subscriptions (user_id, tier, status)
    VALUES (new.id, 'free', 'active');

    RETURN new;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

CREATE OR REPLACE TRIGGER on_auth_user_created
    AFTER INSERT ON auth.users
    FOR EACH ROW EXECUTE FUNCTION public.handle_new_user();
