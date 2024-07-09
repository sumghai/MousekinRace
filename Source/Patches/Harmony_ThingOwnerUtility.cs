using HarmonyLib;
using Verse;

namespace MousekinRace
{
    // Set container temperature of Root Cellars to user-specified value in mod settings
    [HarmonyPatch(typeof(ThingOwnerUtility), nameof(ThingOwnerUtility.TryGetFixedTemperature))]
    public static class Harmony_ThingOwnerUtility
    {
        static bool Prefix(ref bool __result, ref float temperature, IThingHolder holder)
        {
            if (holder is Building_StorageCellar)
            {
                temperature = MousekinRaceMod.Settings.RootCellarTemperature;
                __result = true;
                return false;
            }
            return true;
        }
    }
}
