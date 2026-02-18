using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace MousekinRace
{
    // If a pawn ritual position is defined as beside a target building, and the building is a Mousekin Town Square,
    // reposition the pawn to stand at a pre-calculated speaker position cell instead (i.e. in front of the flagpole)
    [HarmonyPatch]
    public class Harmony_RitualPosition_Generic_GetCell_OverrideForTownSquare
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(RitualPosition_BehindThingCenter), nameof(RitualPosition_BehindThingCenter.GetCell));
            yield return AccessTools.Method(typeof(RitualPosition_BesideThing), nameof(RitualPosition_BesideThing.GetCell));
        }
        
        static bool Prefix(LordJob_Ritual ritual, ref PawnStagePosition __result)
        {
            if (ritual.selectedTarget.Thing is Building_TownSquare townSquare)
            {
                __result = new PawnStagePosition(townSquare.speechSpeakerCellPos, townSquare, Rot4.South, false);
                return false;
            }
            return true;
        }
    }
}
