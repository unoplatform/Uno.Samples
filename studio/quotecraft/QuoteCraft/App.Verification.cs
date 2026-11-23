namespace QuoteCraft;

/// <summary>
/// Debug-only verification hooks for the screenshot matrix and the runtime checks (docs/evidence), read from the
/// command line by <c>VerificationArgs</c>. App.OnLaunched calls the two entry points; in Release they
/// do nothing and none of the hooks is compiled.
/// <list type="bullet">
/// <item><c>--qc-seed=sample</c> puts sample data in before the first page loads; <c>--qc-fault=load</c> makes the
/// database fail for the first 20 s, for the error states and Retry (<see cref="PrepareVerificationDataAsync"/>).</item>
/// <item><c>--qc-focus=log</c> prints each element that takes keyboard focus; <c>--qc-theme=light|dark</c> sets the
/// theme through the theme service, as the app's own selector does; <c>--qc-route=&lt;route&gt;[:detail]</c> opens
/// Dashboard (the quotes list), Clients, Catalog, Settings, Onboarding or QuoteEditor (the seeded quote with the most
/// line items, opened from the dashboard like a tap does); <c>:detail</c> also selects the first quote, client or
/// category, <c>Onboarding:step3</c> opens its last step and <c>Onboarding:complete</c> runs it through Create First
/// Quote (<see cref="ApplyVerificationViewAsync"/>). Each step prints a <c>VERIFY</c> line to stderr.</item>
/// </list>
/// </summary>
public partial class App
{
#if DEBUG
    /// <summary>Before the first page: sample data (<c>--qc-seed=sample</c>) and the load fault (<c>--qc-fault=load</c>).</summary>
    private static async Task PrepareVerificationDataAsync()
    {
        if (VerificationArgs.Get("seed") == "sample")
        {
            try
            {
                await SampleData.SeedAsync();
            }
            catch (Exception ex)
            {
                VerificationArgs.Log($"seed failed: {ex}");
            }
        }

        if (VerificationArgs.Get("fault") == "load")
        {
            // Error states: the database fails for 20 s from the first page, then recovers for Retry.
            AppDatabase.FailUntil = DateTimeOffset.UtcNow.AddSeconds(20);
            VerificationArgs.Log($"fault=load until={AppDatabase.FailUntil:HH:mm:ss}");
        }
    }

