using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace MousekinRace
{
    // Ignore outdoor work speed penalties for selected workbenches
    [HarmonyPatch(typeof(StatPart_WorkTableTemperature), "Applies", new Type[] { typeof(ThingDef), typeof(Map), typeof(IntVec3) })]
    public class Harmony_StatPart_WorkTableTemperature_IgnorePenalties
    {
        public static void Postfix(ref bool __result, ThingDef tDef)
        {
            __result = tDef.GetModExtension<BuildingIgnoreWorkSpeedPenaltiesExtension>() == null && __result;
        }
    }
}
