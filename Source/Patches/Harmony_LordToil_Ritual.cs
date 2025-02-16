using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace MousekinRace
{
    // If a ritual focus target is a Mousekin Town Square building, change spectator focus location from the building itself to:
    // - the Town Square's center position cell for the Purging Flames heretic execution ritual
    // - the Town Square's speaker position cell for all other rituals (e.g. Leader speech)
    [HarmonyPatch(typeof(LordToil_Ritual), nameof(LordToil_Ritual.UpdateAllDuties))]
    public static class Harmony_LordToil_Ritual_UpdateAllDuties_OverrideForTownSquare
    {       
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            CodeMatch[] toMatch1 =
            [
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(GenAdj), nameof(GenAdj.OccupiedRect), [typeof(Thing)]))
            ];

            CodeInstruction[] toInsert1 =
            [
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_LordToil_Ritual_UpdateAllDuties_OverrideForTownSquare), nameof(ConditionallyGetSpectateRect1)))
            ];

            codeMatcher.MatchStartForward(toMatch1);
            codeMatcher.RemoveInstruction();
            codeMatcher.Insert(toInsert1);

            CodeMatch[] toMatch2 =
            [
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(CellRect), nameof(CellRect.CenteredOn), [typeof(IntVec3), typeof(int)]))
            ];

            CodeInstruction[] toInsert2 =
            [
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_LordToil_Ritual_UpdateAllDuties_OverrideForTownSquare), nameof(ConditionallyGetSpectateRect2)))
            ];

            codeMatcher.MatchStartForward(toMatch2);
            codeMatcher.RemoveInstruction();
            codeMatcher.Insert(toInsert2);

            return codeMatcher.InstructionEnumeration();
        }

        private static CellRect ConditionallyGetSpectateRect1(this Thing thing, LordToil_Ritual lordToil_Ritual)
        {            
            return (thing is Building_TownSquare townSquare) ? CellRect.CenteredOn(RitualIsHereticExecution(lordToil_Ritual) ? townSquare.centerCellPos : townSquare.speechSpeakerCellPos, 0) : thing.OccupiedRect();
        }

        private static CellRect ConditionallyGetSpectateRect2(IntVec3 center, int radius, LordToil_Ritual lordToil_Ritual)
        {
            return (lordToil_Ritual.ritual.selectedTarget.Thing is Building_TownSquare townSquare) ? CellRect.CenteredOn(RitualIsHereticExecution(lordToil_Ritual) ? townSquare.centerCellPos : townSquare.speechSpeakerCellPos, radius) : CellRect.CenteredOn(center, radius);
        }

        static bool RitualIsHereticExecution(LordToil_Ritual lordToil_Ritual)
        {
            return lordToil_Ritual.ritual is LordJob_Ritual_HereticExecution;
        }
    }
}
