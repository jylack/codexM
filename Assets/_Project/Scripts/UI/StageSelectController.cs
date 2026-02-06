// 현재 도전 스테이지 기준으로 [이전, 현재, 다음] 3카드 뷰를 계산합니다.
using System.Collections.Generic;
using System.Linq;

namespace Project.UI
{
    public class StageSelectController
    {
        private readonly string _placeholderStageId;

        public StageSelectController(string placeholderStageId = "---")
        {
            _placeholderStageId = placeholderStageId;
        }

        public List<string> BuildWindow(string currentStageId, List<string> allStageIds)
        {
            if (allStageIds == null || allStageIds.Count == 0)
            {
                return new List<string> { _placeholderStageId, _placeholderStageId, _placeholderStageId };
            }

            var sorted = allStageIds
                .Distinct()
                .OrderBy(ParseStageId)
                .ToList();

            var currentIndex = sorted.IndexOf(currentStageId);
            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            var left = currentIndex > 0 ? sorted[currentIndex - 1] : _placeholderStageId;
            var center = sorted[currentIndex];
            var right = currentIndex < sorted.Count - 1 ? sorted[currentIndex + 1] : _placeholderStageId;
            return new List<string> { left, center, right };
        }

        private static int ParseStageId(string stageId)
        {
            return int.TryParse(stageId, out var value) ? value : int.MaxValue;
        }
    }
}
