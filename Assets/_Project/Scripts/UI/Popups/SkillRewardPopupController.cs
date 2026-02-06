using System;
using System.Collections.Generic;
using Project.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class SkillRewardPopupController
    {
        private readonly GameObject _root;
        private readonly Text _title;
        private readonly Button[] _skillButtons = new Button[3];

        public SkillRewardPopupController(Transform parent)
        {
            _root = PopupPrefabBinder.CreateOrFallback(parent, "SkillRewardPopup");
            _title = UIFactory.Text(_root.transform, "Choose Skill Reward", new Vector2(0, 180));
            _title.rectTransform.sizeDelta = new Vector2(980, 80);

            for (var i = 0; i < _skillButtons.Length; i++)
            {
                _skillButtons[i] = UIFactory.Button(_root.transform, "-", new Vector2(0, 80 - (i * 90)), null);
                _skillButtons[i].GetComponent<RectTransform>().sizeDelta = new Vector2(1050, 70);
            }

            SetActive(false);
        }

        public void Open(IReadOnlyList<SkillData> skills, Action<string> onSelected)
        {
            for (var i = 0; i < _skillButtons.Length; i++)
            {
                var hasSkill = i < skills.Count;
                var btn = _skillButtons[i];
                btn.gameObject.SetActive(hasSkill);
                btn.onClick.RemoveAllListeners();
                if (!hasSkill)
                {
                    continue;
                }

                var skill = skills[i];
                var label = $"{skill.displayName} - {ToShortDescription(skill.description)}";
                btn.GetComponentInChildren<Text>().text = label;
                btn.onClick.AddListener(() =>
                {
                    onSelected?.Invoke(skill.id);
                    SetActive(false);
                });
            }

            SetActive(true);
        }

        public void SetActive(bool active) => _root.SetActive(active);

        private static string ToShortDescription(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "No description";
            }

            const int maxLen = 32;
            var oneLine = text.Replace("\n", " ").Trim();
            return oneLine.Length <= maxLen ? oneLine : $"{oneLine.Substring(0, maxLen)}...";
        }
    }
}
