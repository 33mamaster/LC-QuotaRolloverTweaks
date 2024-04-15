using GameNetcodeStuff;
using HarmonyLib;

// TODO: Remove this debugging file
namespace QuotaRolloverTweaks.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerB_Patch
    {
        [HarmonyPatch("Crouch")]
        [HarmonyPostfix]
        static void croutchTrigger()
        {
            Plugin.Logger.LogInfo("Croutched");
            Plugin.Logger.LogInfo($"Reading overflowQuota: {QuotaRolloverTweaksData.overflowQuota}");
        }
    }
}
