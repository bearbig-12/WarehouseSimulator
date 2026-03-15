using UnityEngine;

namespace UnityWarehouseSceneHDRP
{
    public class PalletSlot : MonoBehaviour
    {
        [Header("위치 정보 (자동 설정됨)")]
        public string shelf;   // A/B/C/D
        public int    floor;   // 0/1/2
        public int    slot;    // 0~7

        [Header("컨테이너 정보 (없으면 비어있음)")]
        public ContainerData container;

        public bool IsEmpty => container == null;

        // Inspector에서 이름 기반으로 위치 자동 파싱
        // 이름 형식: Pallet_A_0_3
        public void ParseNameToPosition()
        {
            string[] parts = gameObject.name.Split('_');
            if (parts.Length >= 4)
            {
                shelf = parts[1];
                int.TryParse(parts[2], out floor);
                int.TryParse(parts[3], out slot);
            }
        }

        // 컨테이너 배치 (입고)
        public void PlaceContainer(ContainerData data)
        {
            container       = data;
            container.shelf = shelf;
            container.floor = floor;
            container.slot  = slot;
            DatabaseManager.Instance.InsertContainer(container);
            Debug.Log($"[{gameObject.name}] 입고: {data.containerId}");
        }

        // 컨테이너 제거 (이동 또는 출고)
        public ContainerData RemoveContainer(bool deleteFromDB = false)
        {
            var data = container;
            container = null;
            if (deleteFromDB && data != null)
                DatabaseManager.Instance.DeleteContainer(data.containerId);
            return data;
        }

        // 다른 슬롯으로 이동
        public void MoveContainerTo(PalletSlot targetSlot)
        {
            if (IsEmpty)
            {
                Debug.LogWarning($"[{gameObject.name}] 비어있어서 이동 불가");
                return;
            }
            if (!targetSlot.IsEmpty)
            {
                Debug.LogWarning($"[{targetSlot.gameObject.name}] 이미 컨테이너 있음");
                return;
            }

            var data = RemoveContainer();
            targetSlot.PlaceContainer(data);
            DatabaseManager.Instance.UpdateContainerPosition(
                data.containerId, targetSlot.shelf, targetSlot.floor, targetSlot.slot);
        }

        private void OnValidate()
        {
            ParseNameToPosition();
        }
    }
}
