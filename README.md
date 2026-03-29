# Warehouse Simulator

Unity HDRP 기반 창고 시뮬레이터 + Unreal Engine 5 포팅 프로젝트

---

## 프로젝트 구조

```
WarehouseSimulator/
├── Warehouse/               # Unity 프로젝트 (HDRP, Unity 2022.3.62f1)
├── WarehouseWinForms/       # WinForms 관리 앱 (.NET 8, Visual Studio 2022)
├── Server/                  # Node.js + Express API 서버
└── WarehouseSimulatorUE/    # Unreal Engine 5.5.4 포팅 프로젝트
```

### 씬 구성
- **OutdoorsScene** : 메인 창고 씬
- 선반 구조: A / B / C / D 열, 각 0~2층, 층당 팔레트 8개

---

## 개발 목표

1. Unity 씬에서 JSON을 사용해 각 슬롯(열/층/번호)의 컨테이너 정보 관리
2. 컨테이너 이동 시 MySQL DB에 실시간 반영
3. DB 기반 웹 대시보드로 창고 현황 시각화

---

## 아키텍처

```
WinForms 앱 ──── REST API ──────────┐
                                    ↓
Unity (씬)                    Node.js + Express 서버 (포트 3000)
    └─ PalletSlot + BoxVisualizer        ↕ WebSocket (실시간)
         ↓ 입고 / 이동 / 출고 이벤트  WinForms (실시간 반영)
    DatabaseManager (UnityWebRequest)
         ↓ HTTP REST API              ↓
    Node.js + Express 서버       MySQL DB (warehouse_db)

UE5 (씬)
    └─ PalletSlot (UActorComponent)
         ↓ 입고 / 이동 / 출고 이벤트
    WarehouseHttpManager (REST API)
    WarehouseWebSocketManager (WebSocket 실시간 동기화)
```

- **Unity → WinForms**: Unity에서 조작 시 REST API 호출 → 서버가 WebSocket emit → WinForms 실시간 갱신
- **WinForms → Unity/UE5**: WinForms에서 조작 시 REST API 호출 → 서버가 WebSocket으로 push → 즉시 갱신
- **UE5 ↔ 서버**: WebSocket 양방향 실시간 동기화

> Unity/UE5에서 MySQL을 직접 연결하지 않고 중간에 API 서버를 두는 방식 채택
> 이유는 아래 트러블슈팅 참고

---

## Unreal Engine 5 포팅

### 개요

Unity C# 프로젝트를 UE 5.5.4 C++로 포팅. 동일한 Node.js 서버 + MySQL DB를 공유하며 WebSocket으로 실시간 동기화.

### 프로젝트 구조

```
WarehouseSimulatorUE/
├── Source/WarehouseSimulatorUE/
│   ├── Public/
│   │   ├── Network/
│   │   │   ├── WarehouseHttpManager.h      # REST API 호출
│   │   │   └── WarehouseWebSocketManager.h # WebSocket 실시간 동기화
│   │   ├── Warehouse/
│   │   │   ├── PalletSlot.h               # 팔레트 슬롯 상태 및 컨테이너 관리
│   │   │   ├── ShelfActor.h               # 선반 액터
│   │   │   ├── ShelfRowActor.h            # 선반 행 액터 (슬롯 정렬 포함)
│   │   │   └── WarehouseLoader.h          # 게임 시작 시 DB 복원 + WebSocket 동기화
│   │   ├── Player/
│   │   │   └── WarehousePlayerController.h # 마우스 클릭 처리
│   │   └── UI/
│   │       └── WarehousePopupWidget.h      # 팝업 UI (입고/이동/출하)
│   └── Private/ (위와 동일 구조, .cpp 파일)
└── Content/Blueprints/
    ├── BP_WarehousePlayerController        # PlayerController Blueprint
    ├── Input/
    │   ├── IA_Click.uasset                 # Enhanced Input Action (마우스 클릭)
    │   └── IMC_Warehouse.uasset            # Input Mapping Context
    └── UI/
        └── WBP_SlotPopup.uasset            # 팝업 Widget Blueprint
```

### Unity → UE5 클래스 매핑

