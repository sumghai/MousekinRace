using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    // Used fixed symbols for Mousekin ideologies
    [HarmonyPatch(typeof(IdeoFoundation), nameof(IdeoFoundation.GetRandomIconDef))]
    public static class Harmony_IdeoFoundation_GetRandomIconDef_LimitToMatchingCultures
    {
        public static void Postfix(Ideo ideo, ref IdeoIconDef __result)
        {
            if (ideo.culture?.GetModExtension<IdeoSymbolIgnoreRandomExtension>() != null)
            {
                IEnumerable<IdeoIconDef> icons = DefDatabase<IdeoIconDef>.AllDefs.Where(x => x.cultures != null && x.cultures.Contains(ideo.culture));
                if (icons.EnumerableCount() > 0)
                { 
                    __result = icons.RandomElement();
                }
            }
        }
    }

    // Prevent Ancients factions from using Mousekin cultures
    [HarmonyPatch(typeof(IdeoFoundation), nameof(IdeoFoundation.RandomizeCulture))]
    public static class Harmony_IdeoFoundation_RandomizeCulture_HideMousekinCulturesFromCulturelessFactions
    {
        public static void Postfix(ref IdeoFoundation __instance, IdeoGenerationParms parms)
        {
            if (parms.forFaction.allowedCultures == null && __instance.ideo.culture.IsMousekin())
            {
                __instance.ideo.culture = DefDatabase<CultureDef>.AllDefsListForReading.Where(x => !x.defName.Contains("Mousekin")).RandomElement();
            }
        }
    }

    // Disallow animal and weapon precepts for Mousekin ideologies
    [HarmonyPatch(typeof(IdeoFoundation), nameof(IdeoFoundation.RandomizePrecepts))]
    public static class Harmony_IdeoFoundation_RandomizePrecepts_NoVeneratedAnimalsAndNobleDespisedWeaponsForMousekin
    {
        public static void Postfix(ref IdeoFoundation __instance)
        {
            if (__instance.ideo.culture.IsMousekin())
            {
                __instance.ideo.precepts.RemoveAll(x => x.def == PreceptDefOf.AnimalVenerated || x.def == PreceptDefOf.NobleDespisedWeapons);
            }
        }
    }

    // Curate preferred xenotype precepts for Mousekin ideologies
    // (instead of letting RimWorld randomly decide)
    [HarmonyPatch(typeof(IdeoFoundation), nameof(IdeoFoundation.RandomizePrecepts))]
    public static class Harmony_IdeoFoundation_RandomizePrecepts_CleanUpPreferredXenotypePreceptsForMousekin
    {
        public static void Postfix(ref IdeoFoundation __instance)
        {
            // Only for Mousekin ideos and when the Biotech DLC is active
            if (__instance.ideo.culture.IsMousekin() && ModsConfig.BiotechActive)
            {
                // Start by removing all randomly-generated preferred xenotype precepts
                __instance.ideo.precepts.RemoveAll(x => x.def == PreceptDefOf.PreferredXenotype);

                // Add the preferred xenotype precept to ideos with the Rodentkind Purity or Nutsnatching memes
                if (__instance.ideo.HasMeme(MousekinDefOf.Mousekin_IdeoMeme_RodentPurity) || __instance.ideo.HasMeme(MousekinDefOf.Mousekin_IdeoMeme_Raider))
                {
                    Precept_Xenotype precept_Xenotype = (Precept_Xenotype)PreceptMaker.MakePrecept(PreceptDefOf.PreferredXenotype);
                    precept_Xenotype.xenotype = MousekinDefOf.Mousekin_XenotypeMousekin;
                    __instance.ideo.AddPrecept(precept_Xenotype);
                }
            }
        }
    }
}
