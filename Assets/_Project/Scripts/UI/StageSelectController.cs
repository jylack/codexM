using System.Collections.Generic;
using System.Linq;

namespace Project.UI
{
    public class StageSelectController
    {
        public List<string> BuildWindow(string currentStageId, List<string> allStageIds)
        {
            var sorted = allStageIds.OrderBy(int.Parse).ToList();
            var currentIndex = sorted.IndexOf(currentStageId);
            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            var left = currentIndex > 0 ? sorted[currentIndex - 1] : sorted[currentIndex];
            var center = sorted[currentIndex];
            var right = currentIndex < sorted.Count - 1 ? sorted[currentIndex + 1] : sorted[currentIndex];
            return new List<string> { left, center, right };
        }
    }
}
