using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ERMS.Models;
namespace ERMS.Services
{
    public class EmployeeApiService 

    {
        private readonly IHttpClientFactory _httpFactory;

        public EmployeeApiService(IHttpClientFactory factory)
        {
            _httpFactory = factory;
        }

        private HttpClient CreateClient(string token)
        {
            var client = _httpFactory.CreateClient("ERMSAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public virtual async Task<List<Employee>> GetAllAsync(string token)
        {
            var client = CreateClient(token);
            var response = await client.GetAsync("api/Employees");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Employee>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public virtual  async Task<Employee> GetByIdAsync(int id, string token)
        {
            var client = CreateClient(token);
            var response = await client.GetAsync($"api/Employee/{id}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Employee>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public virtual async Task CreateAsync(Employee emp, string token)
        {
            var client = CreateClient(token);
            var json = JsonSerializer.Serialize(emp);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/createEmployee", content);
            response.EnsureSuccessStatusCode();
        }

        public virtual async Task UpdateAsync(Employee emp, string token)
        {
            var client = CreateClient(token);
            var json = JsonSerializer.Serialize(emp);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/editEmployee/{emp.Id}", content);
            response.EnsureSuccessStatusCode();
        }

        public virtual async Task DeleteAsync(int id, string token)
        {
            var client = CreateClient(token);
            var response = await client.DeleteAsync($"api/deleteEmployee/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}