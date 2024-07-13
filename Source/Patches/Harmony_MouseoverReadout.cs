using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace MousekinRace
{
    // Skip listing items stored inside Root Cellars in the Mouseover Readout at the bottom-left corner of the GUI
    [HarmonyPatch(typeof(MouseoverReadout), nameof(MouseoverReadout.MouseoverReadoutOnGUI))]
    public static class Harmony_MouseoverReadout_MouseoverReadoutOnGUI_RootCellarsSkipListingContents
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> RootCellarsSkipListingContents(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            CodeMatch[] toMatch = new CodeMatch[]
            {
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(GridsUtility), nameof(GridsUtility.GetThingList)))
            };

            CodeInstruction[] toInsert = new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_MouseoverReadout_MouseoverReadoutOnGUI_RootCellarsSkipListingContents), nameof(GetThingListSkipRootCellarContents)))
            };

            codeMatcher.MatchStartForward(toMatch);
            codeMatcher.RemoveInstruction();
            codeMatcher.Insert(toInsert);

            return codeMatcher.InstructionEnumeration();
        }

        private static List<Thing> GetThingListSkipRootCellarContents(this IntVec3 c, Map map)
        {
            return c.GetThingList(map).Where(t => !Building_CellarOutdoor.ThingIsInCellar(t)).ToList();
        }
    }
}