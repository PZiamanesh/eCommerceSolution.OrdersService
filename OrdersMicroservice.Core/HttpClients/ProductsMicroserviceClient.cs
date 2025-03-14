using OrdersMicroservice.Core.DTOs;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace OrdersMicroservice.Core.HttpClients;

public class ProductsMicroserviceClient
{
    private readonly HttpClient httpClient;
    private readonly IDistributedCache distributedCache;
    private readonly JsonSerializerOptions jsonOptions;

    public ProductsMicroserviceClient(
        HttpClient httpClient,
        IDistributedCache distributedCache
        )
    {
        this.httpClient = httpClient;
        this.distributedCache = distributedCache;
        jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public async Task<ProductDTO?> GetProductByProductId(Guid productId)
    {
        string cacheKey = $"product_{productId}";
        string? cachedProduct = await distributedCache.GetStringAsync(cacheKey);

        if (cachedProduct != null)
        {
            return JsonSerializer.Deserialize<ProductDTO>(cachedProduct);
        }

        var response = await httpClient.GetAsync($"/gateway/products/search/product-id/{productId}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var productDto = await response.Content.ReadFromJsonAsync<ProductDTO>(jsonOptions);
        if (productDto is null)
        {
            return null;
        }

        var cachOptions = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(30))
            .SetSlidingExpiration(TimeSpan.FromSeconds(10));

        await distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize<ProductDTO>(productDto), cachOptions);

        return productDto;
    }
}
