using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    // Remove traits that conflict with each degree of the Mousekin Faith spectrum
    [HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new Type[] { typeof(PawnGenerationRequest) })]
    public static class Harmony_PawnGenerator_RemoveTraitsConflictingWithFaithSpectrum
    {
        static void Postfix(ref Pawn __result)
        {
            if (Utils.IsMousekin(__result))
            {
                int pawnFaithTraitDegree = __result.story.traits.DegreeOfTrait(MousekinDefOf.Mousekin_TraitSpectrum_Faith);

                if (pawnFaithTraitDegree > 0)
                {
                    var traitConflictResolverExt = MousekinDefOf.Mousekin_TraitSpectrum_Faith.GetModExtension<TraitConflictResolverExtension>();

                    traitConflictResolverExt.conflictingTraitsByFaithDegree.TryGetValue(pawnFaithTraitDegree, out List<TraitDef> conflictingTraits);

                    if (conflictingTraits != null)
                    {
                        __result.story.traits.allTraits.RemoveAll(x => conflictingTraits.Contains(x.def));
                    }
                }
            }
        }
    }
}
