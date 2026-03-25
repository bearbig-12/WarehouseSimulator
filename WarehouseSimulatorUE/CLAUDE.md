# 프로젝트 규칙

## 편집 동작
- 파일 편집 시 확인 프롬프트 없이 자동 적용 (acceptEdits)
- 불필요한 권한 질문 생략 (dontAsk)

## 수정 범위
- 요청한 변경만 최소한으로 수정
- 요청하지 않은 파일은 건드리지 않음
- 리팩토링, 주석 추가, 코드 정리 등 부가 작업 하지 않음

## 프로젝트 개요
- Unreal Engine 5.5.4 C++ 창고 시뮬레이터
- Unity C# → UE5 C++ 포팅 프로젝트
- 주요 경로:
  - 헤더: `Source/WarehouseSimulatorUE/Public/`
  - 구현: `Source/WarehouseSimulatorUE/Private/`

## 코드 규칙
- `int32 SlotIndex` 사용 (UWidget::Slot 충돌 방지)
- `FContainerData::SlotIndex` 사용
- UUserWidget 파라미터명으로 `Slot` 사용 금지 → `InSlot` 사용
