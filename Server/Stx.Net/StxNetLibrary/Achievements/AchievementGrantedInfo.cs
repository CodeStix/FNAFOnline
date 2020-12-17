using Stx.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stx.Net.Achievements
{
    public class AchievementGrantedInfo : IByteDefined<AchievementGrantedInfo>
    {
        public string Name { get; set; }
        public string GrantedToClient { get; set; }
        public int GoalReached { get; set; }
        public int RewardLevels { get; set; }

        public AchievementGrantedInfo()
        { }

        public AchievementGrantedInfo(string name, string grantedToClient, int goalReached, int rewardLevels)
        {
            this.Name = name;
            this.GrantedToClient = grantedToClient;
            this.GoalReached = goalReached;
            this.RewardLevels = rewardLevels;
        }
    }
}
