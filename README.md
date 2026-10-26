# The World

Unity **6000.3.11f1** / **3D URP 17.3.0** 기반의 시작 프로젝트다. 현재 `theWorld` 폴더 자체가 Unity 프로젝트 루트다.

스스로 판단하며 살아가는 도시의 NPC 주민을 생성하고 행동을 지정하는 인간 시뮬레이션 게임을 기획 중이다. 확정된 구상과 미정 사항은 [게임 기획](Docs/GameDesign.md)에 정리한다.

## 열고 실행하기

1. Unity Hub에서 **Add > Add project from disk**로 이 폴더를 등록한다.
2. Unity **6000.3.11f1**로 열고 패키지 설치와 임포트가 끝날 때까지 기다린다.
3. `Assets/_Project/Scenes/Main.unity`를 열고 **Play**를 누른다.

기본 씬에는 카메라, 태양광, 전역 볼륨, 20×20 크기의 바닥, 파란 큐브 프리팹이 있다. 바닥과 큐브에는 Collider가 있다. 아직 이동 조작이나 게임 규칙은 없는 3D 작업용 출발점이다.

PowerShell에서도 에디터를 열 수 있다.

```powershell
.\Tools\Unity.ps1 -Action Open
```

## 주요 위치

| 위치 | 용도 |
| --- | --- |
| `Assets/_Project/` | 직접 만드는 게임 에셋, 코드, 씬, 설정 |
| `Assets/ThirdParty/` | 외부 제작 에셋과 플러그인 |
| `Packages/` | 패키지 목록과 버전 잠금 파일 |
| `ProjectSettings/` | Unity 공통 프로젝트 설정 |
| `Docs/` | 구조와 개발 규칙 문서 |
| `Tools/` | 에디터 실행 및 검증 도구 |

전체 트리와 배치 예시는 [프로젝트 폴더 구조](Docs/ProjectStructure.md), 작업 규칙과 패키지 설명은 [개발 가이드](Docs/DevelopmentGuide.md)를 참고한다.

## 검증하기

프로젝트를 연 Unity 에디터를 닫고 실행한다.

```powershell
.\Tools\Unity.ps1 -Action Validate
```

Unity가 패키지를 로드하고 스크립트를 컴파일한 뒤 기본 씬 구성과 URP 설정을 검사한다. 결과 로그는 `Logs/ProjectValidation.log`에 저장한다. 에디터 안에서는 **The World > Project > Validate** 메뉴를 사용할 수 있다.

`Assets`, `Packages`, `ProjectSettings`, `Docs`, `Tools`와 루트 설정 파일을 버전 관리한다. Unity의 `.meta`도 함께 보관한다. `Library`, `Temp`, `Logs`, `UserSettings`, 빌드 결과는 `.gitignore`로 제외했다.
