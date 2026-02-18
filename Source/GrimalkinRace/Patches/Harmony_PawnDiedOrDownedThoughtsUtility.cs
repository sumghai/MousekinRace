using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace GrimalkinRace
{
    // Add a positive memory/thought to any Grimalkins who have just killed a Mousekin for any reason
    [HarmonyPatch(typeof(PawnDiedOrDownedThoughtsUtility), nameof(PawnDiedOrDownedThoughtsUtility.AppendThoughts_ForHumanlike))]
    public static class Harmony_PawnDiedOrDownedThoughtsUtility_AppendThoughts_ForHumanlike_GrimalkinKillingMousekin
    {
        static void Postfix(Pawn victim, DamageInfo? dinfo, ref PawnDiedOrDownedThoughtsKind thoughtsKind, ref List<IndividualThoughtToAdd> outIndividualThoughts, List<ThoughtToAddToAll> outAllColonistsThoughts)
        {
            bool isExecution = dinfo.HasValue && dinfo.Value.Def.execution;

            if (dinfo.HasValue && dinfo.Value.Def.ExternalViolenceFor(victim) && dinfo.Value.Instigator != null && dinfo.Value.Instigator is Pawn)
            {
                Pawn pawn = (Pawn)dinfo.Value.Instigator;

                if (!pawn.Dead && pawn.needs.mood != null && pawn.story != null && pawn != victim && PawnUtility.ShouldGetThoughtAbout(pawn, victim) && pawn.IsGrimalkin())
                {
                    if (thoughtsKind == PawnDiedOrDownedThoughtsKind.Died && victim.IsMousekin())
                    {
                        outIndividualThoughts.Add(new IndividualThoughtToAdd(GrimalkinDefOf.Grimalkin_Thought_KilledMousekin, pawn));
                    }
                }
            }
        }
    }
}
