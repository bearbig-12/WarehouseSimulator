using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityWarehouseSceneHDRP
{
    // VR 카메라 시선 방향으로 Raycast해서 팔레트 선택
    // XR Origin → Camera Offset → Right Controller에 붙임
    public class PalletVRInteractable : MonoBehaviour
    {
        [SerializeField] private WarehouseUI warehouseUI;
        [SerializeField] private float       maxDistance = 20f;

        private Camera _camera;

        private void Awake()
        {
            if (warehouseUI == null)
                warehouseUI = FindObjectOfType<WarehouseUI>();
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (_camera == null) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            // 카메라 중앙에서 앞방향으로 레이캐스트
            Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
            Debug.DrawRay(_camera.transform.position, _camera.transform.forward * maxDistance, Color.green, 2f);

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
            {
                Debug.Log($"[VR] 레이 히트: {hit.collider.gameObject.name} / Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

                Transform t = hit.collider.transform;
                while (t != null)
                {
                    var ps = t.GetComponent<PalletSlot>();
                    if (ps != null && !string.IsNullOrEmpty(ps.shelf))
                    {
                        warehouseUI.OpenPopup(ps);
                        return;
                    }
                    t = t.parent;
                }
                Debug.Log("[VR] PalletSlot 못찾음");
            }
            else
            {
                Debug.Log("[VR] 레이 히트 없음");
            }
        }
    }
}
