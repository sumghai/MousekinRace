using HarmonyLib;
using ItemProcessor;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Verse;

namespace MousekinRace
{
    [HarmonyPatch(typeof(BackCompatibility), nameof(BackCompatibility.BackCompatibleDefName))]
    public static class Harmony_BackCompatibility_BackCompatibleDefName_CrossModCompatPatch
    {
        public static void Postfix(Type defType, string defName, ref string __result)
        {
            if (defType == typeof(ThingDef))
            {
                // IF VCE is active, replace selected Mousekin thingDefs with their VCE counterparts
                if (ModCompatibility.VanillaCookingExpandedIsActive)
                {
                    switch (defName)
                    {
                        case "Mousekin_Plant_Wheat":
                            __result = "VCE_Wheat";
                            break;
                        case "Mousekin_RawWheat":
                        case "Mousekin_Flour":
                            __result = "VCE_Flour";
                            break;
                    }
                }
                // Otherwise, revert to Mousekin thingDefs if VCE is disabled/uninstalled
                else
                {
                    switch (defName)
                    {
                        case "VCE_Wheat":
                            __result = "Mousekin_Plant_Wheat";
                            break;
                        case "VCE_Flour":
                            __result = "Mousekin_Flour";
                            break;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(BackCompatibility), nameof(BackCompatibility.GetBackCompatibleType))]
    public static class Harmony_BackCompatibility_GetBackCompatibleType_CrossModCompatPatch
    {
        public static void Postfix(Type baseType, string providedClassName, XmlNode node, ref Type __result)
        {
            if (baseType == typeof(Thing) && node["def"] != null && node["def"].InnerText == "Mousekin_Windmill")
            {
                // If VCE is active, disable ItemProcessor of oldWindmill
                if (ModCompatibility.VanillaCookingExpandedIsActive)
                {
                    __result = typeof(Building);
                }
                // Otherwise, restore ItemProcessor if VCE is disabled/uninstalled
                else
                {
                    __result = typeof(ItemProcessor.Building_ItemProcessor);
                }
            }
        }
    }

    [HarmonyPatch(typeof(BackCompatibility), nameof(BackCompatibility.PostCheckSpawnBackCompatibleThingAfterLoading))]
    public static class Harmony_BackCompatibility_PostCheckSpawnBackCompatibleThingAfterLoading_CrossModCompatPatch
    {
        public static void Postfix(Map map)
        {
            // If VCE is not active, check if any windmills have improperly-configured Item Processors,
            // and if necessary spawn a replacement with the original thingID, health and faction
            if (!ModCompatibility.VanillaCookingExpandedIsActive)
            {
                List<Thing> windmills = map.listerThings.AllThings.Where(t => t.def == MousekinDefOf.Mousekin_Windmill).ToList();
                if (!windmills.NullOrEmpty())
                {
                    foreach (Building_ItemProcessor oldWindmill in windmills.Cast<Building_ItemProcessor>())
                    {
                        if (oldWindmill.innerContainerFirst == null)
                        {
                            string oldID = oldWindmill.GetUniqueLoadID();
                            IntVec3 oldPosition = oldWindmill.Position;
                            int oldHitPoints = oldWindmill.HitPoints;
                            Faction oldFaction = oldWindmill.Faction;

                            Thing newWindmill = ThingMaker.MakeThing(MousekinDefOf.Mousekin_Windmill);
                            newWindmill.HitPoints = oldHitPoints;
                            newWindmill.SetFaction(oldFaction);
                            oldWindmill.Destroy();
                            newWindmill.ThingID = oldID;
                            GenSpawn.Spawn(newWindmill, oldPosition, map);
                        }
                    }
                }
            }
        }
    }
}