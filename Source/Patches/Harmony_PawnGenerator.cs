using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    // Curate generation of Mousekin pawns
    // - Ensure Mousekin Slaves always only join as slaves
    // - Remove traits that conflict with each degree of the Mousekin Faith spectrum
    [HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new Type[] { typeof(PawnGenerationRequest) })]
    public static class Harmony_PawnGenerator_GeneratePawn_CurateForMousekins
    {
        static void Postfix(ref Pawn __result)
        {
            if (Utils.IsMousekin(__result))
            {
                // Mousekin slaves
                if (ModsConfig.IdeologyActive && __result.kindDef == MousekinDefOf.MousekinSlave)
                { 
                    __result.guest.joinStatus = JoinStatus.JoinAsSlave;
                }
                
                // Mousekin Faith trait spectrum cleanup
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

    // Ensure that any Mousekin Slaves generated:
    // - is spawned wearing their default slave rag apparel (instead of being given random clothing for warm)
    // - has the Word of Valerian ideo (if the Mousekin Kingdom faction exists on the map)
    // - child slaves (i.e. those with no adulthoods) should always have the generic Mousekin Slave Child backstory
    [HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.TryGenerateNewPawnInternal))]
    public static class Harmony_PawnGenerator_TryGenerateNewPawnInternal_CurateForMousekinSlaves
    {
        static void Prefix(ref PawnGenerationRequest request)
        {
            if (request.KindDef == MousekinDefOf.MousekinSlave)
            {
                request.ForceAddFreeWarmLayerIfNeeded = false;

                if (ModsConfig.IdeologyActive && Find.IdeoManager.ideos.First(x => x.culture.IsMousekinKingdomLike()) is Ideo mousekinDefaultIdeo)
                {
                    request.FixedIdeo = mousekinDefaultIdeo;
                }
            }
        }

        static void Postfix(ref Pawn __result, ref PawnGenerationRequest request) 
        {
            if (request.KindDef == MousekinDefOf.MousekinSlave && __result.story.adulthood == null)
            {
                __result.story.Childhood = MousekinDefOf.Mousekin_Childhood_SlaveChild;
                // Regenerate traits and skills as required
                PawnGenerator.GenerateTraits(__result, request);
                PawnGenerator.GenerateSkills(__result, request);
            }
        }
    }
}
