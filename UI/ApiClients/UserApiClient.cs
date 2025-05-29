using DTOsLibrary;
using System.Net.Http;
using System.Net.Http.Json;

namespace UI.ApiClients;

public class UserApiClient : AbstractApiClient
{
    public UserApiClient(HttpClient client) : base(client) { }

    public async Task<List<AuctionLotDto>> GetAuctionLotsAsync()
    {
        var response = await client.GetAsync("lots");

        if (!await HandleErrorAsync(response))
        {
            return new List<AuctionLotDto>();
        }

        var lots = await response.Content.ReadFromJsonAsync<List<AuctionLotDto>>();
        return lots ?? new List<AuctionLotDto>();
    }

    public async Task<bool> CreateLotAsync(AuctionLotDto dto)
    {
        var response = await client.PostAsJsonAsync("lot", dto);
        return await HandleErrorAsync(response);
    }

    public async Task<bool> CreateBidAsync(BidDto dto)
    {
        var response = await client.PostAsJsonAsync("bid", dto);
        return await HandleErrorAsync(response);
    }

    public async Task<bool> DeleteLotAsync(int lotId)
    {
        var response = await client.DeleteAsync($"lot/{lotId}");
        return await HandleErrorAsync(response);
    }

    public async Task<List<AuctionLotDto>> GetUserLotsAsync(int userId)
    {
        var response = await client.GetAsync($"mylots/{userId}");

        if (!await HandleErrorAsync(response))
        {
            return new List<AuctionLotDto>();
        }

        var lots = await response.Content.ReadFromJsonAsync<List<AuctionLotDto>>();
        return lots ?? new List<AuctionLotDto>();
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        var response = await client.GetAsync("categories");

        if (!await HandleErrorAsync(response))
        {
            return new List<CategoryDto>();
        }

        var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
        return categories ?? new List<CategoryDto>();
    }
}
