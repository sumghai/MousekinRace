﻿using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace MousekinRace
{
    // Force Mousekin-unique animals to use name generators if purchased by a Mousekin
    [HarmonyPatch]
    public class Harmony_GiveSoldThingToPlayer_MousekinPurchasedAnimalForceNameGen
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Caravan_TraderTracker), nameof(Caravan_TraderTracker.GiveSoldThingToPlayer));
            yield return AccessTools.Method(typeof(Pawn_TraderTracker), nameof(Pawn_TraderTracker.GiveSoldThingToPlayer));
            yield return AccessTools.Method(typeof(Settlement_TraderTracker), nameof(Settlement_TraderTracker.GiveSoldThingToPlayer));
            yield return AccessTools.Method(typeof(TradeShip), nameof(TradeShip.GiveSoldThingToPlayer));
        }

        static void Prefix(Thing toGive, Pawn playerNegotiator)
        {
            if (Utils.GetRaceOfFaction(playerNegotiator.Faction.def) == MousekinDefOf.Mousekin)
            {
                if (toGive is Pawn purchasedPawn && purchasedPawn.RaceProps.Animal)
                {
                    if (purchasedPawn.Name == null && purchasedPawn.kindDef.HasModExtension<NameAnimalOnTameExtension>())
                    {
                        purchasedPawn.Name = PawnBioAndNameGenerator.GeneratePawnName(purchasedPawn, NameStyle.Full, null);
                        Messages.Message("MousekinRace_MessagePurchasedAnimalNamed".Translate(purchasedPawn.def.label, purchasedPawn.Name.ToString()), purchasedPawn, MessageTypeDefOf.PositiveEvent, true);
                    }
                }
            }
        }
    }
}