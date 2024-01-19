# MVUX

## Guidance to use Records with MVUX

### What is a Record

Records are special types that focus on comparing values. C# 10 introduces record structs, allowing you to make records as value types. A key feature is value-based equality, meaning two variables of a record type are considered equal if their definitions match exactly, and the values in their fields are the same. This is different from class types, where equality depends on referring to the exact same object.

An essential point about records is that they are immutable, meaning you can't change their values once set. The compiler automatically generates methods to support this when you declare a record. This makes records useful when you want immutability and value-based equality in your code.

### How to create immutable records

You can achieve creating immutable records in two ways. First, by declaring your record with parameters that will create an immutable record with the parameters as its properties:

```cs
public partial record ChatResponse(string Message, bool IsError);
```

Another way is by creating properties using the `init` keyword instead of `set` to enforce immutability. Here's a brief example:

```cs
public partial record ChatResponse
{
    public string Message { get; init; }
    public bool IsError { get; init; }
}
```

### How to use records with MVUX

<!-- Service/Business layer creates the record and send it to the Presentation layer -->

Records can be instantiated from the Presentation layer as parameter for a request or the business layer where data is usually retrieved/processed. For example in our `ChatService` we would have the following method being called from the Model. A `ChatEntry` list is received as parameter from the Model, where `ChatEntry` is also a record, and we are creating the instance of `ChatResponse` and returning it to the Model:

```cs
public async ValueTask<ChatResponse> AskAsync(IImmutableList<ChatEntry> history)
{
    var request = CreateRequest(history);

    var result = await _client.CreateCompletion(request);

    if (result.Successful)
    {
        var response = result.Choices.Select(choice => choice.Message.Content);

        return new ChatResponse(string.Join("", responseContent));
    }
    else
    {
        return new ChatResponse(result.Error?.Message, IsError: true);
    }
}
```

### Updating records

As we are dealing with immutable records it's not possible to update it or its properties, to achieve that we need to create a new instance based on the previous record. This ensures we are not modifying data from the UI in the wrong thread. See the example:

Given the `Message` record:

```cs
public partial record Message(string Content, Status status, Source source);
```

In our Model:

```cs
var message = new Message("Hello world!", Status.Value, Source.User);

...

message = message with
{
    Content = response.Message,
    Status = response.IsError ? Status.Error : Status.Value
};

//Then you can update your message list displayed in the UI thread safe
await Messages.UpdateAsync(message);
...

```