| Unity C# | UE5 C++ | 역할 |
|---|---|---|
| `PalletSlot.cs` | `UPalletSlot` (UActorComponent) | 팔레트 슬롯 상태 관리 |
| `BoxVisualizer.cs` | `UPalletSlot` (통합) | 컨테이너 오브젝트 생성/제거 |
| `BoxPool.cs` | `ABoxPool` (AActor) | 오브젝트 풀링 |
| `WarehouseUI.cs` | `UWarehousePopupWidget` (UUserWidget) | 팝업 UI |
| `PalletClickHandler.cs` | `AWarehousePlayerController` | 마우스 클릭 처리 |
| `WarehouseLoader.cs` | `UWarehouseLoader` (UActorComponent) | DB 복원 + 동기화 |
| `DatabaseManager.cs` | `UWarehouseHttpManager` | REST API 호출 |
| — | `UWarehouseWebSocketManager` | WebSocket 실시간 동기화 |

### 실행 방법

1. Node.js 서버 실행 (`cd Server && node server.js`)
2. UE 에디터에서 `WarehouseLevel` 열기
3. World Settings → GameMode → `BP_WarehouseGameMode` 확인
4. Play 버튼으로 실행
5. 팔레트 클릭 → 팝업에서 입고/이동/출하

---

## 서버 실행 방법

### 1. 환경 변수 설정 (.env)

서버는 MySQL 접속 정보를 `.env` 파일에서 읽는다. `.env` 파일은 보안상 GitHub에 올리지 않으므로 직접 생성해야 한다.

```bash
cd Server
cp .env.example .env   # Windows: copy .env.example .env
```

`.env` 파일을 열어 비밀번호 입력:
```
DB_HOST=localhost
DB_USER=root
DB_PASSWORD=본인_MySQL_비밀번호
DB_NAME=warehouse_db
PORT=3000
```

> **왜 .env를 쓰는가?**
> 비밀번호를 코드에 직접 적으면 GitHub에 올라가 유출될 수 있다.
> `.env`는 로컬에만 존재하고 `.gitignore`로 업로드를 차단한다.
> 팀원은 `.env.example`을 보고 어떤 값이 필요한지 파악한 뒤 자신의 `.env`를 만든다.
> 이것이 현업에서 표준으로 사용하는 방식이다.

### 2. 서버 실행

```bash
cd Server
node server.js
# 서버 실행 중: http://localhost:3000
```

DB 데이터 확인:
```
브라우저에서 http://localhost:3000/containers 접속
```

DB 데이터 초기화 (MySQL Workbench):
```sql
TRUNCATE TABLE warehouse_db.containers;
```

---

## DB 스키마

```sql
CREATE DATABASE warehouse_db;
USE warehouse_db;

CREATE TABLE containers (
    container_id VARCHAR(20) PRIMARY KEY,
    item_name    VARCHAR(100),
    weight       FLOAT,
    arrival_date DATE,
    shelf        CHAR(1),   -- A/B/C/D
    floor        INT,       -- 0/1/2
    slot         INT,       -- 0~7
    width        FLOAT DEFAULT 1.0,
    depth        FLOAT DEFAULT 1.0,
    height       FLOAT DEFAULT 1.0
);
```

컬럼 추가 (기존 DB 업그레이드 시):
```sql
ALTER TABLE containers
ADD COLUMN width  FLOAT DEFAULT 1.0,
ADD COLUMN depth  FLOAT DEFAULT 1.0,
ADD COLUMN height FLOAT DEFAULT 1.0;
```

---

## 주요 스크립트

