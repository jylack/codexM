// 아이템 ScriptableObject 스키마 정의입니다.
using System.Collections.Generic;
using UnityEngine;

namespace Project.Data
{
    [CreateAssetMenu(menuName = "_Project/Items/ItemData", fileName = "Item_")]
    public class ItemData : ScriptableObject
    {
        public string id;
        public string displayName;
        public Sprite icon;
        public Rarity rarity;
        public ItemSlot slot;
        public StatBlock baseStats;
        public List<SkillData> grantedSkills = new List<SkillData>();
        public UpgradeCurve upgradeCurve;
    }
}
