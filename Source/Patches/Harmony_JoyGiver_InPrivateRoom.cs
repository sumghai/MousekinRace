using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using Verse.AI;

namespace MousekinRace.Patches
{
    [HarmonyPatch]
    public static class Harmony_JoyGiver_RestrictPrayerByFaithDegree
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(JoyGiver_InPrivateRoom), nameof(JoyGiver_InPrivateRoom.TryGiveJob));
            yield return AccessTools.Method(typeof(JoyGiver_InPrivateRoom), nameof(JoyGiver_InPrivateRoom.TryGiveJobWhileInBed));
        }

        static bool Prefix(Pawn pawn, ref Job __result)
        {           
            int pawnFaithTraitDegree = pawn.story.traits.DegreeOfTrait(MousekinDefOf.Mousekin_TraitSpectrum_Faith);

            // Prevent Apostates (1) and Devotionists (2) from ever praying
            // Nones (0), Pious (3) and Inquisitionists are allowed
            if (pawnFaithTraitDegree > 0 && pawnFaithTraitDegree < 3)
            {
                __result = null;
                return false;
            }
            return true;
        }
    }
}