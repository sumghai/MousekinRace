using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    // Conditionally patch moral guide title for Mousekin player faction
    // for ideo role loss mental break
    [HarmonyPatch(typeof(MentalState_IdeoChange), nameof(MentalState_IdeoChange.GetBeginLetterText))]
    public static class Harmony_MentalState_IdeoChange_GetBeginLetterText_ReplaceRoleTitlesForMousekinPlayer
    {
        static void Postfix(ref TaggedString __result, ref MentalState_IdeoChange __instance)
        {
            CultureDef oldIdeoCultureDef = __instance.oldIdeo.culture;
            if (__instance.pawn.Faction.IsPlayer && oldIdeoCultureDef.IsMousekinKingdomLike() && __instance.changedIdeo && __instance.oldRole != null && __instance.oldRole.def == PreceptDefOf.IdeoRole_Moralist)
            { 
                TaggedString temp = __result;
                __result = temp.Replace(__instance.oldRole.LabelCap, MousekinDefOf.MousekinPriest.label.Replace(MousekinDefOf.Mousekin.label, "").Trim().CapitalizeFirst());
            }
        }
    }
}