| 스크립트 | 위치 | 역할 |
|---|---|---|
| `DatabaseManager.cs` | Scene_Warehouse/Scripts | HTTP API 호출 (입고/이동/출고) |
| `PalletSlot.cs` | Scene_Warehouse/Scripts | 팔레트 슬롯 상태 및 컨테이너 관리 |
| `BoxVisualizer.cs` | Scene_Warehouse/Scripts | 팔레트에 박스 오브젝트 생성/반환 |
| `BoxPool.cs` | Scene_Warehouse/Scripts | PlasticBox_A 오브젝트 풀 관리 |
| `WarehouseUI.cs` | Scene_Warehouse/Scripts | 팝업 UI (입고/이동/출고/정보 표시) |
| `PalletClickHandler.cs` | Scene_Warehouse/Scripts | 마우스 클릭으로 팔레트 선택 |
| `WarehouseLoader.cs` | Scene_Warehouse/Scripts | 게임 시작 시 DB 복원 + 3초 폴링 동기화 |
| `PalletSlotSetup.cs` | Scene_Warehouse/Editor | 에디터 툴: 팔레트 컴포넌트 일괄 설정 |
| `CameraMove.cs` | Player/Scripts | 1인칭 카메라 이동 (WASD + 마우스) |
| `PalletVRInteractable.cs` | Scene_Warehouse/Scripts | VR 카메라 시선 레이캐스트로 팔레트 선택 |

---

## WinForms 앱 실행 방법

Visual Studio 2022에서 `WarehouseWinForms/WarehouseWinForms.slnx` 열고 실행 (서버가 먼저 켜져 있어야 함).

| 기능 | 설명 |
|---|---|
| 입고 | CNT-000 형식 ID, 품목명, 중량, 크기(가로/세로/높이) 입력 후 DB 저장 |
| 이동 | 목록에서 컨테이너 선택 후 이동할 선반/층/슬롯 선택 |
| 출하 | 목록에서 컨테이너 선택 후 확인 |
| 실시간 | socket.io로 Unity 조작 시 WinForms 자동 갱신 |

### 실시간 동기화 구조 (socket.io)

```
[Unity에서 변경]
Unity → REST API 호출 → 서버
                         └→ socket.io emit (containerAdded / containerMoved / containerRemoved)
                               └→ WinForms 수신 → 자동 새로고침

[WinForms에서 변경]
WinForms → REST API 호출 → 서버
                            └→ socket.io emit (동일)
                                  └→ Unity는 3초 폴링으로 반영
```

- **WinForms → Unity**: REST API 호출 후 Unity가 3초 폴링으로 감지
- **Unity → WinForms**: 서버가 socket.io로 push → WinForms 즉시 갱신 (버튼 불필요)
- socket.io는 서버가 클라이언트에게 변경을 능동적으로 알려주는 **push 방식**

### 입력 검증

| 항목 | 규칙 |
|---|---|
| 컨테이너 ID | `CNT-000` ~ `CNT-999` 형식만 허용 |
| 중복 ID | 입고 시 현재 목록에서 중복 체크 후 차단 |
| 목적지 슬롯 | 이동 시 해당 슬롯 점유 여부 체크 후 차단 |
| 박스 크기 | 최대 5 × 5 × 5 m (초과 시 자동 클램프) |

---

## 코드 개념 설명

### async / await

`async/await`는 **시간이 걸리는 작업(네트워크 요청, DB 쿼리 등)을 기다리는 동안 프로그램이 멈추지 않게** 해주는 문법이다.

#### 왜 필요한가?

서버에 HTTP 요청을 보내면 응답이 올 때까지 시간이 걸린다.
`await` 없이 그냥 기다리면 그 사이에 UI가 얼어붙어 사용자가 아무것도 못 한다.

```
일반 방식 (동기):
요청 보냄 → [UI 멈춤 ⏳] → 응답 도착 → 다음 코드 실행

async/await 방식 (비동기):
요청 보냄 → [UI 정상 작동 ✅] → 응답 도착 → 다음 코드 실행
```

#### WinForms에서의 사용 예 (`ApiService.cs`)

```cs
// async : 이 메서드 안에서 await를 쓸 수 있다는 선언
public async Task<List<ContainerModel>> GetAllAsync()
{
    // await : 응답이 올 때까지 기다리되, 그 동안 UI는 살아있음
    var json = await _http.GetStringAsync($"{BASE}/containers");
    return JsonConvert.DeserializeObject<List<ContainerModel>>(json) ?? new();
}
```

