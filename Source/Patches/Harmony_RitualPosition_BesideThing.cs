using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    // If a pawn ritual position is defined as beside a target building, and the building is a Mousekin Town Square,
	// reposition the pawn to stand at a pre-calculated speaker position cell instead (i.e. in front of the flagpole)
	[HarmonyPatch(typeof(RitualPosition_BesideThing), nameof(RitualPosition_BesideThing.GetCell))]
    public class Harmony_RitualPosition_BesideThing_GetCell_AdjustForTownSquare
    {
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
