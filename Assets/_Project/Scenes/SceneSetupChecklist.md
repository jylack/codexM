# Scene Setup Checklist

## 1) Scene별 GameObject / 컴포넌트 배치

### LoginScene
- `GameBootstrap` GameObject 1개 배치
  - `Project.Core.GameBootstrap`
- (선택) 기존 UI를 유지하는 경우 `Canvas`, `EventSystem`을 씬에 유지
- (신규 런타임 UI 사용) 씬에는 루트만 두고 `UIRoot`가 런타임 생성

### RoomScene
- `GameBootstrap` GameObject 1개 배치
  - `Project.Core.GameBootstrap`
- `UIRoot`(또는 UIRoot 프리팹 인스턴스) 배치
  - Room 진입 시 3카드(이전/현재/다음) 표시 UI 연결

### InGameScene
- `GameBootstrap` GameObject 1개 배치
  - `Project.Core.GameBootstrap`
- `UIRoot`(또는 UIRoot 프리팹 인스턴스) 배치
  - 로그/스탯/보상 팝업 UI 루트 연결

---

## 2) Canvas / EventSystem 사용 분기

### A. 기존 Canvas/EventSystem을 씬에서 계속 사용하는 경우
- 각 씬의 기존 `Canvas`와 `EventSystem` 유지
- `UIRoot`가 중복 생성하지 않도록 생성 옵션/초기화 경로 점검
- EventSystem은 씬당 활성 1개만 존재하도록 확인

### B. `UIRoot`가 런타임으로 생성/관리하는 경우(권장)
- 씬에 수동 `Canvas`/`EventSystem`를 두지 않음
- `UIRoot` 초기화 시 `Canvas` + `EventSystem` 자동 생성 확인
- 씬 전환 시 중복 인스턴스가 생기지 않도록 singleton/중복 파괴 규칙 확인

---

## 3) 프리팹별 필수 직렬화 필드 체크

아래 필드는 Inspector에서 반드시 할당되어야 한다.

### LoginView 프리팹
- `loginButton` (`Button`)
- `accountIdInput` (`TMP_InputField` 또는 `InputField`)
- `statusText` (`TMP_Text` 또는 `Text`)

### RoomView 프리팹
- `prevCardRoot` (이전 스테이지 카드 루트)
- `currentCardRoot` (현재 스테이지 카드 루트)
- `nextCardRoot` (다음 스테이지 카드 루트)
- `startButton` (`Button`)
- `stageTitleText` (`TMP_Text`/`Text`)
- `stageDescText` (`TMP_Text`/`Text`)

### InGameHUD 프리팹
- `logScrollRect` (`ScrollRect`)
- `logContentRoot` (`RectTransform`)
- `logItemTextPrefab` (`TMP_Text`/`Text` 프리팹)
- `statsText` (`TMP_Text`/`Text`)
- `openRewardPopupButton` (테스트용일 경우)

### RewardPopup 프리팹
- `popupRoot` (팝업 활성/비활성 루트)
- `optionButtonA` (`Button`)
- `optionButtonB` (`Button`)
- `optionButtonC` (`Button`)
- `optionTextA` (`TMP_Text`/`Text`)
- `optionTextB` (`TMP_Text`/`Text`)
- `optionTextC` (`TMP_Text`/`Text`)
- `closeButton` (`Button`, 정책상 허용 시)

> 버튼/텍스트/스크롤/팝업 참조 누락 시 런타임 NRE 발생 가능성이 높으므로, Play 전 `Missing (Object)` 여부를 반드시 확인한다.

---

## 4) `GameBootstrap` ↔ `UIRoot` 연결 순서

권장 초기화 순서:
1. Scene 로드
2. `GameBootstrap.Awake/Start`에서 코어 서비스(데이터/세션/런플로우) 초기화
3. `UIRoot` 인스턴스 확보(씬 배치 또는 런타임 생성)
4. `GameBootstrap`이 `UIRoot`에 Presenter/ViewModel 바인딩
5. 초기 화면 표시
   - 첫 진입: Login
   - 로그인 후: Room
   - Start 후: InGame
6. InGame 종료 시 Room으로 복귀 및 UI 상태 리셋

체크 포인트:
- `UIRoot`가 `GameBootstrap`보다 늦게 준비되는 경우 null 바인딩 방지(초기화 순서 보장)
- 씬 재진입 시 이벤트 중복 구독 방지(구독 해제/재구독 규칙 확인)

---

## 5) 수동 검증 플로우

아래 플로우를 순서대로 통과해야 한다.

1. `Login` 진입
   - accountId 입력 후 Login 성공
2. `Room` 진입
   - 3카드(`[prev,current,next]`) 정상 노출
3. `Start` 클릭
4. `InGame` 진입
   - log/stats 실시간 갱신 확인
5. `Reward Popup` 노출
   - 선택지 버튼/텍스트 정상 표시 및 선택 처리 확인
6. `Run End`
7. `Room` 복귀
   - 다음 런 시작 가능 상태 확인

---

## 6) Build Settings / 데이터 체크

- `Assets/_Project/Scenes/RoomScene.unity`를 Build Settings 첫 씬(또는 진입 정책에 맞는 씬 순서)으로 등록
- ScriptableObjects 기본 경로:
  - `Assets/_Project/ScriptableObjects/Skills`
  - `Assets/_Project/ScriptableObjects/Items`
  - `Assets/_Project/ScriptableObjects/Stages`
  - `Assets/_Project/ScriptableObjects/Monsters`
- 최소 검증 데이터:
  - Stage 1 (2일 이상)
  - Day 1 event = `None`
  - Day 2 event = `Battle` 또는 `CurrencyEvent`
