// 스킬 ScriptableObject 스키마 정의입니다.
using UnityEngine;

namespace Project.Data
{
    [CreateAssetMenu(menuName = "_Project/Skills/SkillData", fileName = "Skill_")]
    public class SkillData : ScriptableObject
    {
        public string id;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;
        public Rarity rarity;
        public SkillTriggerType trigger;
        public float cooldownSeconds;
        public string condition;
        public SkillEffect effect;
        public string vfxKey;
        public string sfxKey;
    }
}
