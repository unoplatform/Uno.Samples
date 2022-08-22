using Commerce.ViewModels;
using Refit;
using System.Text.Json.Serialization;

namespace Commerce.Data;

public interface IProductEndpoint
{
    [Get("/products?limit={count}&skip={start}")]
    Task<ProductsResponse> ProductsAsync(CancellationToken ct, int start, int count);

}

public interface IReviewsEndpoint
{
    ValueTask<ReviewData[]> GetReviews(int productId, CancellationToken ct);
}

public interface IAuthenticationEndpoint
{
    [Post("/auth/login")]
    Task<AuthResponse> Login(Credentials credentials, CancellationToken ct);
}

public static class DummyJsonEndpointConstants
{
    // See https://dummyjson.com/docs/auth for username/password values
    public const string ValidUserName = "kminchelle";
    public const string ValidPassword = "0lelplR";
}


public class AuthResponse
{
    public string? Token { get; set; }
}

public class ProductsResponse : BaseResponse
{
    public ProductData[]? Products { get; set; }

}

public class BaseResponse
{
    public int Total { get; set; }
    public string? Skip { get; set; }
    public int Limit { get; set; }
}

public record ProductData
{
    [JsonPropertyName("id")]
    public int ProductId { get; init; }
    public string? Brand { get; init; }
    [JsonPropertyName("title")]
    public string? Name { get; init; }
    public string? LongName { get; init; }
    public string? Description { get; init; }
    public string? Category { get; init; }
    [JsonPropertyName("price")]
    public double? FullPrice { get; init; }
    [JsonIgnore]
    public double Price => (FullPrice??0.0) * (1 - (Discount??0.0) / 100.0);
    [JsonPropertyName("discountPercentage")]
    public double? Discount { get; init; }
    [JsonIgnore]
    public string? Photo => Photos?.FirstOrDefault();
    [JsonPropertyName("images")]
    public string[]? Photos { get; init; }
    public double? Rating { get; init; } = Random.Shared.NextDouble()*5.0;
    //public ReviewData[]? Reviews { get; init; }
}
