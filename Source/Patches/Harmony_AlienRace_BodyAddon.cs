using AlienRace;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace.Patches
{
    [HarmonyPatch(typeof(AlienPartGenerator.BodyAddon), nameof(AlienPartGenerator.BodyAddon.CanDrawAddon))]
    public static class Harmony_AlienRace_BodyAddon_CanDrawAddon_HideUnderApparelWithAttachedHeadgear
    {
        static void Postfix(AlienPartGenerator.BodyAddon __instance, Pawn pawn, ref bool __result)
        {
            if (pawn.apparel?.WornApparel?.Find(ap => ap.TryGetComp<CompApparelWithAttachedHeadgear>() is CompApparelWithAttachedHeadgear comp) is Apparel apparelWithAttachedHeadgear && apparelWithAttachedHeadgear != null)
            {
                if (apparelWithAttachedHeadgear.GetComp<CompApparelWithAttachedHeadgear>().Props.attachedHeadgearDef.apparel.tags.Any(s => __instance.hiddenUnderApparelTag.Contains(s)))
                {
                    __result = __result && !apparelWithAttachedHeadgear.GetComp<CompApparelWithAttachedHeadgear>().isHatOn;
                }
            }
        }
    }
}
