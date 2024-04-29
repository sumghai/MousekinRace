using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Reflection;

namespace MousekinRace
{
    // Disable "Offer gift" float menu option for settlements belonging to enemies of the player-chosen allegiance faction
    // Works for both caravans and transport pods
    [HarmonyPatch]
    public static class Harmony_CaravanArrivalAction_OfferGifts_CanOfferGiftsTo_DisableIfAllegianceSet
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(CaravanArrivalAction_OfferGifts), nameof(CaravanArrivalAction_OfferGifts.CanOfferGiftsTo));
            yield return AccessTools.Method(typeof(TransportPodsArrivalAction_GiveGift), nameof(TransportPodsArrivalAction_GiveGift.CanGiveGiftTo));
        }
        static void Postfix(ref FloatMenuAcceptanceReport __result, Settlement settlement)
        {
            if (AllegianceSys_Utils.IsEnemyBecauseOfAllegiance(Faction.OfPlayer, settlement.Faction))
            {
                __result = false;
            }
        }
    }
}
