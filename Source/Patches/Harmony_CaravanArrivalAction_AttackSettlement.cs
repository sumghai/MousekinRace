using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Reflection;

namespace MousekinRace
{
    // Disable "Attack settlement" float menu option for settlements belonging to the player-chosen allegiance faction
    // Works for both caravans and transport pods
    [HarmonyPatch]
    public static class Harmony_CaravanArrivalAction_AttackSettlement_CanAttack_DisableIfAllegianceSet
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(CaravanArrivalAction_AttackSettlement), nameof(CaravanArrivalAction_AttackSettlement.CanAttack));
            yield return AccessTools.Method(typeof(TransportersArrivalAction_AttackSettlement), nameof(TransportersArrivalAction_AttackSettlement.CanAttack));
        }
        static void Postfix(ref FloatMenuAcceptanceReport __result, Settlement settlement)
        {
            /*if (GameComponent_Allegiance.Instance.alignedFaction is Faction allegianceFaction && settlement.Faction == allegianceFaction)
            {
                __result = false;
            }*/
        }
    }
}
