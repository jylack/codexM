// 룸 패널에서 스테이지 카드 탐색과 시작 요청을 처리합니다.
using System;
using System.Collections.Generic;
using Project.Core;
using Project.Gameplay;
using UnityEngine;

namespace Project.UI
{
    public class RoomPanelController
    {
        private readonly GameObject _panel;
        private readonly UITextRef _status;
        private readonly UITextRef _cards;
        private readonly StageSelectController _stageSelect = new StageSelectController();
        private readonly MapManager _mapManager;
        private readonly GameState _gameState;
        private string _focusedStageId;

        public RoomPanelController(Transform parent, Action<string> onStartStage, MapManager mapManager, GameState gameState)
        {
            _mapManager = mapManager;
            _gameState = gameState;

            _panel = UIFactory.Panel(parent, "RoomPanel");
            _status = UIFactory.Text(_panel.transform, "Room", new Vector2(0, 220));
            _cards = UIFactory.Text(_panel.transform, string.Empty, new Vector2(0, 80));
            UIFactory.Button(_panel.transform, "Prev", new Vector2(-300, 80), PrevStage);
            UIFactory.Button(_panel.transform, "Next", new Vector2(300, 80), NextStage);
            UIFactory.Button(_panel.transform, "Start Stage", new Vector2(0, -120), () => onStartStage?.Invoke(_focusedStageId));
        }

        public void Refresh()
        {
            var save = _gameState.SaveData;
            _focusedStageId = save.currentStageId;
            _status.text = $"Room | Gold: {save.gold} | Current Stage: {save.currentStageId}";
            UpdateCards();
        }

        public void SetActive(bool active) => _panel.SetActive(active);

        private void PrevStage()
        {
            var ids = _mapManager.GetSortedStageIds();
            var index = Mathf.Max(0, ids.IndexOf(_focusedStageId) - 1);
            if (ids.Count > 0) _focusedStageId = ids[index];
            UpdateCards();
        }

        private void NextStage()
        {
            var ids = _mapManager.GetSortedStageIds();
            var index = Mathf.Min(ids.Count - 1, ids.IndexOf(_focusedStageId) + 1);
            if (ids.Count > 0) _focusedStageId = ids[index];
            UpdateCards();
        }

        private void UpdateCards()
        {
            List<string> window = _stageSelect.BuildWindow(_focusedStageId, _mapManager.GetSortedStageIds());
            _cards.text = $"[{window[0]}]  [{window[1]}]  [{window[2]}]";
        }
    }
}
