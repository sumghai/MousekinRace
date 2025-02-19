using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    // Add Mousekin Town Squares to the list of allowed ritual obligation targets
    // for rituals such as Role Change, Execution etc.
    
    [HarmonyPatch(typeof(RitualObligationTargetWorker_AnyRitualSpotIdeogramOrAltar), nameof(RitualObligationTargetWorker_AnyRitualSpotIdeogramOrAltar.GetTargets))]
    public class Harmony_RitualObligationTargetWorker_AnyRitualSpotIdeogramOrAltar_GetTargets_AddTownSquare
    {
        static void Postfix(Map map, ref IEnumerable<TargetInfo> __result)
        {
            List<Thing> townSquares = map.listerThings.ThingsOfDef(MousekinDefOf.Mousekin_TownSquare);
            for (int j = 0; j < townSquares.Count; j++)
            {
                __result = __result.AddItem(townSquares[j]);
            }
        }
    }

    [HarmonyPatch(typeof(RitualObligationTargetWorker_AnyRitualSpotIdeogramOrAltar), nameof(RitualObligationTargetWorker_AnyRitualSpotIdeogramOrAltar.GetTargetInfos))]
    public class Harmony_RitualObligationTargetWorker_AnyRitualSpotIdeogramOrAltar_GetTargetInfos_AddTownSquare
    {
        static void Postfix(ref IEnumerable<string> __result)
        {
            __result = __result.AddItem(MousekinDefOf.Mousekin_TownSquare.label);
        }
    }

    [HarmonyPatch(typeof(RitualObligationTargetWorker_AnyRitualSpotIdeogramOrAltar), nameof(RitualObligationTargetWorker_AnyRitualSpotIdeogramOrAltar.CanUseTargetInternal))]
    public class Harmony_RitualObligationTargetWorker_AnyRitualSpotIdeogramOrAltar_CanUseTargetInternal_AddTownSquare
    {
        static void Postfix(TargetInfo target, ref RitualTargetUseReport __result)
        {
            if (target.Thing is Building_TownSquare)
            {
                __result = true;
            }
        }
    }
}
