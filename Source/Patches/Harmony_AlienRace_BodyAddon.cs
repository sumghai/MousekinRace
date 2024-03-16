using AlienRace;
using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace MousekinRace
{
    // Conditionally hide body addons (e.g. ears) if a hooded headgear is worn
    [HarmonyPatch(typeof(AlienPartGenerator.BodyAddon), nameof(AlienPartGenerator.BodyAddon.CanDrawAddon), new Type[] { typeof(Pawn) }, new ArgumentType[] { ArgumentType.Normal })]
    public static class Harmony_AlienRace_BodyAddon_CanDrawAddon_HideUnderApparelWithAttachedHeadgear
    {
        static void Postfix(AlienPartGenerator.BodyAddon __instance, Pawn pawn, ref bool __result)
        {
            if (pawn.apparel?.WornApparel?.Find(ap => ap.TryGetComp<CompApparelWithAttachedHeadgear>() is CompApparelWithAttachedHeadgear comp) is Apparel apparelWithAttachedHeadgear && apparelWithAttachedHeadgear != null)
            {
                /*if (apparelWithAttachedHeadgear.GetComp<CompApparelWithAttachedHeadgear>().Props.attachedHeadgearDef.apparel.tags.Any(s => __instance.hiddenUnderApparelTag.Contains(s)))
                {
                    __result = __result && !apparelWithAttachedHeadgear.GetComp<CompApparelWithAttachedHeadgear>().isHatOn;
                }*/
            }
        }
    }
}
