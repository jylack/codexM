// 인게임 로그/보상 선택/종료 버튼 UI와 DayManager 이벤트 연결을 담당합니다.
using System;
using System.Collections.Generic;
using Project.Data;
using Project.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class InGamePanelController
    {
        private readonly GameObject _panel;
        private readonly Text _header;
        private readonly Text _log;
        private readonly Button[] _skillButtons = new Button[3];
        private readonly DayManager _dayManager;
        private readonly Action<StageInfo> _onStageEntered;
        private readonly Action<int> _onDayChanged;

        public InGamePanelController(Transform parent, DayManager dayManager)
        {
            _dayManager = dayManager;
            _panel = UIFactory.Panel(parent, "InGamePanel");
            _header = UIFactory.Text(_panel.transform, "InGame", new Vector2(0, 230));
            _log = UIFactory.Text(_panel.transform, "", new Vector2(0, 60));
            _log.alignment = TextAnchor.UpperCenter;
            _log.rectTransform.sizeDelta = new Vector2(1200, 260);

            for (var i = 0; i < _skillButtons.Length; i++)
            {
                var idx = i;
                _skillButtons[i] = UIFactory.Button(_panel.transform, $"Skill {i + 1}", new Vector2((i - 1) * 280, -150), () =>
                {
                    _dayManager.SubmitSkillChoice(_skillButtons[idx].GetComponentInChildren<Text>().text);
                    SetSkillButtonsActive(false);
                });
            }
            SetSkillButtonsActive(false);

            UIFactory.Button(_panel.transform, "Exit Run", new Vector2(0, -250), () => _dayManager.RequestExitStage());

            _onStageEntered = s => _header.text = $"{s.displayName} ({s.stageId})";
            _onDayChanged = day => Append($"Day {day}");
            _dayManager.OnStageEntered += _onStageEntered;
            _dayManager.OnDayChanged += _onDayChanged;
            _dayManager.OnBattleLog += Append;
            _dayManager.OnSkillChoiceOffered += OnSkillOffered;
        }

        public void Dispose()
        {
            _dayManager.OnStageEntered -= _onStageEntered;
            _dayManager.OnDayChanged -= _onDayChanged;
            _dayManager.OnBattleLog -= Append;
            _dayManager.OnSkillChoiceOffered -= OnSkillOffered;
        }

        public void SetActive(bool active)
        {
            _panel.SetActive(active);
            if (active)
            {
                _log.text = string.Empty;
            }
        }

        private void OnSkillOffered(List<SkillData> skills)
        {
            for (var i = 0; i < _skillButtons.Length; i++)
            {
                var label = i < skills.Count ? skills[i].id : "N/A";
                _skillButtons[i].GetComponentInChildren<Text>().text = label;
            }
            SetSkillButtonsActive(true);
            Append("Choose a skill reward.");
        }

        private void SetSkillButtonsActive(bool active)
        {
            foreach (var btn in _skillButtons)
            {
                btn.gameObject.SetActive(active);
            }
        }

        private void Append(string line)
        {
            _log.text += $"
{line}";
        }
    }
}
