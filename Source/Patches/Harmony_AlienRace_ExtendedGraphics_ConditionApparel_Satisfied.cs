using AlienRace;
using AlienRace.ExtendedGraphics;
using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace MousekinRace
{
    // Hide Humanoid Alien Race body addons if their hiddenUnderApparelTag matches the toggleable hood's own tags, and the hood is enabled
    [HarmonyPatch(typeof(ConditionApparel), nameof(ConditionApparel.Satisfied))]
    public static class Harmony_AlienRace_ExtendedGraphics_ConditionApparel_Satisfied_HideTaggedBodyAddonsUnderHood
    {
        static void Postfix(ConditionApparel __instance, ref bool __result, ExtendedGraphicsPawnWrapper pawn, ref ResolveData data)
        {
            if (__result && pawn.WrappedPawn.apparel != null && ((!AlienRenderTreePatches.IsPortrait(pawn.WrappedPawn) && pawn.VisibleInBed()) || (AlienRenderTreePatches.IsPortrait(pawn.WrappedPawn) && !Prefs.HatsOnlyOnMap)))
            {
                if (pawn.WrappedPawn.apparel.WornApparel.Exists(ap => ap.GetComp<CompApparelWithAttachedHeadgear>() is CompApparelWithAttachedHeadgear comp && comp.Props.hideHarBodyAddonsWithTag.Any(s => __instance.hiddenUnderApparelTag.Contains(s) && comp.isHatOn)))
                {
                    __result = false;
                }
            }            
        }
    }
}
