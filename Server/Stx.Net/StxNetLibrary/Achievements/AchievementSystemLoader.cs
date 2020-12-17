using Stx.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stx.Net.Achievements
{
    public static class AchievementSystemLoader
    {
        private static JsonConfig<AchievementSystem> achivementFile;

        public static AchievementSystem LoadFromFile(string fromFile)
        {
            achivementFile = new JsonConfig<AchievementSystem>(fromFile);

            return achivementFile.Settings;
        }

        public static void SaveToFile()
        {
            achivementFile.Save();
        }
    }
}
