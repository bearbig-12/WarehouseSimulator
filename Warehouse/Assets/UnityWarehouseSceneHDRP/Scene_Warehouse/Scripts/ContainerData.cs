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

        public ContainerData() { }

        public ContainerData(string containerId, string itemName, float weight,
                             string arrivalDate, string shelf, int floor, int slot)
        {
            this.containerId = containerId;
            this.itemName    = itemName;
            this.weight      = weight;
            this.arrivalDate = arrivalDate;
            this.shelf       = shelf;
            this.floor       = floor;
            this.slot        = slot;
        }
    }
}
