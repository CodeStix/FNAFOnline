using Stx.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stx.Net.Achievements
{
    [Serializable]
    public class Achievement
    {
        public string Name { get; }
        public int Goal { get; set; }
        public int RewardLevels { get; set; }
        public Hashtable ValuesPerClient { get; }
        public List<string> GrantedToClients { get; } = new List<string>();

        public delegate void GrantedDelegate(AchievementGrantedInfo grantedInfo);
        public static event GrantedDelegate OnGranted;

        public Achievement(string name, int goal, int rewardLevels)
        {
            this.Name = name;
            this.Goal = goal;
            this.RewardLevels = rewardLevels;
            this.ValuesPerClient = new Hashtable();
        }

        public void TryGrantAll()
        {
            foreach(string client in ValuesPerClient.Keys)
            {
                TryGrant(client);
            }
        }

        public bool TryGrant(string clientID)
        {
            if (GrantedToClients.Contains(clientID))
                return false;

            if (!ValuesPerClient.ContainsKey(clientID))
                ValuesPerClient.Add(clientID, 0);

            int i = GetValueFor(clientID);

            if (i >= Goal)
            {
                GrantedToClients.Add(clientID);

                OnGranted?.Invoke(new AchievementGrantedInfo(Name, clientID, Goal, RewardLevels));

                return true;
            }

            return false;
        }

        public int GetValueFor(string clientID)
        {
            if (!ValuesPerClient.ContainsKey(clientID))
                ValuesPerClient.Add(clientID, 0);

            if (ValuesPerClient[clientID] is int)
                return (int)ValuesPerClient[clientID];
            else
                return int.Parse(ValuesPerClient[clientID].ToString());
        }

        public void SetValueFor(string clientID, int value)
        {
            if (!ValuesPerClient.ContainsKey(clientID))
                ValuesPerClient.Add(clientID, 0);

            ValuesPerClient[clientID] = value;

            TryGrant(clientID);
        }

        public void IncreaseValueFor(string clientID, int value = 1)
        {
            int i = GetValueFor(clientID);

            i += value;

            SetValueFor(clientID, i);

            TryGrant(clientID);
        }
    }
}
