using System.Net;
using System.Net.Http.Json;
using Ecommerce_DBFirst.ViewModels;

namespace Ecommerce_DBFirst.Services
{
    public interface IInventoryApiClient
    {
        Task<List<InventoryItemVM>> GetAllAsync();
        Task<InventoryItemVM?> GetByProductIdAsync(int productId);
        Task<(bool IsSuccess, string? Error)> CreateAsync(InventoryCreateVM input);
        Task<(bool IsSuccess, string? Error)> UpdateStockAsync(int productId, InventoryUpdateVM input);
        Task<(bool IsSuccess, string? Error)> DeleteAsync(int productId);
    }

    public class InventoryApiClient : IInventoryApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<InventoryApiClient> _logger;

        public InventoryApiClient(HttpClient httpClient, ILogger<InventoryApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<InventoryItemVM>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("api/inventory");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Inventory API GET all failed. StatusCode: {StatusCode}", response.StatusCode);
                return new List<InventoryItemVM>();
            }

            return await response.Content.ReadFromJsonAsync<List<InventoryItemVM>>() ?? new List<InventoryItemVM>();
        }

        public async Task<InventoryItemVM?> GetByProductIdAsync(int productId)
        {
            var response = await _httpClient.GetAsync($"api/inventory/{productId}");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Inventory API GET by product failed. ProductId: {ProductId}, StatusCode: {StatusCode}", productId, response.StatusCode);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<InventoryItemVM>();
        }

        public async Task<(bool IsSuccess, string? Error)> CreateAsync(InventoryCreateVM input)
        {
            var response = await _httpClient.PostAsJsonAsync("api/inventory", input);
            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Inventory API create failed. ProductId: {ProductId}, StatusCode: {StatusCode}, Error: {Error}",
                input.ProductId, response.StatusCode, error);

            return (false, string.IsNullOrWhiteSpace(error) ? "API create failed." : error);
        }

        public async Task<(bool IsSuccess, string? Error)> UpdateStockAsync(int productId, InventoryUpdateVM input)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/inventory/{productId}", input);
            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Inventory API update failed. ProductId: {ProductId}, StatusCode: {StatusCode}, Error: {Error}",
                productId, response.StatusCode, error);

            return (false, string.IsNullOrWhiteSpace(error) ? "API update failed." : error);
        }

        public async Task<(bool IsSuccess, string? Error)> DeleteAsync(int productId)
        {
            var response = await _httpClient.DeleteAsync($"api/inventory/{productId}");
            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Inventory API delete failed. ProductId: {ProductId}, StatusCode: {StatusCode}, Error: {Error}",
                productId, response.StatusCode, error);

            return (false, string.IsNullOrWhiteSpace(error) ? "API delete failed." : error);
        }
    }
}
