﻿using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.Ingested))]
    public static class Harmony_Toils_Ingest
    {
        public static void Postfix(Pawn ingester, Thing __instance)
        {
            if (Utils.IsMousekin(ingester) && ingester.TryGetComp<CompRemembersCheeseEaten>() is CompRemembersCheeseEaten comp && comp != null)
            {
                if (comp.Props.cheeseDefs.Contains(__instance.def))
                {
                    float moodOffset = comp.EatCheese();

                    if (ingester.Spawned)
                    {
                        MoteBubble moteBubble = (MoteBubble)ThingMaker.MakeThing((moodOffset > 0f) ? ThingDefOf.Mote_ThoughtGood : ThingDefOf.Mote_ThoughtBad);
                        moteBubble.SetupMoteBubble(MousekinDefOf.Mousekin_Thought_AteCheese.Icon, ingester);
                        moteBubble.Attach(ingester);
                        GenSpawn.Spawn(moteBubble, ingester.Position, ingester.Map);
                    }
                }
            }
        }
    }
}
