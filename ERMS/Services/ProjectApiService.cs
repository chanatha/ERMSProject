using ERMS.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ERMS.Services
{
    public class ProjectApiService
    {
        private readonly IHttpClientFactory _httpFactory;

        public ProjectApiService(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        private HttpClient CreateClient(string token)
        {
            var client = _httpFactory.CreateClient("ERMSAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public virtual async Task<List<Project>> GetAllAsync(string token)
        {
            var client = CreateClient(token);
            var response = await client.GetAsync("api/Projects");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Project>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public virtual async Task<Project> GetByIdAsync(int id, string token)
        {
            var client = CreateClient(token);
            var response = await client.GetAsync($"api/Project/{id}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Project>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public virtual async Task CreateAsync(Project project, string token)
        {
            var client = CreateClient(token);
            var json = JsonSerializer.Serialize(project);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/createProject", content);
            response.EnsureSuccessStatusCode();
        }

        public virtual async Task UpdateAsync(Project project, string token)
        {
            var client = CreateClient(token);
            var json = JsonSerializer.Serialize(project);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/editProject/{project.Id}", content);
            response.EnsureSuccessStatusCode();
        }

        public virtual async Task DeleteAsync(int id, string token)
        {
            var client = CreateClient(token);
            var response = await client.DeleteAsync($"api/deleteProject/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
