using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using VFECore;

namespace MousekinRace
{
    [HarmonyPatch]
    public static class Harmony_VFE_NewFactionSpawningUtility_SyncRelationsWithAllegianceFaction
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(NewFactionSpawningUtility), nameof(NewFactionSpawningUtility.SpawnWithSettlements));
            yield return AccessTools.Method(typeof(NewFactionSpawningUtility), nameof(NewFactionSpawningUtility.SpawnWithoutSettlements));
        }

        static void Postfix()
        {
            GameComponent_Allegiance gameComp = GameComponent_Allegiance.Instance;
            
            if (gameComp != null && gameComp.HasDeclaredAllegiance)
            {
                AllegianceSys_Utils.SyncRelationsWithAllegianceFaction(gameComp.alignedFaction);
            }
        }
    }
}
