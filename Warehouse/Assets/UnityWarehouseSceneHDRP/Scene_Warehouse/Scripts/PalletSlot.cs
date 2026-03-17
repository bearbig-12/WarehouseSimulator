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

        public bool IsEmpty => container == null || string.IsNullOrEmpty(container.containerId);

        private BoxVisualizer _visualizer;

        private void Awake()
        {
            _visualizer = GetComponent<BoxVisualizer>();
        }

        // Inspector에서 이름 기반으로 위치 자동 파싱
        // 이름 형식: Pallet_A_0_3 또는 Pallet_0_3 (shelf는 부모에서 파싱)
        public void ParseNameToPosition()
        {
            string[] parts = gameObject.name.Split('_');
            if (parts.Length >= 4)
            {
                // Pallet_A_0_3 형식
                shelf = parts[1];
                int.TryParse(parts[2], out floor);
                int.TryParse(parts[3], out slot);
            }
            else if (parts.Length >= 3)
            {
                // Pallet_0_3 형식 → shelf는 부모 계층에서 찾기 (Shelf_A)
                int.TryParse(parts[1], out floor);
                int.TryParse(parts[2], out slot);
                shelf = GetShelfFromParent();
            }
        }

        private string GetShelfFromParent()
        {
            Transform t = transform.parent;
            while (t != null)
            {
                if (t.name.StartsWith("Shelf_") && t.name.Length > 6)
                    return t.name.Substring(6, 1); // "Shelf_A" → "A"
                t = t.parent;
            }
            return "";
        }

        // 폴링 동기화 시 슬롯 비우기 (DB 쓰기 없음)
        public void ClearContainer()
        {
            container = null;
            if (_visualizer != null) _visualizer.ReturnBox();
        }

        // 게임 시작 시 DB 데이터로 슬롯 복원 (DB 쓰기 없음)
        public void LoadContainer(ContainerData data)
        {
            container = data;
            if (_visualizer != null) _visualizer.SpawnBox(data.width, data.depth, data.height);
            Debug.Log($"[{gameObject.name}] DB 복원: {data.containerId}");
        }

        // 컨테이너 배치 (입고)
        public void PlaceContainer(ContainerData data)
        {
            container       = data;
            container.shelf = shelf;
            container.floor = floor;
            container.slot  = slot;
            DatabaseManager.Instance.InsertContainer(container);
            if (_visualizer != null) _visualizer.SpawnBox(data.width, data.depth, data.height);
            Debug.Log($"[{gameObject.name}] 입고: {data.containerId}");
        }

        // 컨테이너 제거 (이동 또는 출고)
        public ContainerData RemoveContainer(bool deleteFromDB = false)
        {
            var data = container;
            container = null;
            if (_visualizer != null) _visualizer.ReturnBox();
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

            var data = RemoveContainer(); // 박스 제거 포함
            targetSlot.container       = data;
            targetSlot.container.shelf = targetSlot.shelf;
            targetSlot.container.floor = targetSlot.floor;
            targetSlot.container.slot  = targetSlot.slot;
            if (targetSlot._visualizer != null) targetSlot._visualizer.SpawnBox(data.width, data.depth, data.height); // 목적지에 박스 생성
            DatabaseManager.Instance.UpdateContainerPosition(
                data.containerId, targetSlot.shelf, targetSlot.floor, targetSlot.slot);
            Debug.Log($"[{gameObject.name}] → [{targetSlot.gameObject.name}] 이동: {data.containerId}");
        }

        private void OnValidate()
        {
            ParseNameToPosition();
        }
    }
}
