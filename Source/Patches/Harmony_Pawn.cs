using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace MousekinRace
{
    // Pawns leaving the map while experiencing the "Leaving colony due to allegiance change" mental state
    // should change their faction to the enemy of the player-chosen allegiance faction
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.ExitMap))]
    public static class Harmony_Pawn_ExitMap_DissidentPawnsChangeFactionIfAllegianceSet
    {
        static void Prefix(Pawn __instance)
        { 
            if (__instance.IsColonist && __instance.MentalStateDef == MousekinDefOf.Mousekin_MentalState_ExitAfterAllegianceChange) 
            {
                AllegianceSys_Utils.SetFactionToOpposingAllegiance(__instance);
            }
        }
    }

    // Ensure that when heretics being executed in the Purging Flames ritual die, their bodies are quickly destroyed by fire
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.Kill))]
    public static class Harmony_Pawn_Kill_foobar
    {
        static void Prefix(Pawn __instance, ref bool __state)
        {
            __state = (__instance.GetLord()?.LordJob as LordJob_Ritual_HereticExecution)?.heretics.Contains(__instance) ?? false;
        }
        
        static void Postfix(Pawn __instance, bool __state)
        {
            if (__instance.Corpse != null && __state) 
            {
                __instance.equipment?.DestroyAllEquipment();
                __instance.inventory?.DestroyAll();
                __instance.apparel?.DestroyAll();
                __instance.Corpse.HitPoints = 0;
                __instance.Corpse.GetComp<CompRottable>().RotProgress = 9999999f;
                FireUtility.TryStartFireIn(__instance.Corpse.Position, __instance.Corpse.Map, Fire.MaxFireSize, null);
            }
        }
    }
}
