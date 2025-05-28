using DTOsLibrary;
using System.Net.Http;
using System.Net.Http.Json;

namespace UI.ApiClients;

public class PreUserApiClient : AbstractApiClient
{
    public PreUserApiClient(HttpClient client) : base(client) { }

    public async Task<BaseUserDto?> AuthorizeUserAsync(string rawlogin, string rawPassword)
    {
        var userDto = new AuthorizationDto
        {
            login = rawlogin,
            password = rawPassword
        };

        var response = await client.PostAsJsonAsync("authorizate", userDto);

        if (!await HandleErrorAsync(response))
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<BaseUserDto>();
    }

    public async Task<bool> CreateUserAsync(BaseUserDto userDto)
    {
        var response = await client.PostAsJsonAsync("register", userDto);
        return await HandleErrorAsync(response);
    }
}
