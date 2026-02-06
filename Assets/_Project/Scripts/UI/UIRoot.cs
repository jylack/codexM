// 런타임 Canvas/UI 패널을 생성하고 화면 전환 및 UI-코어 연결을 담당합니다.
using Project.Core;
using Project.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class UIRoot
    {
        private readonly SceneFlow _sceneFlow;
        private readonly AuthService _authService;
        private readonly AccountRepository _accountRepository;
        private readonly GameState _gameState;
        private readonly DayManager _dayManager;
        private readonly MapManager _mapManager;

        private readonly LoginPanelController _loginPanel;
        private readonly GameObject _canvasRoot;
        private readonly RoomPanelController _roomPanel;
        private readonly InGamePanelController _inGamePanel;

        public UIRoot(GameBootstrap bootstrap, SceneFlow sceneFlow, AuthService authService, AccountRepository accountRepository, GameState gameState, DayManager dayManager, MapManager mapManager)
        {
            _sceneFlow = sceneFlow;
            _authService = authService;
            _accountRepository = accountRepository;
            _gameState = gameState;
            _dayManager = dayManager;
            _mapManager = mapManager;

            // 런타임에서 Canvas/EventSystem을 생성하고 패널을 구성
            _canvasRoot = BuildCanvas();
            _loginPanel = BuildLoginPanel(_canvasRoot.transform);
            _loginPanel.Initialize(_authService, _accountRepository, _gameState, _sceneFlow);
            _roomPanel = new RoomPanelController(_canvasRoot.transform, HandleStartStage, _mapManager, _gameState);
            _inGamePanel = new InGamePanelController(_canvasRoot.transform, _dayManager);

            _sceneFlow.OnScreenChanged += OnScreenChanged;
            _dayManager.OnRunEnded += OnRunEnded;
        }

        public void Dispose()
        {
            _sceneFlow.OnScreenChanged -= OnScreenChanged;
            _dayManager.OnRunEnded -= OnRunEnded;
            _inGamePanel.Dispose();
        }



        private LoginPanelController BuildLoginPanel(Transform parent)
        {
            var prefab = Resources.Load<GameObject>("LoginScreen");
            if (prefab != null)
            {
                var instance = Object.Instantiate(prefab, parent, false);
                var controller = instance.GetComponent<LoginPanelController>();
                if (controller != null)
                {
                    return controller;
                }

                Debug.LogWarning("LoginScreen prefab에 LoginPanelController가 없어 런타임 UI를 생성합니다.");
                Object.Destroy(instance);
            }

            var fallback = new GameObject("LoginScreen", typeof(RectTransform), typeof(LoginPanelController));
            fallback.transform.SetParent(parent, false);
            var fallbackController = fallback.GetComponent<LoginPanelController>();
            fallbackController.SetupRuntimeFallbackUI();
            return fallbackController;
        }

        private void HandleStartStage(string stageId)
        {
            _sceneFlow.Enter(ScreenType.InGame);
            _dayManager.RequestStartStage(stageId);
        }

        private void OnRunEnded(RunResult result)
        {
            _sceneFlow.Enter(ScreenType.Room);
        }

        private void OnScreenChanged(ScreenType screen)
        {
            _loginPanel.SetActive(screen == ScreenType.Login);
            _roomPanel.SetActive(screen == ScreenType.Room);
            _inGamePanel.SetActive(screen == ScreenType.InGame);

            if (screen == ScreenType.Room)
            {
                _roomPanel.Refresh();
            }
        }

        private static GameObject BuildCanvas()
        {
            var canvasGo = new GameObject("UIRoot_Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            var eventSystem = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
            Object.DontDestroyOnLoad(canvasGo);
            Object.DontDestroyOnLoad(eventSystem);
            return canvasGo;
        }
    }
}
