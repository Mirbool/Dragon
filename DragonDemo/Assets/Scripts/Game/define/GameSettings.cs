using QFSW.QC;
using UnityEngine;

namespace Koakuma.Game
{
    public static class GameSettings
    {
        public const string SKILL_CD_ENABLED_KEY = "SKILL_CD_ENABLED_KEY";
        private static bool? skillCDEnabled;
        [Command("setting.skill-cd-enable", MonoTargetType.All)]
        public static bool SkillCDEnabled
        {
            get
            {
                if (!skillCDEnabled.HasValue)
                {
                    skillCDEnabled = PlayerPrefs.GetInt(SKILL_CD_ENABLED_KEY, 1) == 1;
                }
                return skillCDEnabled.Value;
            }
            set
            {
                skillCDEnabled = value;
                PlayerPrefs.SetInt(SKILL_CD_ENABLED_KEY, value ? 1 : 0);
            }
        }

        public const string REGION_INDICATOR_ENABLED_KEY = "REGION_INDICATOR_ENABLED_KEY";
        private static bool? regionIndicatorEnabled;
        [Command("setting.region-indicator-enable", MonoTargetType.All)]
        public static bool RegionIndicatorEnabled
        {
            get
            {
                if (!regionIndicatorEnabled.HasValue)
                {
                    regionIndicatorEnabled = PlayerPrefs.GetInt(REGION_INDICATOR_ENABLED_KEY, 0) == 1;
                }
                return regionIndicatorEnabled.Value;
            }
            set
            {
                regionIndicatorEnabled = value;
                PlayerPrefs.SetInt(REGION_INDICATOR_ENABLED_KEY, value ? 1 : 0);
                //foreach (var character in GameManager.Data.AllCharacters.Values)
                //{
                //    if (value)
                //    {
                //        character.Actor.ShowCharacterRegion();
                //    }
                //    else
                //    {
                //        character.Actor.HideCharacterRegion();
                //    }
                //}
            }
        }
    }
}