- `async`가 붙은 메서드는 반환 타입이 `Task` 또는 `Task<T>`가 된다
- `await`는 `async` 메서드 안에서만 쓸 수 있다
- 호출하는 쪽에서도 `await`를 붙여야 결과를 기다린다

```cs
// Form1.cs
_containers = await _api.GetAllAsync(); // 응답 올 때까지 대기 후 결과 저장
```

#### Node.js에서의 사용 예 (`server.js`)

Node.js도 동일한 개념으로, DB 쿼리가 끝날 때까지 기다릴 때 쓴다.

```js
app.post('/containers', async (req, res) => {
    // await : DB 쿼리가 완료될 때까지 대기
    await db.query('INSERT INTO containers ...', [...]);
    res.json({ success: true });
});
```

#### Unity에서의 차이점

Unity는 `async/await` 대신 **코루틴(IEnumerator + yield return)** 을 써서 같은 효과를 낸다.

```cs
// Unity 방식 (코루틴)
yield return req.SendWebRequest(); // 응답 올 때까지 대기 (게임은 계속 실행)

// C# 일반 방식 (async/await)
var result = await httpClient.GetAsync(url);
```

---

## 개발 일지

### 2026-03-29 — VR 인터랙션 입력 처리 개선

**추가된 기능**

- `PalletVRInteractable.cs` 신규 작성: VR 모드에서 카메라 시선 방향 레이캐스트로 팔레트 선택
- OpenXR 패키지 (`com.unity.xr.openxr` 1.14.3) 및 XR Interaction Toolkit 2.6.5 추가
- XR Interaction Toolkit Starter Assets / XR Device Simulator 샘플 추가
- `OutdoorsScene`에 XR Origin (XR Rig) 구성

**버그 수정**

- 팝업이 열린 상태에서 입력 필드 클릭 시 팔레트 레이캐스트가 발동되어 기입력 내용이 초기화되는 버그 수정
  - `PalletClickHandler` / `PalletVRInteractable`에 `EventSystem.IsPointerOverGameObject()` 체크 추가 → UI 위 클릭 시 레이캐스트 차단
  - `WarehouseUI`에 `_blockPalletClick` 플래그 추가 — 팝업 오픈 시 팔레트 클릭 차단, 이동 모드(`OnMove`) 진입 시 해제하여 목적지 팔레트 선택 허용

---

### 2026-03-20 — 버그 수정: 컨테이너 이동 시 유니티 오브젝트 중복 생성

**현상**

WinForms에서 컨테이너를 A-0-0 → B-0-0으로 이동하면 Unity 씬에서 A-0-0과 B-0-0 양쪽에 동시에 박스가 존재하는 문제 발생.

**원인**

`WarehouseLoader.OnRefresh()`의 슬롯 비우기 조건이 **컨테이너 ID 기반**이었음.

```cs
// 버그 코드: ID가 DB에 있으면 위치 상관없이 슬롯을 안 비움
var dbIds = new HashSet<string>();
if (!slot.IsEmpty && !dbIds.Contains(slot.container.containerId))
    slot.ClearContainer();
```

CNT-001이 B-0-0으로 이동해도 `dbIds`에 CNT-001이 존재하기 때문에, A-0-0 슬롯이 비워지지 않고 그대로 남아 있었음.

**수정**

비교 기준을 **컨테이너 ID → 슬롯 키(shelf_floor_slot)** 로 변경.

```cs
// 수정 코드: DB에서 점유 중인 슬롯 키 목록에 없으면 비움
var dbSlotKeys = new HashSet<string>();
if (!kvp.Value.IsEmpty && !dbSlotKeys.Contains(kvp.Key))
    kvp.Value.ClearContainer();
```

A-0-0은 DB 점유 슬롯 목록에 없으므로 정상적으로 비워짐.

---

### 2026-03-18 — WinForms 대시보드 재작성 (Visual Studio 2022)

**변경 사항**

- **WinForms 프로젝트 재작성**: 기존 dotnet CLI 버전 삭제 후 Visual Studio 2022로 재작성
  - VS 디자이너로 UI 직접 설계 (Form1, IncomingForm, MoveForm)
  - `TableLayoutPanel` 기반 레이아웃으로 입고/이동 다이얼로그 구성
