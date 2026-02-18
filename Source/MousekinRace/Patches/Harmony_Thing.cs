using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace MousekinRace
{
    // Refrigerate items in Root Cellars
    [HarmonyPatch(typeof(Thing), nameof(Thing.AmbientTemperature), MethodType.Getter)]
    public static class Harmony_Thing_AmbientTemperature_RefrigerateRootCellarContents
    {
        static void Postfix(Thing __instance, ref float __result)
        {
            if (Building_CellarOutdoor.ThingIsInCellar(__instance))
            {
                __result = MousekinRaceMod.Settings.RootCellarTemperature;
            }
        }
    }

    // Hide items stored in Root Cellars or storage buildings with the CompStorageHiddenContents comp from rendering on the map
    [HarmonyPatch]
    public static class Harmony_Thing_DrawGUIOverlay_HideRootCellarContents
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Apparel), nameof(Apparel.DrawAt));
            yield return AccessTools.Method(typeof(Book), nameof(Book.DrawAt));
            yield return AccessTools.Method(typeof(Thing), nameof(Thing.DrawGUIOverlay));
            yield return AccessTools.Method(typeof(Thing), nameof(Thing.Print));
            yield return AccessTools.Method(typeof(ThingWithComps), nameof(ThingWithComps.DrawGUIOverlay));
            yield return AccessTools.Method(typeof(ThingWithComps), nameof(ThingWithComps.Print));
            yield return AccessTools.Method(typeof(MinifiedThing), nameof(MinifiedThing.Print));
        }

        static bool Prefix(Thing __instance)
        {
            if (Building_CellarOutdoor.ThingIsInCellar(__instance) || CompStorageHiddenContents.ThingIsInStorage(__instance))
            {
                return false;
            }
            return true;
        }
    }

    // Add custom throught bubbles when Mousekins eat cheese
    [HarmonyPatch(typeof(Thing), nameof(Thing.Ingested))]
    public static class Harmony_Toils_Ingest_AddThoughtBubblesWhenMousekinEatsCheese
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
