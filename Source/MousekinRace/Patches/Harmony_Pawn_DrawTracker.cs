using HarmonyLib;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    [HarmonyPatch(typeof(Pawn_DrawTracker), nameof(Pawn_DrawTracker.DrawPos), MethodType.Getter)]
    public static class Pawn_DrawTracker_DrawPos_OffsetPawnsWorkingInsideMineEntrance
    {
        public static void Postfix(ref Vector3 __result, ref Pawn ___pawn)
        {
            if (___pawn?.Map != null && ___pawn.Spawned && ___pawn.jobs?.curDriver?.CurToilString == "Mousekin_Toil_MineResources")
            {
                Building_MineEntrance mineEntrance = (___pawn.jobs.curDriver as JobDriver_MineResourcesFromMineEntrance).MineEntrance;

                __result = (mineEntrance.Position).ToVector3() + new Vector3(0.5f, 0f);
                return;
            }

        }
    }
}
