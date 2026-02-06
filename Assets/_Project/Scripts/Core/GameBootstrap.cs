// 게임 시작 시 서비스/매니저/UI를 정해진 순서로 초기화하는 진입점입니다.
using System.Collections.Generic;
using Project.Data;
using Project.Gameplay;
using Project.UI;
using UnityEngine;

namespace Project.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Data Sources")]
        [SerializeField] private List<StageDefinitionSO> stageDefinitions = new List<StageDefinitionSO>();
        [SerializeField] private List<SkillData> skillPool = new List<SkillData>();

        private bool _initialized;
        private ServiceRegistry _services;
        private UIRoot _uiRoot;

        private void Awake()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }

        private void Initialize()
        {
            // 1) 서비스 레지스트리 생성
            _services = new ServiceRegistry();

            // 2) 인증/저장소 초기화
            var authService = new AuthService();
            var accountRepository = new AccountRepository();
            _services.Register(authService);
            _services.Register(accountRepository);

            // 3) 게임 상태 초기화
            var gameState = new GameState();
            var sceneFlow = new SceneFlow();
            _services.Register(gameState);
            _services.Register(sceneFlow);

            // 4) 게임플레이 매니저 초기화
            var mapManager = new MapManager(stageDefinitions);
            var battleManager = new BattleManager(this);
            var stageManager = new StageManager(battleManager);
            var dayManager = new DayManager(this, gameState, accountRepository, mapManager, stageManager, battleManager, skillPool);
            _services.Register(mapManager);
            _services.Register(battleManager);
            _services.Register(stageManager);
            _services.Register(dayManager);

            // 5) UI 초기화 후 로그인 화면 진입
            _uiRoot = new UIRoot(this, sceneFlow, authService, accountRepository, gameState, dayManager, mapManager);
            sceneFlow.Enter(ScreenType.Login);
        }

        private void OnDestroy()
        {
            _uiRoot?.Dispose();
        }
    }
}
