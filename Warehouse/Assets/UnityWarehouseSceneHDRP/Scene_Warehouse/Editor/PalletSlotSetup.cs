using UnityEngine;
using UnityEditor;

namespace UnityWarehouseSceneHDRP
{
    public class PalletSlotSetup
    {
        [MenuItem("Warehouse/Setup All PalletSlots")]
        public static void SetupAllPalletSlots()
        {
            GameObject shelves = GameObject.Find("Shelves");
            if (shelves == null)
            {
                Debug.LogError("'Shelves' 오브젝트를 찾을 수 없습니다.");
                return;
            }

            int count = 0;

            foreach (Transform shelfGroup in shelves.transform)          // Shelf_A, B, C, D
            {
                foreach (Transform child in shelfGroup)                  // Shelf, Floor_0, Floor_1, Floor_2
                {
                    if (!child.name.StartsWith("Floor")) continue;

                    foreach (Transform pallet in child)                  // Pallet_A_0_0 ...
                    {
                        PalletSlot slot = pallet.gameObject.GetComponent<PalletSlot>();
                        if (slot == null)
                            slot = pallet.gameObject.AddComponent<PalletSlot>();

                        slot.ParseNameToPosition();
                        EditorUtility.SetDirty(pallet.gameObject);
                        count++;
                    }
                }
            }

            Debug.Log($"PalletSlot {count}개 설정 완료!");
        }
    }
}
