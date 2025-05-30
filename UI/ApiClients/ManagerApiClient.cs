using System;
using System.Collections.Generic;
using DTOsLibrary;
using System.Net.Http;
using System.Net.Http.Json;

namespace UI.ApiClients
{
    public class ManagerApiClient : AbstractApiClient
    {
        public ManagerApiClient(HttpClient client) : base(client) { }

        public async Task<bool> CreateCategoryAsync(string categoryName)
        {
            var response = await client.PostAsJsonAsync($"category/{categoryName}", categoryName);
            return await HandleErrorAsync(response);
        }
        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var response = await client.DeleteAsync($"category/{categoryId}");
            return await HandleErrorAsync(response);
        }
        public async Task<bool> ApproveLotAsync(int lotId)
        {
            var response = await client.PostAsJsonAsync($"approve-lot/{lotId}", lotId);
            return await HandleErrorAsync(response);
        }
        public async Task<bool> RejectLotAsync(int lotId)
        {
            var response = await client.PostAsJsonAsync($"reject-lot/{lotId}", lotId);
            return await HandleErrorAsync(response);
        }
        public async Task<bool> StopLotAsync(int lotId)
        {
            var response = await client.PostAsJsonAsync($"stop-lot/{lotId}", lotId);
            return await HandleErrorAsync(response);
        }

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

        public async Task<List<AuctionLotDto>> SearchLotsAsync(SearchLotsDto dto)
        {
            var response = await client.PostAsJsonAsync("search", dto);

            if (!await HandleErrorAsync(response))
            {
                return new List<AuctionLotDto>();
            }

            var lots = await response.Content.ReadFromJsonAsync<List<AuctionLotDto>>();
            return lots ?? new List<AuctionLotDto>();
        }
    }
}
