using System.Collections.Generic;

namespace LabyrinthGame
{
    public class SkinAchievement : AchievementTracker
    {
        public SkinAchievement()
            : base(new List<int> { 5, 10, 15, 20, 25, 30, 35, 40 }, 5)
        {
        }
    }
}