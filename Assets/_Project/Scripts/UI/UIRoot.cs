// 런타임 Canvas/UI 패널을 생성하고 화면 전환 및 UI-코어 연결을 담당합니다.
using System.Collections.Generic;
using Project.Core;
using Project.Data;
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
            _roomPanel = new RoomPanelController(canvasRoot.transform, HandleStartStage, _mapManager, _gameState);
            _inGamePanel = new InGamePanelController(canvasRoot.transform, _dayManager);

            _sceneFlow.OnScreenChanged += OnScreenChanged;
            _dayManager.OnRunEnded += OnRunEnded;
        }

        public void Dispose()
        {
            _sceneFlow.OnScreenChanged -= OnScreenChanged;
            _dayManager.OnRunEnded -= OnRunEnded;
            _inGamePanel.Dispose();
        }

        private void HandleLogin(string accountId)
        {
            // 로그인 성공 시 계정 저장 데이터 로드
            if (!_authService.Login(accountId))
            {
                return;
            }

            _gameState.SetSaveData(_accountRepository.Load(accountId));
            _sceneFlow.Enter(ScreenType.Room);
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
