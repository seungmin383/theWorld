# 프로젝트 폴더 구조

## 전체 트리

```text
theWorld/
├── Assets/
│   ├── _Project/
│   │   ├── Art/
│   │   │   ├── Animations/
│   │   │   ├── Materials/       # M_Ground, M_Prototype
│   │   │   ├── Models/
│   │   │   └── Textures/
│   │   ├── Audio/
│   │   │   ├── Music/
│   │   │   └── SFX/
│   │   ├── Data/
│   │   ├── Prefabs/
│   │   │   ├── Environment/
│   │   │   ├── Gameplay/        # PF_PrototypeCube
│   │   │   └── UI/
│   │   ├── Scenes/              # Main.unity
│   │   ├── Scripts/
│   │   │   ├── Runtime/
│   │   │   │   ├── Core/
│   │   │   │   ├── Gameplay/
│   │   │   │   └── UI/
│   │   │   └── Editor/          # ProjectSetup, TheWorld.Editor.asmdef
│   │   ├── Settings/
│   │   │   ├── Input/           # PlayerControls.inputactions
│   │   │   └── Rendering/      # URP, Renderer, Volume 설정
│   │   └── UI/
│   │       ├── Fonts/
│   │       └── Sprites/
│   └── ThirdParty/
├── Docs/
│   ├── GameDesign.md
│   ├── ProjectStructure.md
│   └── DevelopmentGuide.md
├── Packages/
│   ├── manifest.json
│   └── packages-lock.json
├── ProjectSettings/
├── Tools/
│   └── Unity.ps1
├── .editorconfig
├── .gitattributes
├── .gitignore
└── README.md
```

빈 에셋 폴더에는 `.gitkeep`을 두어 Git에서도 구조를 유지한다. `.meta`는 Unity가 생성하는 에셋 및 폴더 식별 파일이다.

## 에셋을 넣는 기준

| 폴더 | 넣을 항목과 예시 |
| --- | --- |
| `Art/Animations` | AnimationClip, AnimatorController, Avatar |
| `Art/Materials` | Material, 직접 작성한 Shader와 Shader Graph |
| `Art/Models` | FBX 등 게임에 임포트할 3D 모델 |
| `Art/Textures` | 알베도, 노멀, 마스크, 환경 텍스처 |
| `Audio/Music`, `Audio/SFX` | 배경음악과 효과음 |
| `Data` | ScriptableObject 인스턴스, 밸런스 및 콘텐츠 데이터 |
| `Prefabs/Environment` | 지형 조각, 건물, 소품 등 월드 배치물 |
| `Prefabs/Gameplay` | 플레이어, 적, 아이템, 상호작용 오브젝트 |
| `Prefabs/UI` | 패널, 버튼, HUD 프리팹 |
| `Scenes` | 실제 플레이 씬과 작업용 씬 |
| `Scripts/Runtime/Core` | 공통 기반 코드. 실제 필요할 때 추가 |
| `Scripts/Runtime/Gameplay` | 이동, 전투, 상호작용 등 게임 규칙 |
| `Scripts/Runtime/UI` | UI 표시와 입력 처리 |
| `Scripts/Editor` | 에디터 메뉴, 임포터, 검증 도구 |
| `Settings/Input` | Input Actions와 입력 관련 설정 |
| `Settings/Rendering` | URP Pipeline, Renderer, Volume Profile |
| `UI/Fonts`, `UI/Sprites` | 폰트, UI 전용 이미지 |
| `ThirdParty` | 공급자별 하위 폴더로 외부 에셋을 보관 |

UPM으로 설치한 외부 패키지는 `Packages`가 관리한다. 외부 에셋이 특정 경로를 요구하면 그 요구를 우선하고 예외를 이 문서에 기록한다.

## 기본 씬

`Assets/_Project/Scenes/Main.unity`는 빌드 씬 목록 첫 번째에 등록돼 있다.

```text
Main
├── Environment
│   └── Ground             # 20×20 Plane, MeshCollider
├── Gameplay
│   └── Prototype Cube     # PF_PrototypeCube 인스턴스, BoxCollider
├── Lighting
│   ├── Sun               # Directional Light, Soft Shadows
│   └── Global Volume     # MainVolumeProfile 사용
└── Main Camera           # MainCamera 태그, AudioListener
```

프로토타입 큐브는 위치와 재질 확인용이며 Rigidbody나 동작 스크립트는 없다. 게임 내용에 맞게 제거하거나 교체하면 된다.

## 루트 폴더의 관리 범위

`Assets`, `Packages`, `ProjectSettings`는 Unity 프로젝트에 필요한 소스다. `Docs`와 `Tools`는 문서와 개발 도구다. `Library`, `Temp`, `Obj`, `Logs`, `UserSettings`는 Unity가 재생성하는 로컬 데이터다. 빌드 출력은 `Builds/<플랫폼>/`에 두고 Git에서 제외한다.

Unity에서 사용하는 에셋은 Project 창에서 이동하거나 이름을 바꾼다. 파일 탐색기를 사용할 때는 해당 `.meta`를 반드시 함께 옮긴다. 경로를 바꾸면 씬, 프리팹, 도구에 들어 있는 경로 상수도 확인한다.
