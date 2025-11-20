using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Noise;

namespace MousekinRace
{
    // Conditionally apply bonus to terrain fertility if the cell was fertilized (and not polluted when Biotech DLC is active)
    [HarmonyPatch(typeof(FertilityGrid), nameof(FertilityGrid.CalculateFertilityAt))]    
    public class Harmony_FertilityGrid_CalculateFertilityAt_CondApplyFertilizerBonus
    {
        static void Postfix(IntVec3 loc, Map ___map, ref float __result)
        {
            if (!___map.pollutionGrid.IsPolluted(loc) && ___map.GetComponent<MapComponent_Fertilizer>().cellFertilityBonus.ContainsKey(loc))
            {
                __result *= (1f + MousekinDefOf.Mousekin_Saltpeter.GetModExtension<FertilizerExtension>().fertilityBonus);
            }
        }
    }
}