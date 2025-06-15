using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    // Disable "Show what will buy" gizmo for settlements belonging to enemies of the player-chosen allegiance faction, and show the reason
    [HarmonyPatch(typeof(Settlement), nameof(Settlement.GetGizmos))]
    public static class Harmony_Settlement_GetGizmos_DisableIfAllegianceSet
    {
        static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Settlement __instance)
        {
            foreach(Gizmo gizmo in __result) 
            {
                if (gizmo is Command_Action command_Action && command_Action.defaultLabel == "CommandShowSellableItems".Translate() && GameComponent_Allegiance.Instance.alignedFaction is Faction allegianceFaction && allegianceFaction.def.GetModExtension<AlliableFactionExtension>() is AlliableFactionExtension allegianceFactionExtension && allegianceFactionExtension.hostileToFactionTypes.Contains(__instance.Faction.def))
                { 
                    command_Action.disabled = true;
                    command_Action.disabledReason = "MousekinRace_CommandShowSellableItemsDesc_DisabledReason".Translate(AllegianceSys_Utils.MembershipToFactionLabel(allegianceFaction));
                }
                yield return gizmo;
            }
        }
    }
}
