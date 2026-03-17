using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityWarehouseSceneHDRP
{
    /// <summary>
    /// 게임 시작 시 DB에서 컨테이너 데이터를 불러와 팔레트 슬롯에 복원합니다.
    /// DatabaseManager, BoxPool보다 늦게 초기화되어야 하므로 한 프레임 대기 후 실행.
    /// </summary>
    public class WarehouseLoader : MonoBehaviour
    {
        [SerializeField] private float pollInterval = 3f;

        private IEnumerator Start()
        {
            // BoxPool.Awake() / DatabaseManager.Awake() 완료 대기
            yield return null;

            DatabaseManager.Instance.LoadAllContainers(OnLoaded);

            // WinForms 등 외부 변경 실시간 반영 폴링
            while (true)
            {
                yield return new WaitForSeconds(pollInterval);
                DatabaseManager.Instance.LoadAllContainers(OnRefresh);
            }
        }

        private void OnLoaded(ContainerData[] containers)
        {
            var slotMap = BuildSlotMap();

            int loaded = 0;
            foreach (var data in containers)
            {
                string key = $"{data.shelf}_{data.floor}_{data.slot}";
                if (slotMap.TryGetValue(key, out PalletSlot slot))
                {
                    slot.LoadContainer(data);
                    loaded++;
                }
                else
                {
                    Debug.LogWarning($"[WarehouseLoader] 슬롯 없음: {key} ({data.containerId})");
                }
            }

            Debug.Log($"[WarehouseLoader] {loaded}/{containers.Length}개 컨테이너 복원 완료");
        }

        // 폴링: DB 상태와 씬 동기화
        private void OnRefresh(ContainerData[] containers)
        {
            var slotMap   = BuildSlotMap();
            var dbIds     = new HashSet<string>();

            // DB에 있는 컨테이너 반영
            foreach (var data in containers)
            {
                dbIds.Add(data.containerId);
                string key = $"{data.shelf}_{data.floor}_{data.slot}";
                if (!slotMap.TryGetValue(key, out PalletSlot slot)) continue;

                // 같은 컨테이너면 스킵, 다르면 갱신
                if (!slot.IsEmpty && slot.container.containerId == data.containerId) continue;
                slot.LoadContainer(data);
            }

            // DB에 없는 컨테이너는 슬롯에서 제거
            foreach (var slot in slotMap.Values)
            {
                if (!slot.IsEmpty && !dbIds.Contains(slot.container.containerId))
                    slot.ClearContainer();
            }
        }

        private Dictionary<string, PalletSlot> BuildSlotMap()
        {
            var map = new Dictionary<string, PalletSlot>();
            foreach (var slot in FindObjectsByType<PalletSlot>(FindObjectsSortMode.None))
                map[$"{slot.shelf}_{slot.floor}_{slot.slot}"] = slot;
            return map;
        }
    }
}
