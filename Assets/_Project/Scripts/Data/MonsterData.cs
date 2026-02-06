// 몬스터 ScriptableObject 스키마 정의입니다.
using System.Collections.Generic;
using UnityEngine;

namespace Project.Data
{
    [CreateAssetMenu(menuName = "_Project/Monsters/MonsterData", fileName = "Monster_")]
    public class MonsterData : ScriptableObject
    {
        public string id;
        public string displayName;
        public StatBlock statBlock;
        public List<SkillData> skills = new List<SkillData>();
    }
}
