using System;
using BepInEx.Configuration;

namespace QuotaRolloverTweaks
{
    internal class Config
    {
        public static ConfigEntry<bool> currentQuotaScrapOnly;
        public static ConfigEntry<bool> noOvertime;

        public static void Load()
        {
            currentQuotaScrapOnly = Plugin.config.Bind(
                "Overtime Bonus",
                "OnlyConsiderCurrentQuotasScrap",
                true,
                "Only consider newly added scrap this quota towards the overtime bonus. Basically removes rollovered quota from the overtime bonus calculation."
            );

            noOvertime = Plugin.config.Bind(
                "Overtime Bonus",
                "RemoveOvertimeBonus",
                false,
                "Disables Overtime bonus by reducing it to 0."
            );
        }
    }
}
