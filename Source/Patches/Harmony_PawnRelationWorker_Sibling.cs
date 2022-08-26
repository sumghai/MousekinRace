using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace MousekinRace
{
    // Tweak world pawn generation such that:
    // - Parents of Mousekin colonists only ever spawn in the first available Mousekin Kingdom faction
    // - Parents of non-Mousekins do not spawn in any Mousekin factions
    [HarmonyPatch(typeof(PawnRelationWorker_Sibling), nameof(PawnRelationWorker_Sibling.GenerateParent))]
    public static class Harmony_PawnRelationWorker_Sibling_GenerateParent_KeepMiceInMouseKingdom
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var factionOriginalMethod = AccessTools.Method(typeof(FactionManager), nameof(FactionManager.TryGetRandomNonColonyHumanlikeFaction));
            var factionNewMethod = AccessTools.Method(typeof(Harmony_PawnRelationWorker_Sibling_GenerateParent_KeepMiceInMouseKingdom), nameof(Harmony_PawnRelationWorker_Sibling_GenerateParent_KeepMiceInMouseKingdom.SetFactionByRace));

            var generatedParentPawnOriginalMethod = AccessTools.Method(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new Type[] { typeof(PawnGenerationRequest) });
            var generatedParentPawnNewMethod = AccessTools.Method(typeof(Harmony_PawnRelationWorker_Sibling_GenerateParent_KeepMiceInMouseKingdom), nameof(Harmony_PawnRelationWorker_Sibling_GenerateParent_KeepMiceInMouseKingdom.GeneratePawnKindByRace));

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(factionOriginalMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1); // Loads existingChild argument
                    yield return new CodeInstruction(OpCodes.Callvirt, factionNewMethod);
                }
                else if (instruction.Calls(generatedParentPawnOriginalMethod))
                {
                    yield return new CodeInstruction(OpCodes.Call, generatedParentPawnNewMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        public static bool SetFactionByRace(this FactionManager factionManager, out Faction faction, bool tryMedievalOrBetter, bool allowDefeated, TechLevel minTechLevel, bool allowTemporary, Pawn forPawn)
        {            
            if (Utils.IsMousekin(forPawn))
            {
                faction = factionManager.AllFactions.Where((Faction f) => f.def == MousekinDefOf.Mousekin_FactionKingdom).First();
                return true;
            } 
            else
            {
                return factionManager.AllFactions.Where((Faction x) => !x.IsPlayer && !x.Hidden && x.def.humanlikeFaction && (allowDefeated || !x.defeated) && (allowTemporary || !x.temporary) && (minTechLevel == TechLevel.Undefined || (int)x.def.techLevel >= (int)minTechLevel) && Utils.GetRaceOfFaction(x.def) != MousekinDefOf.Mousekin).TryRandomElementByWeight((Faction x) => (tryMedievalOrBetter && (int)x.def.techLevel < 3) ? 0.1f : 1f, out faction);
            }
        }

        // Ensure world pawn parents (Mousekin and non-Mousekin) use gender-neutral pawnkinds when spawning in NPC factions
        public static Pawn GeneratePawnKindByRace(PawnGenerationRequest request)
        {          
            request.KindDef = Utils.IsMousekin(request.KindDef) ? request.Faction.def.basicMemberKind : request.Faction.RandomPawnKind();

            return PawnGenerator.GeneratePawn(request);
        }
    }

    // Ensure Mousekin parents are at least 20~30 years older than their oldest child, and have no cryptosleep-related shenanigans
    [HarmonyPatch(typeof(PawnRelationWorker_Sibling), nameof(PawnRelationWorker_Sibling.GenerateParentParams))]
    public static class Harmony_PawnRelationWorer_Sibling_GenerateParentParams_SetSensibleAges
    {
        static void Postfix(Pawn generatedChild, Pawn existingChild, ref float biologicalAge, ref float chronologicalAge)
        {
            if (Utils.IsMousekin(existingChild))
            {
                float parentFinalAge = Math.Max(existingChild.ageTracker.AgeBiologicalYears, generatedChild.ageTracker.AgeBiologicalYears) + Rand.RangeInclusive(20, 30);
                biologicalAge = parentFinalAge;
                chronologicalAge = parentFinalAge;
            }
        }
    }
}
