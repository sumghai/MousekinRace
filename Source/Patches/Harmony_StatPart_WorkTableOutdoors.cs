using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace MousekinRace.Patches
{
    // Ignore outdoor work speed penalties for selected workbenches
    [HarmonyPatch(typeof(StatPart_WorkTableOutdoors), "Applies", new Type[] { typeof(ThingDef), typeof(Map), typeof(IntVec3) })]
    public class Harmony_StatPart_WorkTableOutdoors_IgnorePenalties
    {
        public static void Postfix(ref bool __result, ThingDef def)
        {
            __result = def.GetModExtension<BuildingIgnoreWorkSpeedPenaltiesExtension>() == null && __result;
        }
    }
}
