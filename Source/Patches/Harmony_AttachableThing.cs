using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace MousekinRace
{
    // Heretics being burned at the stake during the Purging Flames heretic execution ritual
    // should have their fire attachment render position shifted slightly downwards
    [HarmonyPatch(typeof(AttachableThing), nameof(AttachableThing.DrawPos), MethodType.Getter)]
    public class Harmony_AttachableThing_DrawPos_OffsetFireForBurningHeretics
    {
        public static void Postfix(AttachableThing __instance, Thing ___parent, ref Vector3 __result)
        {
            if (__instance is Fire && ___parent is Pawn pawn && pawn.GetLord()?.LordJob is LordJob_Ritual_HereticExecution hereticExecution && hereticExecution.heretics.Contains(pawn))
            {
                __result.z -= 0.45f;
            }
        }
    }
}
