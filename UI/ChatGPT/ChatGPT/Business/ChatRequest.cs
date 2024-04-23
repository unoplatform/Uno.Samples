namespace ChatGPT.Business;

public record struct ChatRequest(IImmutableList<ChatEntry> History);