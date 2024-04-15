using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using QuotaRolloverTweaks.Patches;

namespace QuotaRolloverTweaks
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("MaxWasUnavailable.LethalModDataLib")]
    [BepInDependency("Boxofbiscuits97.QuotraRollover")]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "33mamaster.QuotaRolloverTweaks";
        private const string modName = "Quota Rollover Tweaks";
        private const string modVersion = "1.0.0";

        internal static new ManualLogSource Logger;

        private readonly Harmony harmony = new Harmony(modGUID);

        private static Plugin Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            Logger = base.Logger;

            Logger.LogInfo($"Mod {modName} is loaded!");

            TimeOfDayPatch.Init();
            harmony.PatchAll();
        }


    }
}
