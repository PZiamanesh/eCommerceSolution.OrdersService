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

    public async Task<UserDTO?> GetUserByUserId(Guid userID)
    {
        var response = await httpClient.GetAsync($"/gateway/users/{userID}");

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("error finding user", null, System.Net.HttpStatusCode.BadRequest);
        }

        var ssss = await response.Content.ReadAsStringAsync();

        var userDto = await response.Content.ReadFromJsonAsync<UserDTO>(jsonOptions);

        return userDto;
    }
}
