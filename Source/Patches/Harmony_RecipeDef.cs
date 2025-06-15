using HarmonyLib;
using Verse;

namespace MousekinRace
{
    // Restrict crafting of faction-unique items (apparel, weapons etc.) to players whose allegiance matches the item's associated faction
    [HarmonyPatch(typeof(RecipeDef), nameof(RecipeDef.AvailableNow), MethodType.Getter)]
    public static class Harmony_RecipeDef_AvailableNow_RestrictCraftinRecipesByAllegiance
    {
        static void Postfix(RecipeDef __instance, ref bool __result)
        {
            ThingDef currentDef = __instance.ProducedThingDef;
            if (MousekinDefOf.FactionRestrictedThingDefs.Contains(currentDef))
            {
                if (GameComponent_Allegiance.Instance.alignedFaction == null || !GameComponent_Allegiance.Instance.alignedFaction.def.GetModExtension<AlliableFactionExtension>().factionRestrictedCraftableThingDefs.Contains(currentDef))
                {
                    __result = false;
                }
            }
        }
    }
}
