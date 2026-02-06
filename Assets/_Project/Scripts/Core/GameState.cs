// 영구 저장 데이터(SaveData)와 스테이지 진행 중 임시 데이터(RunContext)를 관리합니다.
using Project.Data;

namespace Project.Core
{
    public class GameState
    {
        public SaveData SaveData { get; private set; }
        public RunContext RunContext { get; private set; }

        public void SetSaveData(SaveData saveData)
        {
            SaveData = saveData;
        }

        public void StartRun(string stageId)
        {
            RunContext = new RunContext
            {
                stageId = stageId,
                currentDay = 1,
                playerBaseStats = new StatBlock { hp = 100, attack = 12, defense = 5, speed = 10 },
                playerCurrentStats = new StatBlock { hp = 100, attack = 12, defense = 5, speed = 10 }
            };
        }

        public void EndRun()
        {
            RunContext = null;
        }
    }
}
