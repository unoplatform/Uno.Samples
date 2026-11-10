namespace DeepSeek.Business;

public partial record ChatResponse(string Id, string Object, long Created, string Model, IImmutableList<ChatChoice> Choices);
