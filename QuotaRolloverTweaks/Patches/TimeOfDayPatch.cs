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
            IL.TimeOfDay.SetNewProfitQuota += SetNewProfitQuota_Patch;
        }

        private static void SetNewProfitQuota_Patch(ILContext il)
        {
            /*
            // Find:

            int num = quotaFulfilled - profitQuota;
            
            // Offset considered quota by previous amount
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
            c.EmitDelegate<Func<int, int>>((overtimeBonus) =>
            {
                int calculatedOvertime = Mathf.Max(overtimeBonus - QuotaRolloverTweaksData.overflowQuota, 0);
                Plugin.Logger.LogInfo($"Calculated Overtime at: {calculatedOvertime}");
                return calculatedOvertime;
            });
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
    }
}

// Insert new IL instructions to set overtimeBonus according to your C# code
//c.Emit(OpCodes.Ldarg_0); // Load "this" onto the stack
//c.Emit(OpCodes.Ldfld, typeof(QuotaRolloverTweaksData).GetField("overflowQuota")); // Load QuotaRolloverTweaksData.overflowQuota onto the stack
//c.Emit(OpCodes.Sub); // Subtract QuotaRolloverTweaksData.overflowQuota from num
//c.Emit(OpCodes.Ldc_I4_5); // Load 5 onto the stack
//c.Emit(OpCodes.Div); // Divide (num - QuotaRolloverTweaksData.overflowQuota) by 5
//c.Emit(OpCodes.Ldc_I4, 15); // Load 15 onto the stack
//c.Emit(OpCodes.Ldarg_0); // Load "this" onto the stack
//c.Emit(OpCodes.Ldfld, typeof(TimeOfDay).GetField("daysUntilDeadline")); // Load daysUntilDeadline onto the stack
//c.Emit(OpCodes.Mul); // Multiply 15 by daysUntilDeadline
//c.Emit(OpCodes.Add); // Add (num - QuotaRolloverTweaksData.overflowQuota) / 5 + 15 * daysUntilDeadline
//c.Emit(OpCodes.Stloc_1); // Store the result in overtimeBonus
