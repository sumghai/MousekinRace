using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;

namespace MousekinRace
{
    // Skip generating quest sites from the Shattered Empire if the player has pledged allegiance to a Mousekin faction
    [HarmonyPatch(typeof(SiteMakerHelper), nameof(SiteMakerHelper.FactionCanOwn), new Type[] { typeof(SitePartDef), typeof(Faction), typeof(bool), typeof(Predicate<Faction>) })]
    public static class Harmony_SiteMakerHelper_FactionCanOwn_SkipEmpireIfAllegianceSet
    {
        public static void Postfix(ref bool __result, Faction faction)
        {
            if (GameComponent_Allegiance.Instance.alignedFaction != null && faction == Faction.OfEmpire)
            {
                __result = false;
            }
        }
    }
}
