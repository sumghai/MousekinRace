using HarmonyLib;
using Verse.Profile;

namespace MousekinRace
{
    [HarmonyPatch(typeof(MemoryUtility), nameof(MemoryUtility.ClearAllMapsAndWorld))]
    public static class Harmony_MemoryUtility_ClearAllMapsAndWorld_ClearMousekinTempData
    {
        static void Postfix()
        {
            UndergroundMineSys_Utils.Clipboard = null;
        }
    }
}
