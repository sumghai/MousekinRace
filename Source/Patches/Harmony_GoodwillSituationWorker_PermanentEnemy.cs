using HarmonyLib;
using RimWorld;

namespace MousekinRace
{
    // Mark enemies of the player-chosen allegiance faction as permanent enemies
    [HarmonyPatch(typeof(GoodwillSituationWorker_PermanentEnemy), nameof(GoodwillSituationWorker_PermanentEnemy.ArePermanentEnemies))]
    public class Harmony_GoodwillSituationWorker_PermanentEnemy_IsEnemyBecauseOfAllegiance
    {
        /*static void Postfix(Faction a, Faction b, ref bool __result) 
        {
            __result = __result || AllegianceSys_Utils.IsEnemyBecauseOfAllegiance(a, b);
        }*/
    }
}
