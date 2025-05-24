using System.Collections.Generic;

namespace LabyrinthGame
{
    public class CoinsAchievement : AchievementTracker
    {
        public CoinsAchievement()
            : base(new List<int> { 25, 50, 75, 100, 125, 150, 175, 200, 225, 250,
                275, 300, 325, 350, 375, 400, 425, 450, 475, 500 }, 5)
        {
        }
    }
}