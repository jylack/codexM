using System.Collections.Generic;
using Project.Data;

namespace Project.Gameplay
{
    public class MapManager
    {
        private readonly Dictionary<string, StageDefinitionSO> _byId = new Dictionary<string, StageDefinitionSO>();

        public MapManager(List<StageDefinitionSO> stages)
        {
            foreach (var stage in stages)
            {
                if (stage != null && !string.IsNullOrEmpty(stage.stageId))
                {
                    _byId[stage.stageId] = stage;
                }
            }
        }

        public StageDefinitionSO GetStage(string stageId)
        {
            return _byId.TryGetValue(stageId, out var stage) ? stage : null;
        }

        public List<string> GetSortedStageIds()
        {
            var ids = new List<string>(_byId.Keys);
            ids.Sort((a, b) => int.Parse(a).CompareTo(int.Parse(b)));
            return ids;
        }
    }
}