- **입력 검증 강화**
  - 중복 ID 입고 차단 (현재 목록 기반 체크)
  - 이동 시 목적지 슬롯 점유 여부 체크
- **실시간 동기화**: socket.io 클라이언트 연결, 서버 이벤트 수신 시 자동 새로고침
- **.gitignore**: WinForms `bin/`, `obj/` 빌드 산출물 제외 추가

---

### 2026-03-17 — WinForms 대시보드 & 실시간 동기화

**추가된 기능**

- **WinForms 앱** (`WinFormsApp/`): .NET 8 WinForms로 창고 관리 데스크탑 앱 구현
  - DataGridView로 전체 컨테이너 목록 표시
  - 입고 / 이동 / 출하 기능 (REST API 호출)
  - socket.io 클라이언트로 실시간 반영 (Unity 조작 시 즉시 갱신)
- **Unity 폴링**: `WarehouseLoader`에 3초 주기 폴링 추가 → WinForms 조작 내용 자동 반영
- **PalletSlot.ClearContainer()**: 폴링 동기화 시 DB에 없는 컨테이너 슬롯 초기화

---

### 2026-03-17 — DB 동기화 (게임 시작 시 컨테이너 자동 복원)

**추가된 기능**

- **WarehouseLoader**: 게임 시작 시 DB에서 전체 컨테이너 데이터를 조회해 해당 팔레트 슬롯에 자동 복원
- **DB 동기화 흐름**: `GET /containers` → shelf/floor/slot 키로 PalletSlot 매핑 → `LoadContainer()` 호출 (DB 재삽입 없음)
- **PalletSlot.LoadContainer()**: 게임 시작 시 전용 복원 메서드 추가 (InsertContainer 호출 안 함)
- **DatabaseManager.LoadAllContainers()**: JSON 배열 파싱 후 ContainerData 배열로 반환

**버그 수정**

- `Pallet` 프리팹 자식에 PalletSlot이 중복으로 붙어 `GetComponentInParent`가 잘못된 슬롯을 반환하던 문제 수정
- `PalletClickHandler`를 shelf 정보가 있는 PalletSlot을 찾을 때까지 부모 방향으로 탐색하도록 개선

**씬 설정**

- 빈 오브젝트 `WarehouseLoader` 생성 후 `WarehouseLoader` 컴포넌트 추가

---

### 2026-03-16 — 박스 시각화 & UI 개선

**추가된 기능**

- **박스 시각화**: 입고 시 `PlasticBox_A` 프리팹을 팔레트 슬롯 위에 생성, 출고/이동 시 제거
- **오브젝트 풀링**: `BoxPool`로 PlasticBox_A를 미리 96개 생성해두고 재사용 (상세 내용은 아래 참고)
- **크기 데이터**: 컨테이너에 가로(width) / 세로(depth) / 높이(height) 필드 추가, DB 반영
- **InfoPanel 크기 표시**: 컨테이너 정보 팝업에 `크기: W × D × H m` 표시

**입력 검증**

| 항목 | 규칙 |
|---|---|
| 컨테이너 ID | `CNT-000` ~ `CNT-999` 형식만 허용 |
| 중복 ID | 씬 내 동일 ID 존재 시 입고 차단 |
| 박스 크기 | 최대 5 × 5 × 5 m (초과 시 자동 클램프) |
| 무게 / 크기 입력 | 숫자(소수점)만 입력 가능 |

**UI/조작 개선**

- 팝업 열릴 때 `CameraMove` / `PalletClickHandler` 자동 비활성화 → 키보드 입력 시 플레이어 이동 방지, 팔레트 오클릭 방지
- 크기 입력 필드 플레이스홀더 표시 (기본값 1 자동 입력 제거)

---

## 오브젝트 풀링 (BoxPool)

### 도입 배경

입고/출고/이동이 발생할 때마다 `Instantiate` / `Destroy`로 박스 오브젝트를 생성·삭제하면 다음 문제가 생긴다:

- **GC 스파이크**: Destroy 시 가비지가 누적되어 프레임 드랍 발생 가능
- **반복 로딩**: 매번 프리팹을 새로 인스턴스화하는 비용 발생

