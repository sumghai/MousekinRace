using AlienRace;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
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

        // Set up custom initial relations between NPC factions,
        // by using recommended values set in the FactionHelperExtension mod extension
        [HarmonyPatch(typeof(Faction), nameof(Faction.TryMakeInitialRelationsWith))]
        static class Faction_TryMakeInitialRelationsWith_SetCustomRelationsBetweenNpcFactions
        {
            // Helper function to fetch the faction's FactionHelperExtension, if it exists
            static FactionHelperExtension GetExtensionFor(Faction f)
            {
                return f.def.GetModExtension<FactionHelperExtension>();
            }

            // Helper function that calculates the minimum of two nullable integers
            static int MinOfNullableInts(int? num1, int? num2)
            {
                if (num1.HasValue && num2.HasValue)
                {
                    return (num1 < num2) ? (int)num1 : (int)num2;
                }
                if (num1.HasValue && !num2.HasValue)
                {
                    return (int)num1; 
                }
                if (!num1.HasValue && num2.HasValue)
                {
                    return (int)num2;
                }
                return 0;
            }

            static bool Prefix(Faction __instance, Faction other)
            {
                var currentFactionExt = GetExtensionFor(__instance);
                var otherFactionExt = GetExtensionFor(other);

                // If no relations already exist between factions, and at least one of the factions references the other
                // via custom values in the FactionHelperExtension
                if ((__instance.RelationWith(other, allowNull: true) == null) && ((currentFactionExt?.startingGoodwillByFactionDefs.Exists(x => x.factionDef == other.def) ?? false) || (otherFactionExt?.startingGoodwillByFactionDefs.Exists(x => x.factionDef == __instance.def) ?? false)))
                {
                    // Get the lowest range of goodwill possible between factions
                    int? currentToOtherFactionGoodwillMin = currentFactionExt.startingGoodwillByFactionDefs.Find(x => x.factionDef == other.def)?.Min ?? null;
                    int? currentToOtherFactionGoodwillMax = currentFactionExt.startingGoodwillByFactionDefs.Find(x => x.factionDef == other.def)?.Max ?? null;
                    int? otherToCurrentFactionGoodwillMin = otherFactionExt.startingGoodwillByFactionDefs.Find(x => x.factionDef == __instance.def)?.Min ?? null;
                    int? otherToCurrentFactionGoodwillMax = otherFactionExt.startingGoodwillByFactionDefs.Find(x => x.factionDef == __instance.def)?.Max ?? null;

                    int mutualGoodwillMin = MinOfNullableInts(currentToOtherFactionGoodwillMin, otherToCurrentFactionGoodwillMin);

                    int mutualGoodwillMax = MinOfNullableInts(currentToOtherFactionGoodwillMax, otherToCurrentFactionGoodwillMax);

                    // Generate a random goodwill value within the range
                    int finalMutualGoodWill = Rand.RangeInclusive(mutualGoodwillMin, mutualGoodwillMax);

                    // Assign mutual faction relations
                    FactionRelationKind kind = (finalMutualGoodWill > -10) ? ((finalMutualGoodWill < 75) ? FactionRelationKind.Neutral : FactionRelationKind.Ally) : FactionRelationKind.Hostile;
                    FactionRelation factionRelation = new FactionRelation
                    {
                        other = other,
                        goodwill = finalMutualGoodWill,
                        kind = kind
                    };
                    __instance.relations.Add(factionRelation);
                    FactionRelation factionRelation2 = new FactionRelation
                    {
                        other = __instance,
                        goodwill = finalMutualGoodWill,
                        kind = kind
                    };
                    other.relations.Add(factionRelation2);

                    return false;
                }

                /* TODO - Move the following commented out snippet to new faction rivalry system
                 * 
                 * if ((currentFactionExt?.rivalFactionTypes.Contains(other.def) ?? false) || (otherFactionExt?.rivalFactionTypes.Contains(__instance.def) ?? false))
                {
                    Log.Warning(__instance + " (" + __instance.def.label + ") and " + other + " (" + other.def.label + ") are rivals!");

                    return false;
                }*/

                return true;
            }
        }
    }
}