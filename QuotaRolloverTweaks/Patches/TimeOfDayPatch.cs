using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;


namespace QuotaRolloverTweaks.Patches
{
    [HarmonyPatch(typeof(TimeOfDay))]
    internal class TimeOfDayPatch
    {
        internal static void Init()
        {
            if (Config.currentQuotaScrapOnly.Value)
            {
                IL.TimeOfDay.SetNewProfitQuota += currentQuotaScrapOnly;
            }
            if (Config.noOvertime.Value)
            {
                IL.TimeOfDay.SetNewProfitQuota += noOvertimeBonus;
            }
        }

        [HarmonyPatch("SetNewProfitQuota")]
        [HarmonyPostfix]
        private static void SaveQuotaFulfillment(ref int ___quotaFulfilled)
        {
            // Match QuotaRollover setting
            if (TimeOfDay.Instance.daysUntilDeadline < 0)
            {
                Plugin.Logger.LogDebug($"Setting New Overflow Quota: {___quotaFulfilled}");
                QuotaRolloverTweaksData.overflowQuota = ___quotaFulfilled;
            }
        }

        private static void currentQuotaScrapOnly(ILContext il)
        {
            /* Offset considered quota by previous amount
            // Find:

            int num = quotaFulfilled - profitQuota;

            // Replace
            int num = Mathf.Max(quotaFulfilled - profitQuota - overflowQuota, 0);
            
            */
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                MoveType.After,
                i => i.MatchLdarg(0),
                i => i.MatchLdfld(typeof(TimeOfDay).GetField("quotaFulfilled")),
                i => i.MatchLdarg(0),
                i => i.MatchLdfld(typeof(TimeOfDay).GetField("profitQuota")),
                i => i.OpCode == OpCodes.Sub
            );

            // Insert new formula
            // int num = Mathf.Max(quotaFulfilled - profitQuota - overflowQuota, 0)
            c.EmitDelegate<Func<int, int>>((num) =>
            {
                int calculatedOvertime = Mathf.Max(num - QuotaRolloverTweaksData.overflowQuota, 0);
                Plugin.Logger.LogInfo($"Calculated Overtime at: {calculatedOvertime}");
                return calculatedOvertime;
            });


        }

        private static void noOvertimeBonus(ILContext il)
        {
            /* Remove Overtime
            // Find:

            SyncNewProfitQuotaClientRpc(profitQuota, overtimeBonus, timesFulfilledQuota);

            // Replace:
            SyncNewProfitQuotaClientRpc(profitQuota, 0, timesFulfilledQuota);

            */
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                i => i.MatchLdarg(0), // Match ldarg.0
                i => i.MatchLdarg(0), // Match ldarg.0 again
                i => i.MatchLdfld(typeof(TimeOfDay).GetField("profitQuota")), // Match ldfld TimeOfDay::profitQuota
                i => i.MatchLdloc(1), // Match ldloc.1
                i => i.MatchLdarg(0), // Match ldarg.0
                i => i.MatchLdfld(typeof(TimeOfDay).GetField("timesFulfilledQuota")), // Match ldfld TimeOfDay::timesFulfilledQuota
                i => i.MatchCall(typeof(TimeOfDay).GetMethod("SyncNewProfitQuotaClientRpc", new[] { typeof(int), typeof(int), typeof(int) })) // Match call to SyncNewProfitQuotaClientRpc
            );

            // Remove old commands and replace
            c.Index += 3;
            c.Remove();
            c.Emit(OpCodes.Ldc_I4_0);

            Plugin.Logger.LogInfo(il.ToString());
        }
    }
}