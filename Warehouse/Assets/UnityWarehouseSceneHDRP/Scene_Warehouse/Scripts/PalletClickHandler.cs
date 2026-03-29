using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityWarehouseSceneHDRP
{
    // 마우스 클릭으로 팔레트 선택
    public class PalletClickHandler : MonoBehaviour
    {
        [SerializeField] private WarehouseUI warehouseUI;
        [SerializeField] private LayerMask   palletLayer;

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, palletLayer))
            {
                // shelf 정보가 있는 PalletSlot을 찾을 때까지 부모 방향으로 탐색
                // (Pallet 프리팹 자체에도 PalletSlot이 붙어있을 경우 건너뜀)
                PalletSlot slot = null;
                Transform t = hit.collider.transform;
                while (t != null)
                {
                    var ps = t.GetComponent<PalletSlot>();
                    if (ps != null && !string.IsNullOrEmpty(ps.shelf))
                    {
                        slot = ps;
                        break;
                    }
                    t = t.parent;
                }

                if (slot != null)
                    warehouseUI.OpenPopup(slot);
            }
        }
    }
}
