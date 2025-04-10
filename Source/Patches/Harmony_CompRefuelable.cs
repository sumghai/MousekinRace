using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    // If a refuelable building also has a special ash maker comp,
    // generate ashes every time it consumes fuel
    [HarmonyPatch(typeof(CompRefuelable), nameof(CompRefuelable.ConsumeFuel))]
    public class Harmony_CompRefuelable_ConsumeFuel_GenerateAshes
    {
        static void Prefix(ref CompRefuelable __instance, float amount)
        {
            if (__instance.parent.TryGetComp<CompAshMaker>() is CompAshMaker compAshMaker)
            {
                compAshMaker.MakeAsh(amount);
            }
        }
    }
}
