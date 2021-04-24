using AlienRace;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using Verse;

namespace MousekinRace
{
    public class MousekinRaceMod : Mod
    {
        private static readonly Type patchType = typeof(HarmonyPatches);

        public MousekinRaceMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("com.MousekinRace.patches");
            harmony.PatchAll();
        }

        // Prevent non-hidden, nomadic factions from generating settlements on the world map,
        // by checking the appropriate flag in the FactionHelperExtension mod extension
        [HarmonyPatch(typeof(WorldObjectsHolder), nameof(WorldObjectsHolder.Add))]
        static class WorldObjectsHolder_Add_HideSettlementsForNomadicFactions
        {
            static bool Prefix(WorldObject o)
            {
                return (o?.Faction?.def.GetModExtension<FactionHelperExtension>()?.canSpawnSettlements == false) ? false : true;
            }
        }
    }
}