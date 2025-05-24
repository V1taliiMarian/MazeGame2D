using System.Collections.Generic;

namespace LabyrinthGame
{
    public class RandomLevelAchievement : AchievementTracker
    {
        public RandomLevelAchievement()
            : base(new List<int> { 5, 15, 25, 35, 45, 55, 65, 75, 85, 95 }, 10)
        {
        }
    }
}