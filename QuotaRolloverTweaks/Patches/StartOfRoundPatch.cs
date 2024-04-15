using HarmonyLib;

// TODO: Remove this debugging file
namespace QuotaRolloverTweaks.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        [HarmonyPatch("ChangeLevel")]
        [HarmonyPostfix]
        private static void SetBuyingRateForDayOverride(ref int ___currentLevelID)
        {
            Plugin.Logger.LogInfo($"Current level: {___currentLevelID}");
            if (___currentLevelID == 3)
            {
                Plugin.Logger.LogInfo("Level landed to Company detected, prepare to sell easily");
                Plugin.Logger.LogInfo($"Original Buy Rate: {StartOfRound.Instance.companyBuyingRate} | Setting Buy Rate to 3x");
                // Set buy rate to x3
                StartOfRound.Instance.companyBuyingRate = 3f;
            }
        }
    }
}
