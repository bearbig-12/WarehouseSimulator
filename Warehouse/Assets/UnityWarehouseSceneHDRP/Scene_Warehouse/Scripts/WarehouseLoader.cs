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
        private IEnumerator Start()
        {
            // BoxPool.Awake() / DatabaseManager.Awake() 완료 대기
            yield return null;

            DatabaseManager.Instance.LoadAllContainers(OnLoaded);
        }

        private void OnLoaded(ContainerData[] containers)
        {
            // shelf-floor-slot 키로 빠르게 슬롯 검색
            var slotMap = new Dictionary<string, PalletSlot>();
            foreach (var slot in FindObjectsByType<PalletSlot>(FindObjectsSortMode.None))
                slotMap[$"{slot.shelf}_{slot.floor}_{slot.slot}"] = slot;

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
    }
}
