// 인게임 화면의 상단 정보/전투 로그/스탯 패널/스킬 보상 팝업과 DayManager 이벤트 연결을 담당합니다.
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
        private readonly UITextRef _header;
        private readonly UITextRef _log;
        private readonly Button[] _skillButtons = new Button[3];
        private readonly DayManager _dayManager;
        private readonly Action _onRunEndedToRoom;

        private readonly Action<StageInfo> _onStageEntered;
        private readonly Action<int> _onDayChanged;
        private readonly Action<string> _onBattleLog;
        private readonly Action<StatsSnapshot> _onStatsChanged;
        private readonly Action<List<SkillData>> _onSkillChoiceOffered;
        private readonly Action<RunResult> _onRunEnded;

        private int _totalDays;

        public InGamePanelController(Transform parent, DayManager dayManager, Action onRunEndedToRoom)
        {
            _dayManager = dayManager;
            _onRunEndedToRoom = onRunEndedToRoom;

            _panel = UIFactory.Panel(parent, "InGamePanel");

            for (var i = 0; i < _skillButtons.Length; i++)
            {
                var idx = i;
                _skillButtons[i] = UIFactory.Button(_panel.transform, $"Skill {i + 1}", new Vector2((i - 1) * 280, -150), () =>
                {
                    _dayManager.SubmitSkillChoice(UIFactory.GetButtonLabel(_skillButtons[idx]));
                    SetSkillButtonsActive(false);
                });
            }
            SetSkillButtonsActive(false);

            _logScrollRect = BuildBattleLogArea(_panel.transform);
            _logContent = _logScrollRect.content.GetComponent<Text>();

            BuildStatsPanels(_panel.transform, out _hpLabel, out _atkLabel, out _defLabel, out _spdLabel);
            UIFactory.Button(_panel.transform, "Exit Stage", new Vector2(0, -250), () => _dayManager.RequestExitStage());

            _skillRewardPopup = new SkillRewardPopup(_panel.transform, _dayManager);

            _onStageEntered = OnStageEntered;
            _onDayChanged = OnDayChanged;
            _onBattleLog = AppendLog;
            _onStatsChanged = OnStatsChanged;
            _onSkillChoiceOffered = skills => _skillRewardPopup.Open(skills);
            _onRunEnded = _ =>
            {
                _skillRewardPopup.Close();
                _onRunEndedToRoom?.Invoke();
            };

            _dayManager.OnStageEntered += _onStageEntered;
            _dayManager.OnDayChanged += _onDayChanged;
            _dayManager.OnBattleLog += _onBattleLog;
            _dayManager.OnStatsChanged += _onStatsChanged;
            _dayManager.OnSkillChoiceOffered += _onSkillChoiceOffered;
            _dayManager.OnRunEnded += _onRunEnded;
        }

        public void Dispose()
        {
            _dayManager.OnStageEntered -= _onStageEntered;
            _dayManager.OnDayChanged -= _onDayChanged;
            _dayManager.OnBattleLog -= _onBattleLog;
            _dayManager.OnStatsChanged -= _onStatsChanged;
            _dayManager.OnSkillChoiceOffered -= _onSkillChoiceOffered;
            _dayManager.OnRunEnded -= _onRunEnded;
        }

        public void SetActive(bool active)
        {
            _panel.SetActive(active);
            if (!active)
            {
                _skillRewardPopup.Close();
                return;
            }

            _logContent.text = string.Empty;
            _logScrollRect.verticalNormalizedPosition = 1f;
        }

        private void OnStageEntered(StageInfo stage)
        {
            for (var i = 0; i < _skillButtons.Length; i++)
            {
                var label = i < skills.Count ? skills[i].id : "N/A";
                UIFactory.SetButtonLabel(_skillButtons[i], label);
            }
            SetSkillButtonsActive(true);
            Append("Choose a skill reward.");
        }

        private void OnDayChanged(int day)
        {
            _dayLabel.text = $"Day {day}/{Mathf.Max(1, _totalDays)}";
        }

        private void OnStatsChanged(StatsSnapshot snapshot)
        {
            _hpLabel.text = $"HP {snapshot.hp}/{snapshot.maxHp}";
            _atkLabel.text = $"ATK {snapshot.attack}";
            _defLabel.text = $"DEF {snapshot.defense}";
            _spdLabel.text = $"SPD {snapshot.speed}";
        }

        private void AppendLog(string line)
        {
            _logContent.text += string.IsNullOrEmpty(_logContent.text) ? line : $"\n{line}";
            Canvas.ForceUpdateCanvases();
            _logScrollRect.verticalNormalizedPosition = 0f;
        }

        private static ScrollRect BuildBattleLogArea(Transform parent)
        {
            var root = new GameObject("BattleLogRoot", typeof(RectTransform));
            root.transform.SetParent(parent, false);
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.sizeDelta = new Vector2(980, 520);
            rootRt.anchoredPosition = new Vector2(0, -10);

            var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
            viewport.transform.SetParent(root.transform, false);
            var viewportRt = viewport.GetComponent<RectTransform>();
            viewportRt.anchorMin = Vector2.zero;
            viewportRt.anchorMax = Vector2.one;
            viewportRt.offsetMin = Vector2.zero;
            viewportRt.offsetMax = Vector2.zero;
            viewport.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.35f);
            viewport.GetComponent<Mask>().showMaskGraphic = false;

            var content = new GameObject("Content", typeof(RectTransform), typeof(Text), typeof(ContentSizeFitter));
            content.transform.SetParent(viewport.transform, false);
            var contentRt = content.GetComponent<RectTransform>();
            contentRt.anchorMin = new Vector2(0, 1);
            contentRt.anchorMax = new Vector2(1, 1);
            contentRt.pivot = new Vector2(0.5f, 1f);
            contentRt.anchoredPosition = Vector2.zero;
            contentRt.sizeDelta = new Vector2(0, 0);

            var text = content.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 26;
            text.alignment = TextAnchor.UpperLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.color = Color.white;
            text.text = string.Empty;

            var fitter = content.GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scrollRect = root.AddComponent<ScrollRect>();
            scrollRect.viewport = viewportRt;
            scrollRect.content = contentRt;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            return scrollRect;
        }

        private static void BuildStatsPanels(Transform parent, out Text hp, out Text atk, out Text def, out Text spd)
        {
            hp = BuildStatPanel(parent, "HP", new Vector2(-690, 70));
            atk = BuildStatPanel(parent, "ATK", new Vector2(690, 70));
            def = BuildStatPanel(parent, "DEF", new Vector2(-690, -80));
            spd = BuildStatPanel(parent, "SPD", new Vector2(690, -80));
        }

        private static Text BuildStatPanel(Transform parent, string name, Vector2 anchoredPos)
        {
            var panel = new GameObject($"{name}_Panel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            var rt = panel.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(300, 110);
            rt.anchoredPosition = anchoredPos;
            panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.45f);

            var label = UIFactory.Text(panel.transform, name, Vector2.zero);
            label.rectTransform.sizeDelta = new Vector2(260, 70);
            label.alignment = TextAnchor.MiddleCenter;
            return label;
        }

        private sealed class SkillRewardPopup
        {
            private readonly GameObject _popup;
            private readonly DayManager _dayManager;
            private readonly Button[] _skillButtons = new Button[3];

            public SkillRewardPopup(Transform parent, DayManager dayManager)
            {
                _dayManager = dayManager;

                _popup = new GameObject("SkillRewardPopup", typeof(RectTransform), typeof(Image));
                _popup.transform.SetParent(parent, false);
                var rt = _popup.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(900, 380);
                rt.anchoredPosition = new Vector2(0, -20);
                _popup.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.9f);

                var title = UIFactory.Text(_popup.transform, "Choose Skill Reward", new Vector2(0, 130));
                title.rectTransform.sizeDelta = new Vector2(700, 60);

                for (var i = 0; i < _skillButtons.Length; i++)
                {
                    var idx = i;
                    _skillButtons[i] = UIFactory.Button(_popup.transform, "N/A", new Vector2((i - 1) * 280, -20), () =>
                    {
                        var skillId = _skillButtons[idx].GetComponentInChildren<Text>().text;
                        if (!string.Equals(skillId, "N/A", StringComparison.Ordinal))
                        {
                            _dayManager.SubmitSkillChoice(skillId);
                            Close();
                        }
                    });
                }

                Close();
            }

            public void Open(List<SkillData> skills)
            {
                for (var i = 0; i < _skillButtons.Length; i++)
                {
                    var label = i < skills.Count ? skills[i].id : "N/A";
                    _skillButtons[i].GetComponentInChildren<Text>().text = label;
                    _skillButtons[i].interactable = !string.Equals(label, "N/A", StringComparison.Ordinal);
                }

                _popup.SetActive(true);
            }

            public void Close() => _popup.SetActive(false);
        }
    }
}
