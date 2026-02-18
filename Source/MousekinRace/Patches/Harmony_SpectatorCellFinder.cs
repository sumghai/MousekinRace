using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace MousekinRace
{
    // If a ritual focus target is a Mousekin Town Square building, force spectators to only spectate from the bottom (south) side
    [HarmonyPatch(typeof(SpectatorCellFinder), nameof(SpectatorCellFinder.TryFindSpectatorCellFor))]
    public class Harmony_SpectatorCellFinder_TryFindSpectatorCellFor_OverrideForTownSquare
    {
        static void Prefix(Pawn p, ref SpectateRectSide allowedSides)
        {
            if (p.GetLord()?.LordJob is LordJob_Ritual lordJob_Ritual && lordJob_Ritual.selectedTarget.Thing is Building_TownSquare)
            {
                allowedSides = SpectateRectSide.Down;
            }
        }
    }
}
