using DeepSeek.Business;
using Refit;

namespace DeepSeek.Services.Endpoints;

[Headers("Content-Type: application/json")]
public interface IDeepSeekApiClient
{
    [Post("")]
    Task<ApiResponse<ChatResponse>> AskAsync(CancellationToken cancellationToken, [Body]ChatRequest chatRequest);
}
