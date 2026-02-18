using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    // Conditionally patch moral guide role title for Mousekin player faction
    // in the "[x] role unfilled" alert

    // Patch alert title
    [HarmonyPatch(typeof(Alert_RolesEmpty), nameof(Alert_RolesEmpty.GetLabel))]
    public static class Harmony_Alert_RolesEmpty_GetLabel_ReplaceRoleTitlesForMousekinPlayer
    {
        static void Postfix(List<Precept_Role> ___emptyRoles, ref string __result)
        {
            if (___emptyRoles.Count == 1 && ___emptyRoles[0].def == PreceptDefOf.IdeoRole_Moralist && Faction.OfPlayer.ideos.PrimaryIdeo.culture.IsMousekinKingdomLike())
            { 
                __result = "IdeoRoleEmpty".Translate(MousekinDefOf.MousekinPriest.label.Replace(MousekinDefOf.Mousekin.label, "").Trim().CapitalizeFirst());
            }
        }
    }

    // Patch alert description
    [HarmonyPatch(typeof(Alert_RolesEmpty), nameof(Alert_RolesEmpty.GetExplanation))]
    public static class Harmony_Alert_RolesEmpty_GetExplanation_ReplaceRoleTitlesForMousekinPlayer
    {
        static void Postfix(List<Precept_Role> ___emptyRoles, ref TaggedString __result)
        {
            if (Faction.OfPlayer.ideos.PrimaryIdeo.culture.IsMousekinKingdomLike())
            {
                TaggedString output = __result;

                TaggedString moralGuideTitleToReplace = Find.FactionManager.AllFactions.FirstOrDefault(f => f.ideos.PrimaryCulture.IsMousekinKingdomLike()).ideos.PrimaryIdeo.precepts.FirstOrDefault(p => p.def == PreceptDefOf.IdeoRole_Moralist).name;

                __result = output.Replace(moralGuideTitleToReplace, MousekinDefOf.MousekinPriest.label.Replace(MousekinDefOf.Mousekin.label, "").Trim().CapitalizeFirst());
            }
        }
    }
}