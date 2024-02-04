using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace MousekinRace
{
    [HarmonyPatch(typeof(WorldFactionsUIUtility), nameof(WorldFactionsUIUtility.DoWindowContents))]
    public static class Harmony_WorldFactionsUIUtility_DoWindowContents
    {
        // Add warning message under faction list that some factions are required by the current scenario
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> CondAppendScenarioRequiredFaction_Explanation_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            CodeMatch[] toMatch = new CodeMatch[]
            {
                new CodeMatch(OpCodes.Pop),
                new CodeMatch(OpCodes.Ldc_R4),
                new CodeMatch(OpCodes.Stsfld, AccessTools.Method(typeof(WorldFactionsUIUtility), nameof(WorldFactionsUIUtility.warningHeight)))
            };

            CodeInstruction[] toInsert = new CodeInstruction[]
            {            
                new CodeInstruction(OpCodes.Ldloc_S, 21),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Harmony_WorldFactionsUIUtility_DoWindowContents), nameof(Harmony_WorldFactionsUIUtility_DoWindowContents.ShowScenarioRequiredFactionExplanation))),
            };

            codeMatcher.MatchEndForward(toMatch).Advance(1);
            codeMatcher.Insert(toInsert);

            return codeMatcher.InstructionEnumeration();
        }

        private static void ShowScenarioRequiredFactionExplanation(StringBuilder stringBuilder)
        {
            Scenario scenario = Current.Game.Scenario;

            if (scenario.AllParts.OfType<ScenPart_RequiredFaction>().Any()) 
            {
                // Prepend the custom warning before any existing warnings
                stringBuilder.Insert(0, "MousekinRace_ScenarioRequiresMousekinFactionsWarning".Translate(scenario.name) + "\n");
            }            
        }
    }
}
