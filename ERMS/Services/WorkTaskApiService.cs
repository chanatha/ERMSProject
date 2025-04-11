using ERMS.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ERMS.Services
{
    public class WorkTaskApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public WorkTaskApiService(IHttpClientFactory factory)
        {
            _httpClientFactory = factory;
        }

        private HttpClient CreateClient(string token)
        {
            var client = _httpClientFactory.CreateClient("ERMSAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public virtual async Task<List<WorkTask>> GetAllAsync(string token)
        {
            var client = CreateClient(token);
            var response = await client.GetAsync("api/worktasks");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<WorkTask>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public virtual async Task<WorkTask> GetByIdAsync(int id, string token)
        {
            var client = CreateClient(token);
            var response = await client.GetAsync($"api/worktask/{id}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<WorkTask>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public virtual async Task CreateAsync(WorkTask task, string token)
        {
            var client = CreateClient(token);
            var json = JsonSerializer.Serialize(task);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/createWorkTask", content);
            response.EnsureSuccessStatusCode();
        }

        public virtual async Task UpdateAsync(WorkTask task, string token)
        {
            var client = CreateClient(token);
            var json = JsonSerializer.Serialize(task);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/editWorkTask/{task.Id}", content);
            response.EnsureSuccessStatusCode();
        }

        public virtual async Task DeleteAsync(int id, string token)
        {
            var client = CreateClient(token);
            var response = await client.DeleteAsync($"api/deleteWorkTask/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
