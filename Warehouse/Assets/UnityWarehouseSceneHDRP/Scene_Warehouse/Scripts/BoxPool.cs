using System.Collections.Generic;
using UnityEngine;

namespace UnityWarehouseSceneHDRP
{
    /// <summary>
    /// PlasticBox_A 프리팹 오브젝트 풀.
    /// 씬에 싱글턴으로 배치. initialSize는 총 팔레트 슬롯 수 이상으로 설정.
    /// </summary>
    public class BoxPool : MonoBehaviour
    {
        public static BoxPool Instance { get; private set; }

        [Header("풀 설정")]
        [SerializeField] private GameObject boxPrefab;   // PlasticBox_A 프리팹 연결
        [SerializeField] private int initialSize = 96;   // Shelf 4 × Floor 3 × Slot 8

        private readonly Queue<GameObject> _pool = new();
        private Transform _poolRoot;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            _poolRoot = new GameObject("BoxPool_Root").transform;
            _poolRoot.SetParent(transform);

            for (int i = 0; i < initialSize; i++)
                _pool.Enqueue(CreateBox());
        }

        /// <summary>풀에서 박스를 꺼냅니다. 부족하면 새로 생성합니다.</summary>
        public GameObject Get()
        {
            GameObject box = _pool.Count > 0 ? _pool.Dequeue() : CreateBox();
            box.SetActive(true);
            return box;
        }

        /// <summary>박스를 풀로 반환합니다.</summary>
        public void Return(GameObject box)
        {
            box.SetActive(false);
            box.transform.SetParent(_poolRoot, false);
            _pool.Enqueue(box);
        }

        private GameObject CreateBox()
        {
            var box = Instantiate(boxPrefab, _poolRoot);
            box.SetActive(false);
            return box;
        }
    }
}
