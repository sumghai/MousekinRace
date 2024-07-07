using HarmonyLib;
using Verse;
using Verse.AI;

namespace MousekinRace.Patches
{
    // If the destination of a pawn's path is a root cellar, always end it on the interaction cell
    [HarmonyPatch(typeof(Pawn_PathFollower), nameof(Pawn_PathFollower.StartPath))]
    public static class Harmony_Pawn_PathFollower_StartPath_RootCellarAlwaysInteractionCell
    {
        static void Prefix(LocalTargetInfo dest, ref PathEndMode peMode)
        {
            if (dest.Thing is Building_StorageCellar) 
            {
                peMode = PathEndMode.InteractionCell;
            }
        }
    }
}
