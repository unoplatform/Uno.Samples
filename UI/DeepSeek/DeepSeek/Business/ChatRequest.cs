namespace DeepSeek.Business;

public record struct ChatRequest(string Model, IImmutableList<ChatEntry> Messages, bool Stream);
