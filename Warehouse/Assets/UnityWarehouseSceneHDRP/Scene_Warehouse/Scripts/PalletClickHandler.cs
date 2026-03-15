using UnityEngine;

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

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, palletLayer))
            {
                PalletSlot slot = hit.collider.GetComponentInParent<PalletSlot>();
                if (slot != null)
                    warehouseUI.OpenPopup(slot);
            }
        }
    }
}
