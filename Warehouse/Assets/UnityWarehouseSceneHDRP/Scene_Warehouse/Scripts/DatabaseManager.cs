using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

namespace UnityWarehouseSceneHDRP
{
    public class DatabaseManager : MonoBehaviour
    {
        public static DatabaseManager Instance { get; private set; }

        [Header("API 서버 설정")]
        [SerializeField] private string serverUrl = "http://localhost:3000";

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        void Start() 
        { 
            TestConnection(); 
        }


        // 연결 테스트
        public void TestConnection()
        {
            StartCoroutine(Get("/containers", (result) =>
            {
                Debug.Log($"서버 연결 성공! 응답: {result}");
            }));
        }

        // 전체 컨테이너 로드 (게임 시작 시 DB 동기화용)
        public void LoadAllContainers(System.Action<ContainerData[]> callback)
        {
            StartCoroutine(Get("/containers", (json) =>
            {
                // JsonUtility는 배열 직접 파싱 불가 → 래퍼로 감싸기
                string wrapped = "{\"items\":" + json + "}";
                var wrapper = JsonUtility.FromJson<ContainerListWrapper>(wrapped);
                var result = new ContainerData[wrapper.items.Length];
                for (int i = 0; i < wrapper.items.Length; i++)
                    result[i] = wrapper.items[i].ToContainerData();
                callback(result);
            }));
        }

        // 컨테이너 입고
        public void InsertContainer(ContainerData data)
        {
            string json = JsonUtility.ToJson(new ContainerJson(data));
            StartCoroutine(Post("/containers", json, (result) =>
            {
                Debug.Log($"입고 완료: {data.containerId}");
            }));
        }

        // 컨테이너 이동
        public void UpdateContainerPosition(string containerId, string shelf, int floor, int slot)
        {
            string json = JsonUtility.ToJson(new MoveJson(shelf, floor, slot));
            StartCoroutine(Patch($"/containers/{containerId}/move", json, (result) =>
            {
                Debug.Log($"이동 완료: {containerId} → {shelf}-{floor}-{slot}");
            }));
        }

        // 컨테이너 출고
        public void DeleteContainer(string containerId)
        {
            StartCoroutine(Delete($"/containers/{containerId}", (result) =>
            {
                Debug.Log($"출고 완료: {containerId}");
            }));
        }

        // ───────────────────────────────────────────
        // HTTP 공통 메서드
        // ───────────────────────────────────────────

        private IEnumerator Get(string path, System.Action<string> callback)
        {
            using var req = UnityWebRequest.Get(serverUrl + path);
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
                callback(req.downloadHandler.text);
            else
                Debug.LogError($"GET 오류: {req.error}");
        }

        private IEnumerator Post(string path, string json, System.Action<string> callback)
        {
            using var req = new UnityWebRequest(serverUrl + path, "POST");
            req.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
                callback(req.downloadHandler.text);
            else
                Debug.LogError($"POST 오류: {req.error}");
        }

        private IEnumerator Patch(string path, string json, System.Action<string> callback)
        {
            using var req = new UnityWebRequest(serverUrl + path, "PATCH");
            req.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
                callback(req.downloadHandler.text);
            else
                Debug.LogError($"PATCH 오류: {req.error}");
        }

        private IEnumerator Delete(string path, System.Action<string> callback)
        {
            using var req = UnityWebRequest.Delete(serverUrl + path);
            req.downloadHandler = new DownloadHandlerBuffer();
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
                callback(req.downloadHandler.text);
            else
                Debug.LogError($"DELETE 오류: {req.error}");
        }

        // ───────────────────────────────────────────
        // JSON 변환용 내부 클래스
        // ───────────────────────────────────────────

        [System.Serializable]
        private class ContainerJson
        {
            public string container_id;
            public string item_name;
            public float  weight;
            public string arrival_date;
            public string shelf;
            public int    floor;
            public int    slot;
            public float  width;
            public float  depth;
            public float  height;

            public ContainerJson(ContainerData d)
            {
                container_id = d.containerId;
                item_name    = d.itemName;
                weight       = d.weight;
                arrival_date = d.arrivalDate;
                shelf        = d.shelf;
                floor        = d.floor;
                slot         = d.slot;
                width        = d.width;
                depth        = d.depth;
                height       = d.height;
            }
        }

        // LoadAllContainers용 서버 응답 파싱 클래스 (snake_case)
        [System.Serializable]
        private class ContainerResponseItem
        {
            public string container_id;
            public string item_name;
            public float  weight;
            public string arrival_date;
            public string shelf;
            public int    floor;
            public int    slot;
            public float  width;
            public float  depth;
            public float  height;

            public ContainerData ToContainerData() => new ContainerData(
                container_id, item_name, weight, arrival_date, shelf, floor, slot, width, depth, height
            );
        }

        [System.Serializable]
        private class ContainerListWrapper
        {
            public ContainerResponseItem[] items;
        }

        [System.Serializable]
        private class MoveJson
        {
            public string shelf;
            public int    floor;
            public int    slot;

            public MoveJson(string shelf, int floor, int slot)
            {
                this.shelf = shelf;
                this.floor = floor;
                this.slot  = slot;
            }
        }
    }
}
