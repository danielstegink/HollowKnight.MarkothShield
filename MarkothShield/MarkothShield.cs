using HollowKnight.MarkothShield;
using Modding;
using System.Collections.Generic;
using UnityEngine;

namespace MarkothShield
{
    public class MarkothShield : Mod, IGlobalSettings<GlobalSettings>, ICustomMenuMod
    {
        public static MarkothShield Instance;

        public override string GetVersion() => "1.0.0.0";

        /// <summary>
        /// This needs to trigger after EE for it to work as intended
        /// </summary>
        /// <returns></returns>
        public override int LoadPriority() => 2;

        #region Global Settings
        internal GlobalSettings globalSettings = new GlobalSettings();

        public void OnLoadGlobal(GlobalSettings s)
        {
            globalSettings = s;
        }

        public GlobalSettings OnSaveGlobal()
        {
            return globalSettings;
        }
        #endregion

        private Sprite markothSprite;
        private Sprite radiantSprite;

        private IMod exaltMod;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            Instance = this;

            markothSprite = DanielSteginkUtils.Helpers.SpriteHelper.GetLocalSprite($"MarkothShield.Resources.38.png", "MarkothShield");
            radiantSprite = DanielSteginkUtils.Helpers.SpriteHelper.GetLocalSprite($"MarkothShield.Resources.Radiant.png", "MarkothShield");

            exaltMod = ModHooks.GetMod("ExaltationExpanded");

            ModHooks.LanguageGetHook += LanguageGet;
            On.CharmIconList.GetSprite += GetSprite;
            ModHooks.ObjectPoolSpawnHook += SpawnExtraShield;

            Log("Initialized");
        }

        #region Charm Info
        /// <summary>
        /// If we upgrade the Dreamshield, we want a new name and description for it
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sheetTitle"></param>
        /// <param name="orig"></param>
        /// <returns></returns>
        private string LanguageGet(string key, string sheetTitle, string orig)
        {
            if (key.Equals("CHARM_NAME_38") && 
                CanUpgrade())
            {
                if (IsExalted())
                {
                    return "Radiant Shield";
                }

                return "Markoth Shield";
            }
            else if (key.Equals("CHARM_DESC_38") &&
                CanUpgrade())
            {
                if (IsExalted())
                {
                    return "Defensive charm empowered by the god of isolation.\n\n" +
                            "Conjures four shields that follow the bearer and attempt to protect them.";
                }

                return "Defensive charm empowered by the lingering dream of a fallen warrior.\n\n" +
                        "Conjures two shields that follow the bearer and attempt to protect them.";
            }

            return orig;
        }

        /// <summary>
        /// Since Markoth Shield is a new charm, it deserves a new icon
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private Sprite GetSprite(On.CharmIconList.orig_GetSprite orig, CharmIconList self, int id)
        {
            if (id == 38 &&
                CanUpgrade())
            {
                if (IsExalted())
                {
                    return radiantSprite;
                }

                return markothSprite;
            }

            return orig(self, id);
        }
        #endregion

        /// <summary>
        /// Spawns a second shield
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private GameObject SpawnExtraShield(GameObject gameObject)
        {
            if (gameObject.name.Contains("Shield") &&
                CanUpgrade())
            {
                if (IsExalted())
                {
                    GameObject shield1 = GameObject.Instantiate(gameObject, gameObject.transform.position, Quaternion.identity);
                    shield1.transform.Rotate(0, 0, 90);

                    GameObject shield2 = GameObject.Instantiate(gameObject, gameObject.transform.position, Quaternion.identity);
                    shield2.transform.Rotate(0, 0, 270);
                }
                else
                {
                    GameObject shield = GameObject.Instantiate(gameObject, gameObject.transform.position, Quaternion.identity);
                    shield.transform.Rotate(0, 0, 180);
                }
            }

            return gameObject;
        }

        #region Logic Checks
        /// <summary>
        /// Checks if the Dreamshield can be upgraded to the Markoth Shield
        /// </summary>
        /// <returns></returns>
        private bool CanUpgrade()
        {
            switch(globalSettings.difficultyLevel)
            {
                case 0:
                    return PlayerData.instance.gotCharm_38;
                case 1:
                    return PlayerData.instance.killedGhostMarkoth;
                case 2:
                    return PlayerData.instance.statueStateMarkoth.completedTier1;
                case 3:
                    return PlayerData.instance.statueStateMarkoth.completedTier2;
                case 4:
                    return PlayerData.instance.statueStateMarkoth.completedTier3;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Exaltation Expanded upgrades Dreamshield upon defeating Ascended Markoth
        /// </summary>
        /// <returns></returns>
        private bool IsExalted()
        {
            if (exaltMod == null)
            {
                return false;
            }

            return PlayerData.instance.statueStateMarkoth.completedTier2;
        }
        #endregion

        #region Menu Options
        public bool ToggleButtonInsideMenu => false;

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? modToggleDelegates)
        {
            return ModMenu.CreateMenuScreen(modListMenu);
        }
        #endregion
    }
}