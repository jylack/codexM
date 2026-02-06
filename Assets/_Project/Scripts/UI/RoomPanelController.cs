// 룸 패널에서 스테이지 카드 탐색/표시, 시작 요청, 로그아웃 UI 동작을 처리합니다.
using System;
using System.Collections.Generic;
using Project.Core;
using Project.Gameplay;
using UnityEngine;

namespace Project.UI
{
    public class RoomPanelController
    {
        private const string PlaceholderStageId = "---";

        private readonly GameObject _panel;
        private readonly Text _accountIdLabel;
        private readonly Text _goldLabel;
        private readonly Text _currentStageLabel;

        private readonly Text _leftCardLabel;
        private readonly Text _centerCardLabel;
        private readonly Text _rightCardLabel;

        private readonly StageSelectController _stageSelect = new StageSelectController(PlaceholderStageId);
        private readonly MapManager _mapManager;
        private readonly GameState _gameState;
        private readonly Action<string> _onStartStage;
        private readonly Action _onLogout;

        private string _selectedStageId;

        public RoomPanelController(Transform parent, Action<string> onStartStage, Action onLogout, MapManager mapManager, GameState gameState)
        {
            _mapManager = mapManager;
            _gameState = gameState;
            _onStartStage = onStartStage;
            _onLogout = onLogout;

            _panel = UIFactory.Panel(parent, "RoomScreen");

            // 상단 바(accountId, gold, currentStage)
            _accountIdLabel = UIFactory.Text(_panel.transform, "Account: -", new Vector2(-520, 220));
            _goldLabel = UIFactory.Text(_panel.transform, "Gold: 0", new Vector2(0, 220));
            _currentStageLabel = UIFactory.Text(_panel.transform, "Current Stage: -", new Vector2(520, 220));

            // 중앙 3카드(좌/중/우) 슬라이더
            _leftCardLabel = UIFactory.Text(_panel.transform, PlaceholderStageId, new Vector2(-420, 60));
            _centerCardLabel = UIFactory.Text(_panel.transform, PlaceholderStageId, new Vector2(0, 60));
            _rightCardLabel = UIFactory.Text(_panel.transform, PlaceholderStageId, new Vector2(420, 60));
            UIFactory.Button(_panel.transform, "<", new Vector2(-650, 60), PrevStage);
            UIFactory.Button(_panel.transform, ">", new Vector2(650, 60), NextStage);

            // 하단 버튼(Start, Logout)
            UIFactory.Button(_panel.transform, "Start", new Vector2(-180, -220), HandleStart);
            UIFactory.Button(_panel.transform, "Logout", new Vector2(180, -220), HandleLogout);
        }

        public void Refresh()
        {
            var save = _gameState.SaveData;
            if (save == null)
            {
                _selectedStageId = string.Empty;
                _accountIdLabel.text = "Account: -";
                _goldLabel.text = "Gold: 0";
                _currentStageLabel.text = "Current Stage: -";
                UpdateCards();
                return;
            }

            _selectedStageId = save.currentStageId;
            _accountIdLabel.text = $"Account: {save.accountId}";
            _goldLabel.text = $"Gold: {save.gold}";
            _currentStageLabel.text = $"Current Stage: {save.currentStageId}";
            UpdateCards();
        }

        public void SetActive(bool active) => _panel.SetActive(active);

        private void PrevStage()
        {
            if (string.IsNullOrEmpty(_selectedStageId))
            {
                return;
            }

            var ids = _mapManager.GetSortedStageIds();
            if (ids.Count == 0)
            {
                return;
            }

            var currentIndex = ids.IndexOf(_selectedStageId);
            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            var nextIndex = Mathf.Max(0, currentIndex - 1);
            _selectedStageId = ids[nextIndex];
            UpdateCards();
        }

        private void NextStage()
        {
            if (string.IsNullOrEmpty(_selectedStageId))
            {
                return;
            }

            var ids = _mapManager.GetSortedStageIds();
            if (ids.Count == 0)
            {
                return;
            }

            var currentIndex = ids.IndexOf(_selectedStageId);
            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            var nextIndex = Mathf.Min(ids.Count - 1, currentIndex + 1);
            _selectedStageId = ids[nextIndex];
            UpdateCards();
        }

        private void UpdateCards()
        {
            List<string> window = _stageSelect.BuildWindow(_selectedStageId, _mapManager.GetSortedStageIds());
            _leftCardLabel.text = window[0];
            _centerCardLabel.text = window[1];
            _rightCardLabel.text = window[2];
        }

        private void HandleStart()
        {
            if (string.IsNullOrEmpty(_selectedStageId) || _selectedStageId == PlaceholderStageId)
            {
                return;
            }

            _onStartStage?.Invoke(_selectedStageId);
        }

        private void HandleLogout()
        {
            _onLogout?.Invoke();
        }
    }
}
