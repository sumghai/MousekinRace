using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace MousekinRace
{
    // Replace Ideology DLC Ideo configuration page with Fixed Ideo page if required by current scenario
    [HarmonyPatch(typeof(Scenario), nameof(Scenario.GetFirstConfigPage))]
    public static class Harmony_Scenario_GetFirstConfigPage_PreselectIdeoForMousekinScenarios
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ScenarioPresetIdeo_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            CodeMatch[] toMatch = new CodeMatch[]
            {
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld)
            };

            CodeInstruction[] toInsert = new CodeInstruction[]
            { 
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_Scenario_GetFirstConfigPage_PreselectIdeoForMousekinScenarios), nameof(Harmony_Scenario_GetFirstConfigPage_PreselectIdeoForMousekinScenarios.CondReplaceChooseIdeoPageWithIdeoPreselectedPage)))
            };

            codeMatcher.MatchStartForward(toMatch);
            codeMatcher.Insert(toInsert);

            return codeMatcher.InstructionEnumeration();
        }

        private static void CondReplaceChooseIdeoPageWithIdeoPreselectedPage(List<Page> pagesList)
        {
            IEnumerable<ScenPart_RequiredFaction> scenParts = Current.Game.Scenario.AllParts.OfType<ScenPart_RequiredFaction>();

            if (scenParts.Any() && scenParts.FirstOrDefault(scenPart => scenPart.useFactionIdeoForPlayer == true) != null)
            {
                pagesList.RemoveAll(page => page.GetType().Equals(typeof(Page_ChooseIdeoPreset)));
                pagesList.Add(new Page_IdeoFixedByScenario());
            }
        }
    }
}
