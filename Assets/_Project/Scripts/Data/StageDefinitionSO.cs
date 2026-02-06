// 스테이지/일차(day) 구성 정보를 담는 ScriptableObject 스키마입니다.
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Data
{
    [Serializable]
    public class DayDefinition
    {
        public int dayNumber;
        public DayEventType dayEventType;
        public MonsterGroupData monsterGroup;
        public int healFlat;
        [Range(0f, 1f)] public float healPercent;
        public int goldFlat;
    }

    [CreateAssetMenu(menuName = "_Project/Stages/StageDefinition", fileName = "Stage_")]
    public class StageDefinitionSO : ScriptableObject
    {
        public string stageId;
        public string displayName;
        public List<DayDefinition> days = new List<DayDefinition>();
    }
}
