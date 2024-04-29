using HarmonyLib;
using RimWorld;

namespace MousekinRace
{
    // Disable goodwill display for the player-chosen allegiance faction and their specified enemies
    [HarmonyPatch(typeof(Faction), nameof(Faction.HasGoodwill), MethodType.Getter)]
    public class Harmony_Faction_HasGoodwill_DisableIfAllegianceSet
    {
        static void Postfix(ref Faction __instance, ref bool __result)
        {
            if (GameComponent_Allegiance.Instance.alignedFaction is Faction allegianceFaction && (__instance.def == allegianceFaction.def || (allegianceFaction.def.GetModExtension<AlliableFactionExtension>() is AlliableFactionExtension facExt && facExt.hostileToFactionTypes.Contains(__instance.def))))
            {
                __result = false;
            }
        }
    }

    // Disallow goodwill changes for the player-chosen allegiance faction and their specified enemies
    [HarmonyPatch(typeof(Faction), nameof(Faction.CanChangeGoodwillFor))]
    public class Harmony_Faction_CanChangeGoodwillFor_DisableIfAllegianceSet
    {
        public static void Postfix(Faction __instance, Faction other, ref bool __result)
        {
            __result = __result && !AllegianceSys_Utils.IsEnemyBecauseOfAllegiance(__instance, other);
        }
    }
}
