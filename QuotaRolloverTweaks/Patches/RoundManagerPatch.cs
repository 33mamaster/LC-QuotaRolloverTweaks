using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace QuotaRolloverTweaks.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatch
    {
        [HarmonyPatch("DespawnPropsAtEndOfRound")]
        [HarmonyPostfix]
        private static void DespawnPropsAtEndOfRoundPatch()
        {
            if (StartOfRound.Instance.allPlayersDead && Config.removeFulfilledQuotaOnDeath.Value)
            {
                Plugin.Logger.LogInfo($"Are all players dead?: {StartOfRound.Instance.allPlayersDead}");
                // Reset amounts
                QuotaRolloverTweaksData.overflowQuota = 0;
                TimeOfDay.Instance.quotaFulfilled = 0;
            }
        }
    }
}
