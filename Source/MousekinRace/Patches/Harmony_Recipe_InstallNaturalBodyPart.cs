using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    // Upon successfully completing a natural Mousekin body part transplant surgery,
    // also apply the Elixir of Harmony hediff to the patient
    [HarmonyPatch(typeof(Recipe_InstallNaturalBodyPart), nameof(Recipe_InstallNaturalBodyPart.ApplyOnPawn))]
    public static class Harmony_Recipe_InstallNaturalBodyPart
    {
        static void Postfix(ref Recipe_InstallNaturalBodyPart __instance, Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (billDoer != null && !__instance.CheckSurgeryFail(billDoer, pawn, ingredients, part, bill) && __instance.recipe.GetModExtension<SurgeryExtraHediffExtension>() is SurgeryExtraHediffExtension modExt && modExt.extraHediff != null)
            { 
                Hediff hediff = HediffMaker.MakeHediff(modExt.extraHediff, pawn);
                hediff.Severity = modExt.severity;
                pawn.health.AddHediff(hediff);
            }
        }
    }
}
