using System.Collections.Generic;

namespace LabyrinthGame
{
    public class TimeAttackAchievement : AchievementTracker
    {
        public TimeAttackAchievement()
            : base(new List<int> { 5, 15, 25, 35, 45, 55, 65, 75, 85, 95 }, 5)
        {
        }
    }
}