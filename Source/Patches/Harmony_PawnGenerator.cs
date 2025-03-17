using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    // Remove traits that conflict with each degree of the Mousekin Faith spectrum
    [HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new Type[] { typeof(PawnGenerationRequest) })]
    public static class Harmony_PawnGenerator_GeneratePawn_RemoveTraitsConflictingWithFaithSpectrum
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

    // Ensure that any Mousekin Slaves generated has the Word of Valerian ideo
    // (if the Mousekin Kingdom faction exists on the map)
    [HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.TryGenerateNewPawnInternal))]
    public static class Harmony_PawnGenerator_TryGenerateNewPawnInternal_OverrideIdeoForMousekinSlaves
    {
        static void Prefix(ref PawnGenerationRequest request)
        {
            if (ModsConfig.IdeologyActive && request.KindDef == MousekinDefOf.MousekinSlave &&  Find.IdeoManager.ideos.First(x => x.culture.IsMousekinKingdomLike()) is Ideo mousekinDefaultIdeo)
            {
                request.FixedIdeo = mousekinDefaultIdeo;
            }
        }
    }
}
