using OrdersMicroservice.Core.DTOs;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace OrdersMicroservice.Core.HttpClients;

public class ProductsMicroserviceClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions jsonOptions;

    public ProductsMicroserviceClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public async Task<ProductDTO?> GetProductByProductId(Guid productId)
    {
        var response = await httpClient.GetAsync($"/api/products/search/product-id/{productId}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var ssss = await response.Content.ReadAsStringAsync();

        var productDto = await response.Content.ReadFromJsonAsync<ProductDTO>(jsonOptions);

        return productDto;
    }
}
