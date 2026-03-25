UE5 C++ 클래스 헤더(.h)와 구현(.cpp) 파일 쌍을 생성해줘.

$ARGUMENTS 형식: `<클래스명> <부모클래스> <폴더>`
예) `WarehouseGameMode AGameModeBase Warehouse`
예) `ContainerWidget UUserWidget UI`

규칙:
- 헤더: `Source/WarehouseSimulatorUE/Public/<폴더>/<클래스명>.h`
- 구현: `Source/WarehouseSimulatorUE/Private/<폴더>/<클래스명>.cpp`
- 클래스 접두사 자동 판별: Actor→A, Component→U, Widget→U, 나머지→U
- GENERATED_BODY(), #pragma once, .generated.h 포함
- Copyright 헤더 포함
- 기본 오버라이드 함수(BeginPlay, NativeConstruct 등)는 부모에 맞게 추가

인자가 없으면 클래스명/부모/폴더를 물어봐줘.
