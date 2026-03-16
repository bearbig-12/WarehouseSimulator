using System;

namespace UnityWarehouseSceneHDRP
{
    [Serializable]
    public class ContainerData
    {
        public string containerId;   // 기본키 (예: CNT-001)
        public string itemName;      // 물건 이름
        public float  weight;        // 무게 (kg)
        public string arrivalDate;   // 입고 날짜 (yyyy-MM-dd)
        public string shelf;         // 열 (A/B/C/D)
        public int    floor;         // 층 (0/1/2)
        public int    slot;          // 슬롯 (0~7)
        public float  width;         // 가로 (m)
        public float  depth;         // 세로 (m)
        public float  height;        // 높이 (m)

        public ContainerData() { }

        public ContainerData(string containerId, string itemName, float weight,
                             string arrivalDate, string shelf, int floor, int slot,
                             float width = 1f, float depth = 1f, float height = 1f)
        {
            this.containerId = containerId;
            this.itemName    = itemName;
            this.weight      = weight;
            this.arrivalDate = arrivalDate;
            this.shelf       = shelf;
            this.floor       = floor;
            this.slot        = slot;
            this.width       = width;
            this.depth       = depth;
            this.height      = height;
        }
    }
}
