using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace MousekinRace
{
    // If a building has a CompAllowedFuelTypes comp, tell pawns search for the best fuel to
    // check the fuel types allowed by ITab_FuelFilter
    [HarmonyPatch(typeof(RefuelWorkGiverUtility), nameof(RefuelWorkGiverUtility.FindBestFuel))]
    public static class Harmony_RefuelWorkGiverUtility_FindBestFuel_RespectFuelFilterSettings
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            CodeMatch[] toMatch =
            [
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Stfld)
            ];

            CodeInstruction[] toInsert =
            [
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_RefuelWorkGiverUtility_FindBestFuel_RespectFuelFilterSettings), nameof(ConditionallyGetFilter)))
            ];

            codeMatcher.MatchEndForward(toMatch);
            codeMatcher.Insert(toInsert);

            return codeMatcher.InstructionEnumeration();
        }

        private static ThingFilter ConditionallyGetFilter(ThingFilter originalFilter, Thing refuelable)
        {            
            return (refuelable.HasComp<CompAllowedFuelTypes>() && !ITab_FuelFilter.IsDisabledByOtherFuelFilterMods()) ? refuelable.TryGetComp<CompAllowedFuelTypes>()?.allowedFuelTypesFilter : originalFilter;
        }
    }
}