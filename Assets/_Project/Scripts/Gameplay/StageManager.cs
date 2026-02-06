// DayDefinition의 이벤트 타입에 따라 해당 일차 이벤트를 실행합니다.
using System.Collections;
using Project.Data;
using UnityEngine;

namespace Project.Gameplay
{
    public class StageManager
    {
        private readonly BattleManager _battleManager;

        public StageManager(BattleManager battleManager)
        {
            _battleManager = battleManager;
        }

        public IEnumerator ExecuteDayEvent(DayDefinition day, RunContext runContext)
        {
            switch (day.dayEventType)
            {
                case DayEventType.None:
                    yield break;
                case DayEventType.Battle:
                case DayEventType.Boss:
                    yield return _battleManager.RunSimpleBattle(runContext);
                    yield break;
                case DayEventType.Rest:
                    var heal = day.healFlat + Mathf.RoundToInt(runContext.playerBaseStats.hp * day.healPercent);
                    runContext.playerCurrentStats.hp = Mathf.Min(runContext.playerBaseStats.hp, runContext.playerCurrentStats.hp + heal);
                    yield break;
                case DayEventType.CurrencyEvent:
                    yield break;
                case DayEventType.Bonus:
                    runContext.playerCurrentStats.attack += 1;
                    yield break;
            }
        }
    }
}
