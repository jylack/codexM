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
        private readonly DayManager _dayManager;
        private readonly SkillRewardPopupController _skillRewardPopup;
        private readonly CurrencyRewardPopupController _currencyRewardPopup;
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

            _skillRewardPopup = new SkillRewardPopupController(parent);
            _currencyRewardPopup = new CurrencyRewardPopupController(parent);

            UIFactory.Button(_panel.transform, "Exit Run", new Vector2(0, -250), () => _dayManager.RequestExitStage());

            _onStageEntered = s => _header.text = $"{s.displayName} ({s.stageId})";
            _onDayChanged = day => Append($"Day {day}\n");
            _dayManager.OnStageEntered += _onStageEntered;
            _dayManager.OnDayChanged += _onDayChanged;
            _dayManager.OnBattleLog += Append;
            _dayManager.OnSkillChoiceOffered += OnSkillOffered;
            _dayManager.OnCurrencyRewardOffered += OnCurrencyRewardOffered;
        }

        public void Dispose()
        {
            _dayManager.OnStageEntered -= _onStageEntered;
            _dayManager.OnDayChanged -= _onDayChanged;
            _dayManager.OnBattleLog -= Append;
            _dayManager.OnSkillChoiceOffered -= OnSkillOffered;
            _dayManager.OnCurrencyRewardOffered -= OnCurrencyRewardOffered;
        }

        public void SetActive(bool active)
        {
            _panel.SetActive(active);
            _skillRewardPopup.SetActive(false);
            _currencyRewardPopup.SetActive(false);
            if (active)
            {
                _log.text = string.Empty;
            }
        }

        private void OnSkillOffered(List<SkillData> skills)
        {
            _skillRewardPopup.Open(skills, skillId =>
            {
                _dayManager.SubmitSkillChoice(skillId);
            });
            Append("Choose a skill reward.\n");
        }

        private void OnCurrencyRewardOffered(int amount)
        {
            _currencyRewardPopup.Open(amount, () =>
            {
                _dayManager.NotifyCurrencyRewardConfirmed();
                Append("Currency reward confirmed.\n");
            });
        }

        private void Append(string line)
        {
            _log.text += line;
        }
    }
}
