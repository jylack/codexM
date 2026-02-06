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