### 동작 방식

```
씬 시작 시 PlasticBox_A × 96개 미리 생성 → 비활성화 상태로 BoxPool_Root 하위에 대기

입고 시  → BoxPool.Get()   : 풀에서 꺼내 활성화 → 팔레트 하위에 배치
출고/이동 시 → BoxPool.Return() : 비활성화 → BoxPool_Root 하위로 이동 → 풀에 반환
```

### 풀 크기 기준

| 항목 | 값 |
|---|---|
| 기본 풀 크기 | 96개 |
| 계산 기준 | Shelf 4열 × Floor 3층 × Slot 8개 = 96 (최대 동시 박스 수) |
| 풀 부족 시 | 자동으로 새 오브젝트 생성 후 반환 |

### 씬 설정 방법

1. 빈 오브젝트 생성 → 이름 `BoxPool`
2. `BoxPool` 컴포넌트 추가
3. Inspector에서 **Box Prefab** 슬롯에 `PlasticBox_A` 프리팹 연결

---

## 트러블슈팅

### Unity + MySQL 직접 연동 실패 (2026-03-15)

**시도한 방법**

Unity에서 `MySql.Data.dll` (NuGet 패키지 v9.6.0)을 `Assets/Plugins/` 폴더에 추가해 직접 MySQL에 연결하려 했음.

**발생한 에러**

```
Assembly 'Assets/Plugins/MySql.Data.dll' will not be loaded due to errors:
Unable to resolve reference 'Google.Protobuf'.
Unable to resolve reference 'ZstdSharp'.
Unable to resolve reference 'K4os.Compression.LZ4.Streams'.
Unable to resolve reference 'BouncyCastle.Cryptography'.
Unable to resolve reference 'System.Security.Permissions'.
Unable to resolve reference 'System.Configuration.ConfigurationManager'.
```

**원인**

`MySql.Data.dll`은 단독으로 동작하지 않고 아래 DLL들에 의존함:

| 의존 패키지 | 용도 |
|---|---|
| Google.Protobuf | 데이터 직렬화 |
| ZstdSharp | 데이터 압축 |
| K4os.Compression.LZ4 / LZ4.Streams | 데이터 압축 |
| BouncyCastle.Cryptography | 암호화 |
| System.Security.Permissions | 보안 처리 |
| System.Configuration.ConfigurationManager | 설정 관리 |

일반 C# 프로젝트(Visual Studio)에서는 NuGet이 이 의존성들을 자동으로 처리하지만,
**Unity는 NuGet을 기본 지원하지 않기 때문에** 필요한 DLL을 개발자가 직접 모두 `Plugins` 폴더에 추가해야 함.

**결론 및 대안**

DLL을 7개 이상 수동으로 관리하는 것은 유지보수가 어렵고, 버전 충돌 위험이 있어
**중간 API 서버를 두는 방식(REST API)으로 전환.**

- Unity → HTTP 요청 → API 서버 → MySQL
- 추후 웹 대시보드(목표 3)와 동일한 API 서버를 공유할 수 있어 구조적으로도 유리함

---

## 추후 개선 사항

### VR 컨트롤러 레이 인터랙션

현재는 카메라 시선 방향으로 레이를 쏘는 방식을 사용하고 있어 조작이 불편하다.

**현재 방식 (카메라 시선)**
```
카메라 중앙 → 앞방향으로 레이 → 팔레트 히트
→ 고개를 팔레트 쪽으로 정확히 향해야 선택 가능 (불편)
```

**개선 방향 (VR 컨트롤러 레이)**
```
오른손 컨트롤러 → 레이 → 팔레트 히트 → 트리거 당기면 선택
→ 손을 팔레트 방향으로 가리키기만 하면 됨 (자연스러움)
```

XR Interaction Toolkit의 `XR Ray Interactor` 컴포넌트가 이를 지원한다.
실제 VR 기기(Quest 등) 연결 시 `PalletVRInteractable` 대신 XRI의 Select 이벤트를 팔레트 슬롯에 연결하는 방식으로 전환 예정.
