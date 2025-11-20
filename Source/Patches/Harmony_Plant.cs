using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    [HarmonyPatch(typeof(Plant), nameof(Plant.Kill))]
    public static class Harmony_Plant_Kill_NotifyFlowerDestroyed
    {
        static void Prefix(Plant __instance, DamageInfo? dinfo)
        {
            if (__instance != null && dinfo.HasValue) 
            { 
                __instance.Map?.GetComponent<MapComponent_FlowerTracker>()?.Notify_FlowerDestroyed(__instance, dinfo.Value);
            }
        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.SpawnSetup))]
    public static class Harmony_Plant_SpawnSetup_NotifyFlowerPlanted
    {
        static void Postfix(Plant __instance, bool respawningAfterLoad)
        {
            if (__instance != null && !respawningAfterLoad)
            {
                __instance.Map?.GetComponent<MapComponent_FlowerTracker>()?.Notify_FlowerPlanted(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.DeSpawn))]
    public static class Harmony_Plant_SpawnSetup_NotifyFlowerDespawned
    {
        static void Prefix(Plant __instance)
        {
            if (__instance != null && __instance.Spawned)
            {
                __instance.Map?.GetComponent<MapComponent_FlowerTracker>()?.Notify_FlowerDestroyed(__instance, null);
            }
        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.PlantCollected))]
    public static class Harmony_Plant_PlantCollected_ResetSoilFertility
    {
        static void Prefix(Plant __instance, out Map __state)
        { 
            __state = __instance.Map;
        }

        static void Postfix(Plant __instance, Map __state) 
        {
            if (__state.GetComponent<MapComponent_Fertilizer>().cellFertilityBonus.ContainsKey(__instance.positionInt) && __instance.def.plant.harvestYield != 0)
            { 
                __state.GetComponent<MapComponent_Fertilizer>().cellFertilityBonus.Remove(__instance.positionInt);
            }
        }
    }
}
