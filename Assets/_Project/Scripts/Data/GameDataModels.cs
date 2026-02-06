using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Data
{
    public enum Rarity { Common, Rare, Epic, Legendary }
    public enum SkillTriggerType { OnTurnStart, OnAttack, OnHit, OnBattleStart, Passive }
    public enum EffectType { Damage, Heal, Buff, Debuff, Shield }
    public enum StackRule { Replace, Stack, RefreshDuration }
    public enum ItemSlot { Weapon, Armor, Accessory }
    public enum DayEventType { None, Battle, Rest, Bonus, CurrencyEvent, Boss }

    [Serializable]
    public class SkillEffect
    {
        public EffectType effectType;
        public float power;
        public int flatValue;
        public float durationSeconds;
        public StackRule stackRule;
    }

    [Serializable]
    public class UpgradeCurve
    {
        public int maxLevel = 1;
        public float multiplier = 1f;
    }

    [Serializable]
    public class StatBlock
    {
        public int hp = 100;
        public int attack = 10;
        public int defense = 5;
        public int speed = 10;
    }

    [Serializable]
    public class StatsSnapshot
    {
        public int hp;
        public int maxHp;
        public int attack;
        public int defense;
        public int speed;

        public static StatsSnapshot From(StatBlock stats, int currentHp)
        {
            return new StatsSnapshot
            {
                hp = currentHp,
                maxHp = stats.hp,
                attack = stats.attack,
                defense = stats.defense,
                speed = stats.speed
            };
        }
    }

    [Serializable]
    public class StageProgress
    {
        public int clearedDays;
        public bool isCleared;
    }

    [Serializable]
    public class StageProgressEntry
    {
        public string stageId;
        public StageProgress progress = new StageProgress();
    }

    [Serializable]
    public class SaveData
    {
        public string accountId;
        public int gold;
        public int premium;
        public List<string> inventory = new List<string>();
        public List<string> equipment = new List<string>();
        public string currentStageId = "1";
        public List<string> clearedStageIds = new List<string>();
        public List<StageProgressEntry> stageProgressEntries = new List<StageProgressEntry>();

        public StageProgress GetOrCreateProgress(string stageId)
        {
            var found = stageProgressEntries.Find(x => x.stageId == stageId);
            if (found != null)
            {
                return found.progress;
            }

            var entry = new StageProgressEntry { stageId = stageId, progress = new StageProgress() };
            stageProgressEntries.Add(entry);
            return entry.progress;
        }
    }

    [Serializable]
    public class RunContext
    {
        public string stageId;
        public int currentDay;
        public StatBlock playerBaseStats = new StatBlock();
        public StatBlock playerCurrentStats = new StatBlock();
        public List<string> skillIds = new List<string>();
    }

    public class StageInfo
    {
        public string stageId;
        public string displayName;
        public int totalDays;
    }

    public class RunResult
    {
        public bool stageCleared;
        public string stageId;
        public int daysCleared;
    }
}
