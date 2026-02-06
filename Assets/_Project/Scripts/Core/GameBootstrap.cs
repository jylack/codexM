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
            _services = new ServiceRegistry();

            var authService = new AuthService();
            var accountRepository = new AccountRepository();
            _services.Register(authService);
            _services.Register(accountRepository);

            var gameState = new GameState();
            var sceneFlow = new SceneFlow();
            _services.Register(gameState);
            _services.Register(sceneFlow);

            var mapManager = new MapManager(stageDefinitions);
            var battleManager = new BattleManager(this);
            var stageManager = new StageManager(battleManager);
            var dayManager = new DayManager(this, gameState, accountRepository, mapManager, stageManager, battleManager, skillPool);
            _services.Register(mapManager);
            _services.Register(battleManager);
            _services.Register(stageManager);
            _services.Register(dayManager);

            _uiRoot = new UIRoot(this, sceneFlow, authService, accountRepository, gameState, dayManager, mapManager);
            sceneFlow.Enter(ScreenType.Login);
        }

        private void OnDestroy()
        {
            _uiRoot?.Dispose();
        }
    }
}
