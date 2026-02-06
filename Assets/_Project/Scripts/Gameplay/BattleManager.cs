using System;
using System.Collections;
using Project.Data;
using UnityEngine;

namespace Project.Gameplay
{
    public class BattleManager
    {
        private readonly MonoBehaviour _runner;

        public event Action<string> OnBattleLog;
        public event Action<StatsSnapshot> OnStatsChanged;

        public BattleManager(MonoBehaviour runner)
        {
            _runner = runner;
        }

        public IEnumerator RunSimpleBattle(RunContext runContext)
        {
            var playerHp = runContext.playerCurrentStats.hp;
            var enemyHp = 35 + (runContext.currentDay * 5);
            var playerFirst = runContext.playerCurrentStats.speed >= 10 || UnityEngine.Random.Range(0, 2) == 0;

            OnBattleLog?.Invoke($"Battle Start - Day {runContext.currentDay}");
            yield return null;

            while (playerHp > 0 && enemyHp > 0)
            {
                if (playerFirst)
                {
                    enemyHp -= Mathf.Max(1, runContext.playerCurrentStats.attack - 2);
                    OnBattleLog?.Invoke($"Player hits. Enemy HP: {Mathf.Max(0, enemyHp)}");
                }
                else
                {
                    playerHp -= Mathf.Max(1, 8 - runContext.playerCurrentStats.defense / 2);
                    OnBattleLog?.Invoke($"Enemy hits. Player HP: {Mathf.Max(0, playerHp)}");
                }

                playerFirst = !playerFirst;
                runContext.playerCurrentStats.hp = Mathf.Max(1, playerHp);
                OnStatsChanged?.Invoke(StatsSnapshot.From(runContext.playerCurrentStats, runContext.playerCurrentStats.hp));
                yield return null;
            }

            OnBattleLog?.Invoke(playerHp > 0 ? "Battle Won" : "Battle Lost (prototype keeps run alive)");
            runContext.playerCurrentStats.hp = Mathf.Max(1, playerHp);
        }
    }
}
