using LethalModDataLib.Attributes;
using LethalModDataLib.Enums;

namespace QuotaRolloverTweaks
{
    internal static class QuotaRolloverTweaksData
    {
        [ModData(SaveWhen.OnSave, LoadWhen = LoadWhen.OnLoad, ResetWhen = ResetWhen.OnGameOver)]
        internal static int overflowQuota = 0;
    }
}
