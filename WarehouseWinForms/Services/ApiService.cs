using Newtonsoft.Json;
using System.Text;
using WarehouseWinForms.Models;

namespace WarehouseWinForms.Services
{
    public class ApiService
    {
        private readonly HttpClient _http = new();
        private const string BASE = "http://localhost:3000";

        public async Task<List<ContainerModel>> GetAllAsync()
        {
            var json = await _http.GetStringAsync($"{BASE}/containers");
            return JsonConvert.DeserializeObject<List<ContainerModel>>(json) ?? new();
        }

        public async Task<bool> IncomingAsync(ContainerModel c)
        {
            var body = JsonConvert.SerializeObject(new {
                container_id = c.ContainerId, item_name = c.ItemName,
                weight = c.Weight, arrival_date = c.ArrivalDate,
                shelf = c.Shelf, floor = c.Floor, slot = c.Slot,
                width = c.Width, depth = c.Depth, height = c.Height
            });
            var res = await _http.PostAsync($"{BASE}/containers",
                new StringContent(body, Encoding.UTF8, "application/json"));
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> MoveAsync(string id, string shelf, int floor, int slot)
        {
            var body = JsonConvert.SerializeObject(new { shelf, floor, slot });
            var req = new HttpRequestMessage(HttpMethod.Patch, $"{BASE}/containers/{id}/move")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            var res = await _http.SendAsync(req);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> OutgoingAsync(string id)
        {
            var res = await _http.DeleteAsync($"{BASE}/containers/{id}");
            return res.IsSuccessStatusCode;
        }
    }
}
