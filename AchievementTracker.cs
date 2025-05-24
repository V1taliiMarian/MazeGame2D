using System;
using System.Collections.Generic;
using System.Linq;

namespace LabyrinthGame
{
    public abstract class AchievementTracker
    {
        public List<int> Thresholds { get; protected set; }
        public List<int> ClaimedThresholds { get; set; }
        public int RewardAmount { get; protected set; }

        protected AchievementTracker(List<int> thresholds, int rewardAmount)
        {
            Thresholds = thresholds;
            ClaimedThresholds = new List<int>();
            RewardAmount = rewardAmount;
        }

        public (int current, int nextThreshold, bool canClaim) GetProgress(int currentValue)
        {
            var nextUnclaimed = Thresholds
                .FirstOrDefault(t => !ClaimedThresholds.Contains(t) && currentValue >= t);

            var nextThreshold = nextUnclaimed != 0
                ? nextUnclaimed
                : Thresholds.FirstOrDefault(t => !ClaimedThresholds.Contains(t));

            bool canClaim = nextUnclaimed != 0;

            return (currentValue, nextThreshold, canClaim);
        }

        public bool ClaimReward(int currentValue, Action<int> addCoinsAction)
        {
            var unclaimed = Thresholds
                .Where(t => t <= currentValue && !ClaimedThresholds.Contains(t))
                .ToList();

            if (!unclaimed.Any())
                return false;

            var threshold = unclaimed.Min();
            ClaimedThresholds.Add(threshold);
            addCoinsAction?.Invoke(RewardAmount);
            return true;
        }

        public List<int> GetUnclaimedThresholds() =>
            Thresholds.Where(t => !ClaimedThresholds.Contains(t)).ToList();
    }
}