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
            static FactionHelperExtension GetExtensionFor(Faction f)
            {
                return f.def.GetModExtension<FactionHelperExtension>();
            }

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

                if ((__instance.RelationWith(other, allowNull: true) == null) && ((currentFactionExt?.startingGoodwillByFactionDefs.Exists(x => x.factionDef == other.def) ?? false) || (otherFactionExt?.startingGoodwillByFactionDefs.Exists(x => x.factionDef == __instance.def) ?? false)))
                {
                    int? currentToOtherFactionGoodwillMin = currentFactionExt.startingGoodwillByFactionDefs.Find(x => x.factionDef == other.def)?.Min ?? null;
                    int? currentToOtherFactionGoodwillMax = currentFactionExt.startingGoodwillByFactionDefs.Find(x => x.factionDef == other.def)?.Max ?? null;
                    int? otherToCurrentFactionGoodwillMin = otherFactionExt.startingGoodwillByFactionDefs.Find(x => x.factionDef == __instance.def)?.Min ?? null;
                    int? otherToCurrentFactionGoodwillMax = otherFactionExt.startingGoodwillByFactionDefs.Find(x => x.factionDef == __instance.def)?.Max ?? null;
                    Log.Warning(currentToOtherFactionGoodwillMin + ", "
                            + currentToOtherFactionGoodwillMax + ", "
                            + otherToCurrentFactionGoodwillMin + ", "
                            + otherToCurrentFactionGoodwillMax
                        );

                    int mutualGoodwillMin = MinOfNullableInts(currentToOtherFactionGoodwillMin, otherToCurrentFactionGoodwillMin);

                    int mutualGoodwillMax = MinOfNullableInts(currentToOtherFactionGoodwillMax, otherToCurrentFactionGoodwillMax);

                    Log.Warning(__instance + "(" + __instance.def + ")" + " and " + other + "(" + other.def + ") have custom starting goodwill ranges! (" + mutualGoodwillMin + "~" + mutualGoodwillMax + ")");

                    int finalMutualGoodWill = Rand.RangeInclusive(mutualGoodwillMin, mutualGoodwillMax);

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

                    FactionRelationKind kind = FactionRelationKind.Hostile;
                    FactionRelation factionRelation = new FactionRelation();
                    factionRelation.other = other;
                    factionRelation.goodwill = -80;
                    factionRelation.kind = kind;
                    __instance.relations.Add(factionRelation);
                    FactionRelation factionRelation2 = new FactionRelation();
                    factionRelation2.other = __instance;
                    factionRelation2.goodwill = -80;
                    factionRelation2.kind = kind;
                    other.relations.Add(factionRelation2);

                    return false;
                }*/

                return true;
            }
        }
    }
}