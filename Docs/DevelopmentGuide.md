# 개발 가이드

## 기준 환경

| 항목 | 설정 |
| --- | --- |
| 프로젝트 이름 | The World |
| 에디터 | Unity 6000.3.11f1 |
| 렌더링 | Universal Render Pipeline 17.3.0 |
| 색 공간 | Linear |
| 입력 | Input System Package, 새 Input System 사용 |
| 기본 씬 | `Assets/_Project/Scenes/Main.unity` |
| 에셋 직렬화 | Force Text |
| 버전 관리 | Visible Meta Files |
| 코드 루트 네임스페이스 | `TheWorld` |
| 제품 버전 | 0.1.0 |
| 기본 회사명 / 식별자 | TheWorld / com.theworld.game |

회사명과 애플리케이션 식별자는 초기 개발용 값이다. 배포 전에 **Project Settings > Player**에서 실제 값으로 변경한다. 에디터 버전은 `ProjectSettings/ProjectVersion.txt`, 패키지 버전은 `Packages/manifest.json`과 `packages-lock.json`을 기준으로 맞춘다.

## 시작 절차

1. Unity Hub에 프로젝트 루트 `theWorld`를 추가하고 기준 버전으로 연다.
2. 최초 실행에는 패키지 다운로드를 위한 네트워크 연결이 필요할 수 있다. 이후 `Library` 캐시를 재사용한다.
3. **The World > Project > Open Main Scene**으로 기본 씬을 연다.
4. **Play**로 기본 렌더링을 확인하고 씬과 프리팹을 편집한다.
5. 씬을 추가하면 **File > Build Profiles > Scene List**에도 등록한다.

## 코드와 에셋 규칙

- C# 파일명은 주요 클래스명과 맞추고 PascalCase를 사용한다. 예: `PlayerController.cs`.
- 게임 코드는 `TheWorld`, `TheWorld.Gameplay`, `TheWorld.UI` 등의 네임스페이스를 사용한다.
- 에디터 전용 코드는 `Scripts/Editor`에 둔다. `TheWorld.Editor.asmdef`가 Editor 플랫폼으로 제한하므로 플레이어 빌드에 포함되지 않는다.
- 런타임 코드는 `UnityEditor`를 참조하지 않는다. 런타임 폴더는 비어 있으며 기능이 생길 때 코드를 추가한다.
- 프리팹은 `PF_`, 머티리얼은 `M_`, 텍스처는 `T_`, ScriptableObject 에셋은 `SO_` 접두사를 사용한다. 씬과 C# 파일에는 접두사를 강제하지 않는다.
- 폴더와 파일 이름은 영문으로 작성하고 공백을 피한다. 씬의 GameObject 이름은 읽기 쉬운 이름을 사용한다.
- 모델 크기는 1 Unity unit = 1m 기준으로 맞춘다. 프리팹 루트의 기본 Scale은 `(1, 1, 1)`을 권장한다.
- 씬 배치물은 Environment, Gameplay, Lighting처럼 역할별 부모 아래에 둔다.
- 머티리얼은 URP 호환 Shader를 사용한다. 일반 3D 오브젝트의 시작점은 `Universal Render Pipeline/Lit`이다.
- 패키지 추가와 업데이트는 Package Manager에서 수행하고 `manifest.json`, `packages-lock.json` 변경을 함께 보관한다.

## 포함된 패키지와 설정

| 패키지 | 버전 | 용도 |
| --- | --- | --- |
| `com.unity.render-pipelines.universal` | 17.3.0 | 3D 렌더링, Shader Graph 등 관련 의존성 포함 |
| `com.unity.inputsystem` | 1.19.0 | 새 입력 시스템 |
| `com.unity.ugui` | 2.0.0 | Canvas UI와 TextMeshPro 관련 기능 |
| `com.unity.ide.visualstudio` | 2.0.26 | Visual Studio 연동 |

물리, 오디오, 애니메이션, 지형, UI 등에 필요한 기본 Unity 모듈도 선언했다. `PlayerControls.inputactions`는 Unity 3D 템플릿의 Player/UI 액션 구성을 보관한다. 실제 캐릭터 조작 코드는 아직 연결하지 않았다.

렌더 설정은 설치된 에디터의 3D URP 템플릿에서 가져왔다. `PC_RPAsset`과 `Mobile_RPAsset`은 각 Renderer를 참조하며 Quality 설정에 연결돼 있다. `MainVolumeProfile`은 씬의 Global Volume이 사용한다. `DefaultVolumeProfile`과 URP Global Settings는 전역 기본 설정에 사용한다. 목적이 정해지면 대상 플랫폼에 맞춰 Quality와 URP 옵션을 조정한다.

## 프로젝트 검증

에디터 메뉴 **The World > Project > Validate** 또는 에디터를 닫은 상태에서 다음 명령을 사용한다.

```powershell
.\Tools\Unity.ps1 -Action Validate
```

설치 경로가 다르면 `-EditorPath`를 지정한다.

```powershell
.\Tools\Unity.ps1 -Action Validate -EditorPath 'D:\Unity\6000.3.11f1\Editor\Unity.exe'
```

배치 검증은 실제 Unity를 실행해 임포트와 컴파일을 수행한다. 이후 URP, Linear 색 공간, Force Text, 기본 씬과 빌드 등록, 카메라와 AudioListener 수, 조명, 바닥 Collider, 누락된 스크립트와 머티리얼을 검사한다. 실패하면 프로세스 종료 코드와 `Logs/ProjectValidation.log`를 확인한다. 초기 씬을 크게 바꾼 뒤에는 `ProjectSetup.ValidateProject`의 검사 조건도 게임 구성에 맞게 변경한다.

이 검사는 시작 프로젝트 구성 확인용이다. 게임 기능 테스트, 실제 GPU 화면 확인, 플랫폼별 플레이어 빌드 검증은 별도로 수행한다.

`ProjectSetup.Initialize`는 최초 구성에 사용한 배치 진입점이다. 기존 Main 씬과 프리팹, 머티리얼을 덮어쓰지 않지만 Player 설정을 초기값으로 적용하므로 일상적인 검증에는 `Validate`를 사용한다.

## 버전 관리

Unity 에셋은 `.meta`와 함께 보관한다. `Library`를 복사하거나 커밋할 필요는 없다. 새 작업 환경에서는 Unity가 재생성한다. 빈 폴더의 `.gitkeep`은 실제 에셋이 들어오면 제거해도 된다.

`.gitignore`는 캐시, 개인 에디터 설정, IDE 생성물, 로그와 빌드 결과를 제외한다. `.gitattributes`는 텍스트와 바이너리를 구분하고 `.editorconfig`는 코드 서식을 맞춘다.

Git 원격 저장소는 [seungmin383/theWorld](https://github.com/seungmin383/theWorld)이며, `origin`으로 연결한다. 기본 브랜치는 `main`이다.
