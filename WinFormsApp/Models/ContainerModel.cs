using Newtonsoft.Json;

namespace WarehouseWinForms.Models
{
    public class ContainerModel
    {
        [JsonProperty("container_id")]
        public string ContainerId { get; set; } = "";

        [JsonProperty("item_name")]
        public string ItemName { get; set; } = "";

        [JsonProperty("weight")]
        public float Weight { get; set; }

        [JsonProperty("arrival_date")]
        public string ArrivalDate { get; set; } = "";

        [JsonProperty("shelf")]
        public string Shelf { get; set; } = "";

        [JsonProperty("floor")]
        public int Floor { get; set; }

        [JsonProperty("slot")]
        public int Slot { get; set; }

        [JsonProperty("width")]
        public float Width { get; set; }

        [JsonProperty("depth")]
        public float Depth { get; set; }

        [JsonProperty("height")]
        public float Height { get; set; }

        public string Location => $"{Shelf}-{Floor}-{Slot}";
    }
}
