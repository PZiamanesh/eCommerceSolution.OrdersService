using OrdersMicroservice.Core.DTOs;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace OrdersMicroservice.Core.HttpClients;

public class UsersMicroserviceClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions jsonOptions;

    public UsersMicroserviceClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public async Task<UserDto?> GetUserByUserId(Guid userId)
    {
        var response = await httpClient.GetAsync($"/api/users/{userId}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var ssss = await response.Content.ReadAsStringAsync();

        var userDto = await response.Content.ReadFromJsonAsync<UserDto>(jsonOptions);

        return userDto;
    }
}
