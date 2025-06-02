using DTOsLibrary;
using DTOsLibrary.DTOEnums;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace UI.ApiClients
{
    public class AdministratorApiClient : AbstractApiClient
    {
        public AdministratorApiClient(HttpClient client) : base(client) { }

        public async Task<List<AuctionLotDto>> GetAuctionLotsAsync()
        {
            var response = await client.GetAsync("lots");
            return await HandleResponseAsync<List<AuctionLotDto>>(response);
        }

        // ============ Користувачі ============
        public async Task<List<BaseUserDto>> GetUsersAsync()
        {
            var response = await client.GetAsync("users");
            return await HandleResponseAsync<List<BaseUserDto>>(response);
        }

        public async Task<BaseUserDto?> GetUserByIdAsync(int id)
        {
            var response = await client.GetAsync($"users/{id}");
            return await HandleResponseAsync<BaseUserDto?>(response);
        }

        public async Task<bool> UpdateUsersAsync(List<BaseUserDto> usersDto)
        {
            var response = await client.PutAsJsonAsync("users", usersDto);
            return await HandleErrorAsync(response);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var response = await client.DeleteAsync($"admin/users/{id}");
            return await HandleErrorAsync(response);
        }

        // ============ Логи дій ============
        public async Task<List<ActionLogDto>> GetLogsAsync(DateTime? date)
        {
            var endpoint = "logs";
            if (date.HasValue)
            {
                endpoint += "/" + Uri.EscapeDataString(date.Value.ToString("yyyy-MM-dd"));
            }

            var response = await client.GetAsync(endpoint);
            return await HandleResponseAsync<List<ActionLogDto>>(response);
        }

        // ============ Реалізатори кодів ============
        public async Task<List<SecretCodeRealizatorDto>> GetCodeRealizatorsAsync()
        {
            var response = await client.GetAsync("secretcoderealizators");
            return await HandleResponseAsync<List<SecretCodeRealizatorDto>>(response);
        }

        public async Task<bool> CreateCodeRealizatorAsync(SecretCodeRealizatorDto realizatorDto)
        {
            var response = await client.PostAsJsonAsync("secretcoderealizators", realizatorDto);
            return await HandleErrorAsync(response);
        }

        public async Task<bool> DeleteCodeRealizatorAsync(int id)
        {
            var response = await client.DeleteAsync($"secretcoderealizators/{id}");
            return await HandleErrorAsync(response);
        }

        // ============ Універсальна обробка відповідей ============
        private async Task<T> HandleResponseAsync<T>(HttpResponseMessage response) 
            where T : new()
        {
            if (!await HandleErrorAsync(response))
            {
                return new T();
            }
            return await response.Content.ReadFromJsonAsync<T>() ?? new T();
        }
    }
}