# Warehouse Simulator

Unity HDRP 기반 창고 시뮬레이터 프로젝트

---

## 프로젝트 구조

```
WarehouseSimulator/
└── Warehouse/          # Unity 프로젝트 (HDRP, Unity 2022.3.62f1)
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
Unity (씬)
    └─ PalletSlot 컴포넌트 (각 팔레트)
         ↓ 입고 / 이동 / 출고 이벤트
    HTTP 요청 (UnityWebRequest)
         ↓
    API 서버 (Python / Node.js 예정)
         ↓
    MySQL DB (warehouse_db)
```

> Unity에서 MySQL을 직접 연결하지 않고 중간에 API 서버를 두는 방식 채택
> 이유는 아래 트러블슈팅 참고

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
    slot         INT        -- 0~7
);
```

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
