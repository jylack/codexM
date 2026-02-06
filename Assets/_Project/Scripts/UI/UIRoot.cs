// 런타임 Canvas/UI 패널을 생성하고 화면 전환 및 UI-코어 연결을 담당합니다.
using Project.Core;
using Project.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
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
            var canvasRoot = BuildCanvas();
            _loginPanel = new LoginPanelController(canvasRoot.transform, HandleLogin);
            _roomPanel = new RoomPanelController(canvasRoot.transform, HandleStartStage, HandleLogout, _mapManager, _gameState);
            _inGamePanel = new InGamePanelController(canvasRoot.transform, _dayManager);

            _sceneFlow.OnScreenChanged += OnScreenChanged;
        }

        public void Dispose()
        {
            _sceneFlow.OnScreenChanged -= OnScreenChanged;
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

        private void HandleLogout()
        {
            _authService.Logout();
            _sceneFlow.Enter(ScreenType.Login);
        }

        private void OnRunEnded(RunResult result)
        {
            _sceneFlow.Enter(ScreenType.Room);
            _roomPanel.Refresh();
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

        private static GameObject ResolveCanvas()
        {
            var existingCanvas = GameObject.Find("UIRoot/Canvas");
            if (existingCanvas != null)
            {
                EnsureEventSystem();
                return existingCanvas;
            }

            var root = new GameObject("UIRoot");
            var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(root.transform, false);

            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            EnsureEventSystem();
            Object.DontDestroyOnLoad(root);
            return canvasGo;
        }

        private static Transform InstantiateScreenRoot(Transform parent, string screenName)
        {
            var prefab = Resources.Load<GameObject>($"UI/{screenName}");
            if (prefab != null)
            {
                return Object.Instantiate(prefab, parent, false).transform;
            }

            var fallback = new GameObject(screenName, typeof(RectTransform));
            fallback.transform.SetParent(parent, false);
            return fallback.transform;
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<EventSystem>() != null)
            {
                return;
            }

            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            Object.DontDestroyOnLoad(eventSystem);
        }
    }
}
