using UnityEngine;

namespace UnityWarehouseSceneHDRP
{
    /// <summary>
    /// PalletSlot에 붙여 사용. BoxPool에서 PlasticBox_A를 꺼내 슬롯에 배치하고,
    /// 출고/이동 시 풀로 반환합니다.
    /// </summary>
    [RequireComponent(typeof(PalletSlot))]
    public class BoxVisualizer : MonoBehaviour
    {
        [Header("박스 설정")]
        [SerializeField] private float sizeMultiplier = 0.3f; // 입력값(m) → Unity 유닛 변환 배율
        [SerializeField] private float yOffset = 0f;          // 팔레트 피벗 기준 높이 보정

        private GameObject _boxObject;
        private PalletSlot _slot;

        private void Awake()
        {
            _slot = GetComponent<PalletSlot>();
        }

        /// <summary>풀에서 박스를 꺼내 슬롯에 배치합니다.</summary>
        public void SpawnBox(float width, float depth, float height)
        {
            ReturnBox();

            _boxObject = BoxPool.Instance.Get();
            _boxObject.transform.SetParent(transform, false);

            float w = width  * sizeMultiplier;
            float d = depth  * sizeMultiplier;
            float h = height * sizeMultiplier;

            _boxObject.transform.localPosition = Vector3.zero;
            _boxObject.transform.localScale    = new Vector3(w, h, d);
        }

        /// <summary>박스를 풀로 반환합니다.</summary>
        public void ReturnBox()
        {
            if (_boxObject == null) return;
            BoxPool.Instance.Return(_boxObject);
            _boxObject = null;
        }
    }
}
