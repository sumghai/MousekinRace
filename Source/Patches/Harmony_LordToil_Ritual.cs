using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace MousekinRace
{
    // If a ritual focus target is a Mousekin Town Square building,
    // change the pawn duty focus to the Town Square's center cell instead of the building itself
    [HarmonyPatch(typeof(LordToil_Ritual), nameof(LordToil_Ritual.UpdateAllDuties))]
    public static class Harmony_LordToil_Ritual_UpdateAllDuties_ChangeFocusCellForTownSquare
    {        
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            CodeMatch[] toMatch =
            [
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(GenAdj), nameof(GenAdj.OccupiedRect), [typeof(Thing)]))
            ];

            CodeInstruction[] toInsert =
            [
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_LordToil_Ritual_UpdateAllDuties_ChangeFocusCellForTownSquare), nameof(ConditionallyGetSpectateRect)))
            ];

            codeMatcher.MatchStartForward(toMatch);
            codeMatcher.RemoveInstruction();
            codeMatcher.Insert(toInsert);

            return codeMatcher.InstructionEnumeration();
        }

        private static CellRect ConditionallyGetSpectateRect(this Thing thing)
        {
            return (thing is Building_TownSquare townSquare) ? CellRect.CenteredOn(townSquare.centerCellPos, 0) : thing.OccupiedRect();
        }
    }
}
