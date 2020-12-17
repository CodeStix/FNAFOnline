using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stx.Net.Achievements
{
    [Serializable]
    public class AchievementSystem
    {
        public List<Achievement> Achievements { get; } = new List<Achievement>();
        public Hashtable ClientLevels { get; } = new Hashtable();

        public AchievementSystem()
        {
            Achievement.OnGranted += Achievement_OnGranted;
        }

        private void Achievement_OnGranted(AchievementGrantedInfo grantedInfo)
        {
            int l = GetClientLevel(grantedInfo.GrantedToClient);

            l += grantedInfo.RewardLevels;

            ClientLevels[grantedInfo.GrantedToClient] = l;
        }

        public void TryGrantAllAchievements()
        {
            foreach (Achievement a in Achievements)
            {
                a.TryGrantAll();
            }
        }

        public int GetClientLevel(string clientID)
        {
            if (!ClientLevels.ContainsKey(clientID))
                ClientLevels.Add(clientID, 0);

            return Convert.ToInt32(ClientLevels[clientID]);
        }

        public Achievement GetFromName(string achivementName)
        {
            return Achievements.FirstOrDefault((e) => e.Name == achivementName);
        }
    }
}
