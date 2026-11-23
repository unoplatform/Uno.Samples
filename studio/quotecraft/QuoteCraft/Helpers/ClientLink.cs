namespace QuoteCraft.Helpers;

/// <summary>
/// Which client a quote belongs to. A quote keeps the client's name (shown on the quote and the PDF) and the client's
/// id (the client's quote count and total, and the address Send uses); this keeps the id in step with the name.
/// </summary>
public static class ClientLink
{
    /// <param name="name">The client name the quote has now (typed, or picked from the suggestions).</param>
    /// <param name="savedName">The name the quote was saved with.</param>
    /// <param name="savedClientId">The client the quote was saved with.</param>
    /// <returns>
    /// The client id to save. An unchanged name keeps its client (even if the client was renamed since); a new name
    /// links to the client with that name, ignoring case; a name no client has links to none.
    /// </returns>
    public static string? Resolve(string? name, string? savedName, string? savedClientId, IEnumerable<ClientEntity> clients)
    {
        var trimmed = name?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            return null;
        }

        if (savedClientId is not null && string.Equals(trimmed, savedName?.Trim(), StringComparison.Ordinal))
        {
            return savedClientId;
        }

        return clients.FirstOrDefault(c => string.Equals(c.Name.Trim(), trimmed, StringComparison.OrdinalIgnoreCase))?.Id;
    }

    /// <summary>
    /// A client field's suggestions (the editor's Client and New Quote's): up to eight client names containing what is
    /// typed, ignoring case. None for an empty field, and a name typed in full is not offered again.
    /// </summary>
    public static async Task<IImmutableList<string>> SuggestAsync(string? typed, IClientRepository clients)
    {
        var query = typed?.Trim() ?? string.Empty;
        if (query.Length == 0)
        {
            return ImmutableList<string>.Empty;
        }
        return (await clients.GetAllAsync())
            .Select(c => c.Name)
            .Where(n => n.Contains(query, StringComparison.OrdinalIgnoreCase) && n != query)
            .Take(8)
            .ToImmutableList();
    }
}
