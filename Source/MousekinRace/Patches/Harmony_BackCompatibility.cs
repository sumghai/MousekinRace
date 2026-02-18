using HarmonyLib;
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
                // If Medieval Overhaul is active, replace selected Mousekin thingDefs with their VCE counterparts
                if (ModCompatibility.MedievalOverhaulIsActive) 
                {
                    switch (defName)
                    {
                        case "Mousekin_Plant_Wheat":
                            __result = "DankPyon_Plant_Wheat";
                            break;
                        case "Mousekin_RawWheat":
                            __result = "DankPyon_RawWheat";
                            break;
                        case "Mousekin_Flour":
                            __result = "DankPyon_Flour";
                            break;
                    }
                }
                // If VCE is active, replace selected Mousekin thingDefs with their VCE counterparts
                // (VCE is lower in precedence, as Medieval Overhaul renames VCE Wheat into Spelt, while still using
                //  MO's own wheat and flour defs for the majority of its recipes)
                else if (ModCompatibility.VanillaCookingExpandedIsActive)
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
                // Otherwise, revert to Mousekin thingDefs if both Medieval Overhaul and VCE are disabled
                else
                {
                    switch (defName)
                    {
                        case "DankPyon_Plant_Wheat":
                        case "VCE_Wheat":
                            __result = "Mousekin_Plant_Wheat";
                            break;
                        case "DankPyon_Flour":
                        case "VCE_Flour":
                            __result = "Mousekin_Flour";
                            break;
                    }
                }
            }
        }
    }
}