    /// <summary>Once the first page shows: the focus log, the theme and the route hooks.</summary>
    private async Task ApplyVerificationViewAsync()
    {
        try
        {
            if (Host is null || MainWindow?.Content is not FrameworkElement root)
            {
                return;
            }

            if (VerificationArgs.Get("focus") == "log")
            {
                // Keyboard checks: each element that takes focus, with its accessible name, so a Tab walk is readable.
                Microsoft.UI.Xaml.Input.FocusManager.GotFocus += (_, e) =>
                {
                    var element = e.NewFocusedElement as DependencyObject;
                    var name = element is null ? string.Empty : Microsoft.UI.Xaml.Automation.AutomationProperties.GetName(element);
                    if (string.IsNullOrEmpty(name) && element is ContentControl { Content: string text })
                    {
                        name = text;
                    }
                    VerificationArgs.Log($"focus {element?.GetType().Name} \"{name}\"");
                };
            }

            if (VerificationArgs.Get("theme") is { } theme)
            {
                var dark = theme.Equals("dark", StringComparison.OrdinalIgnoreCase);
                // The window's theme service, as the shell's theme menu uses it.
                var scope = (FrameworkElement?)await WaitForAsync<MainPage>(root) ?? root;
                var service = scope.FindServiceProvider()?.GetService<Uno.Extensions.Toolkit.IThemeService>();
                if (service is not null)
                {
                    await service.SetThemeAsync(dark ? Uno.Extensions.Toolkit.AppTheme.Dark : Uno.Extensions.Toolkit.AppTheme.Light);
                    Host.Services.GetRequiredService<Services.ThemeChoiceNotifier>().Raise();
                }
                VerificationArgs.Log($"theme={(dark ? "Dark" : "Light")} service={service is not null} actual={root.ActualTheme}");
            }

            if (VerificationArgs.Get("route") is not { } spec)
            {
                return;
            }

            var parts = spec.Split(':', 2);
            var route = parts[0];
            var detail = parts.Length == 2 && parts[1] == "detail";
            if (route == "Onboarding")
            {
                // The shell's navigator, as ShowOnboardingOnFirstRunAsync uses it.
                var shellNavigator = (MainWindow.Content as IContentControlProvider)?.ContentControl.Navigator();
                var response = shellNavigator is null ? null
                    : await shellNavigator.NavigateRouteAsync(this, "Onboarding", qualifier: Qualifiers.ClearBackStack);
                VerificationArgs.Log($"route={spec} success={response?.Success}");
                // Onboarding:complete walks the steps and presses Create First Quote, as a first-run user would.
                // Onboarding:step3 shows the quote settings step.
                if (parts.Length == 2 && parts[1] is "complete" or "step3"
                    && await WaitForAsync<OnboardingPage>(root) is { } onboarding
                    && onboarding.DataContext is OnboardingViewModel { Model: { } model })
                {
                    await Task.Delay(1000);
                    // One step per press, as a user would wait: a second press racing the first read step 1 again.
                    for (var press = 0; press < 10 && await model.CurrentStep < 3; press++)
                    {
                        await model.NextStep(CancellationToken.None);
                        await Task.Delay(300);
                    }
                    if (parts[1] == "complete")
                    {
                        await model.Complete(CancellationToken.None);
                        VerificationArgs.Log($"route={spec} completed editor={await WaitForAsync<QuoteEditorPage>(root) is not null}");
                    }
                }
                return;
            }

            var main = await WaitForAsync<MainPage>(root);
            if (main is null)
            {
                VerificationArgs.Log($"route={spec} failed: no main page");
                return;
            }

            if (route is "Clients" or "Catalog" or "Settings")
            {
                // Select the tab, as a tap does: navigating the region from here left the window unpainted after a
                // resize under Xvfb (the tab route repaints normally).
                // xaml-lint: allow responsive - not a breakpoint: picks the tab bar that is laid out (Debug-only hook)
                var tab = Descendants<Uno.Toolkit.UI.TabBarItem>(main)
                    .FirstOrDefault(t => Uno.Extensions.Navigation.UI.Region.GetName(t) == route && t.ActualWidth > 0);
                if (tab is not null)
                {
                    tab.IsSelected = true;
                }
                VerificationArgs.Log($"route={spec} tab={tab is not null}");
            }

            if (route == "QuoteEditor")
            {
                var dashboard = await WaitForAsync<DashboardPage>(root);
                var quotes = await Host.Services.GetRequiredService<IQuoteRepository>().GetAllAsync();
                var quote = quotes.OrderByDescending(q => q.LineItems.Count).FirstOrDefault();
                if (dashboard?.Navigator() is not { } navigator || quote is null)
                {
                    VerificationArgs.Log($"route={spec} failed: dashboard={dashboard is not null} quote={quote is not null}");
                    return;
                }
                var response = await navigator.NavigateRouteAsync(dashboard, "QuoteEditor", data: quote);
                VerificationArgs.Log($"route={spec} quote={quote.QuoteNumber} success={response?.Success}");
                return;
            }

            if (detail)
            {
                await Task.Delay(1500);
                var selected = route switch
                {
                    "Dashboard" => await SelectFirstAsync<DashboardPage, DashboardModel>(root, vm => (vm as DashboardViewModel)?.Model, async m =>
                        {
                            var all = await Host.Services.GetRequiredService<IQuoteRepository>().GetAllAsync();
                            var first = all.OrderByDescending(q => q.LineItems.Count).First();
                            await m.OpenQuote(new QuoteListItem(first, IsSelected: true), CancellationToken.None);
                            return first.Title;
                        }),
                    "Clients" => await SelectFirstAsync<ClientsPage, ClientsModel>(root, vm => (vm as ClientsViewModel)?.Model, async m =>
                        {
                            var first = (await m.Clients).First();
                            await m.SelectClient(first, CancellationToken.None);
                            return first.Name;
                        }),
                    "Catalog" => await SelectFirstAsync<CatalogPage, CatalogModel>(root, vm => (vm as CatalogViewModel)?.Model, async m =>
                        {
                            var first = (await m.CategoryCards).First();
                            await m.SelectCategoryCard(first, CancellationToken.None);
                            return first.Name;
                        }),
                    _ => null,
                };
                VerificationArgs.Log($"route={spec} selected={selected}");
            }
        }
        catch (Exception ex)
        {
            VerificationArgs.Log($"failed: {ex}");
        }
    }

    /// <summary>Waits for the page, then runs <paramref name="select"/> on its model (the generated view model's Model).</summary>
    private static async Task<string?> SelectFirstAsync<TPage, TModel>(FrameworkElement root, Func<object?, TModel?> model, Func<TModel, Task<string>> select)
        where TPage : FrameworkElement
        where TModel : class
    {
        var page = await WaitForAsync<TPage>(root);
        return page is not null && model(page.DataContext) is { } found ? await select(found) : null;
    }

    private static async Task<T?> WaitForAsync<T>(DependencyObject root) where T : DependencyObject
    {
        for (var i = 0; i < 100; i++)
        {
            if (Find<T>(root) is { } found)
            {
                return found;
            }
            await Task.Delay(100);
        }
        return null;
    }

    private static IEnumerable<T> Descendants<T>(DependencyObject parent) where T : DependencyObject
    {
        for (var i = 0; i < Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(parent, i);
            if (child is T match)
            {
                yield return match;
            }
            foreach (var nested in Descendants<T>(child))
            {
                yield return nested;
            }
        }
    }

    private static T? Find<T>(DependencyObject parent) where T : DependencyObject
    {
        for (var i = 0; i < Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(parent, i);
            if (child is T match && (child is not UIElement element || element.Visibility is Visibility.Visible))
            {
                return match;
            }
            if (Find<T>(child) is { } found)
            {
                return found;
            }
        }
        return null;
    }
#else
    private static Task PrepareVerificationDataAsync() => Task.CompletedTask;

    private static Task ApplyVerificationViewAsync() => Task.CompletedTask;
#endif
}
