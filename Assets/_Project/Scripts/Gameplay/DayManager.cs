// 스테이지 입장부터 일차 루프, 보상 처리, 클리어 저장까지 핵심 흐름을 담당합니다.
using System;
using System.Collections;
using System.Collections.Generic;
using Project.Core;
using Project.Data;
using UnityEngine;

namespace Project.Gameplay
{
    public class DayManager
    {
        private readonly MonoBehaviour _runner;
        private readonly GameState _gameState;
        private readonly AccountRepository _accountRepository;
        private readonly MapManager _mapManager;
        private readonly StageManager _stageManager;
        private readonly List<SkillData> _skillPool;

        private string _pendingSkillChoice;
        private bool _requestExit;

        public event Action<StageInfo> OnStageEntered;
        public event Action<int> OnDayChanged;
        public event Action<string> OnBattleLog;
        public event Action<StatsSnapshot> OnStatsChanged;
        public event Action<List<SkillData>> OnSkillChoiceOffered;
        public event Action<RunResult> OnRunEnded;

        public DayManager(
            MonoBehaviour runner,
            GameState gameState,
            AccountRepository accountRepository,
            MapManager mapManager,
            StageManager stageManager,
            BattleManager battleManager,
            List<SkillData> skillPool)
        {
            _runner = runner;
            _gameState = gameState;
            _accountRepository = accountRepository;
            _mapManager = mapManager;
            _stageManager = stageManager;
            _skillPool = skillPool;

            battleManager.OnBattleLog += log => OnBattleLog?.Invoke(log);
            battleManager.OnStatsChanged += snapshot => OnStatsChanged?.Invoke(snapshot);
        }

        public void RequestStartStage(string stageId)
        {
            _runner.StartCoroutine(RunStage(stageId));
        }

        public void SubmitSkillChoice(string skillId)
        {
            _pendingSkillChoice = skillId;
        }

        public void RequestExitStage()
        {
            _requestExit = true;
        }

        private IEnumerator RunStage(string stageId)
        {
            _requestExit = false;
            _gameState.StartRun(stageId);
            var run = _gameState.RunContext;
            var stage = _mapManager.GetStage(stageId);
            if (stage == null)
            {
                yield break;
            }

            OnStageEntered?.Invoke(new StageInfo { stageId = stage.stageId, displayName = stage.displayName, totalDays = stage.days.Count });

            // 현재 일차가 총 일차를 넘을 때까지 반복
            while (run.currentDay <= stage.days.Count && !_requestExit)
            {
                var day = stage.days[run.currentDay - 1];
                OnDayChanged?.Invoke(run.currentDay);

                // Day 1 + None 이벤트는 스킵 규칙 적용
                if (!(run.currentDay == 1 && day.dayEventType == DayEventType.None))
                {
                    yield return _stageManager.ExecuteDayEvent(day, run);
                }

                // 매일 이벤트 처리 후 보상 UI는 반드시 수행
                yield return WaitReward(day, run);
                run.currentDay++;
            }

            var cleared = !_requestExit && run.currentDay > stage.days.Count;
            var result = new RunResult { stageCleared = cleared, stageId = stageId, daysCleared = Mathf.Clamp(run.currentDay - 1, 0, stage.days.Count) };
            if (cleared)
            {
                ApplyClearToSave(stageId, result.daysCleared);
            }

            _gameState.EndRun();
            OnRunEnded?.Invoke(result);
        }

        private IEnumerator WaitReward(DayDefinition day, RunContext run)
        {
            _pendingSkillChoice = null;
            // CurrencyEvent는 화폐 보상 UI 흐름으로 처리
            if (day.dayEventType == DayEventType.CurrencyEvent)
            {
                var goldGain = Mathf.Max(10, day.goldFlat);
                _gameState.SaveData.gold += goldGain;
                OnBattleLog?.Invoke($"Currency reward: +{goldGain} gold");
                yield return null;
                yield break;
            }

            // 그 외 이벤트는 스킬 3개 제안 후 1개 선택
            var offer = new List<SkillData>();
            for (var i = 0; i < Mathf.Min(3, _skillPool.Count); i++)
            {
                offer.Add(_skillPool[UnityEngine.Random.Range(0, _skillPool.Count)]);
            }
            OnSkillChoiceOffered?.Invoke(offer);

            while (string.IsNullOrEmpty(_pendingSkillChoice) && !_requestExit)
            {
                yield return null;
            }

            if (!string.IsNullOrEmpty(_pendingSkillChoice))
            {
                run.skillIds.Add(_pendingSkillChoice);
                OnBattleLog?.Invoke($"Selected skill: {_pendingSkillChoice}");
            }
        }

        private void ApplyClearToSave(string stageId, int clearedDays)
        {
            var save = _gameState.SaveData;
            var progress = save.GetOrCreateProgress(stageId);
            progress.clearedDays = Mathf.Max(progress.clearedDays, clearedDays);
            progress.isCleared = true;
            if (!save.clearedStageIds.Contains(stageId))
            {
                save.clearedStageIds.Add(stageId);
            }

            if (int.TryParse(stageId, out var id))
            {
                save.currentStageId = (id + 1).ToString();
            }

            _accountRepository.Save(save);
        }
    }
}
