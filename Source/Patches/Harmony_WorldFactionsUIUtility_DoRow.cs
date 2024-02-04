using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    // At world gen faction setup, replace the Remove button with a "(Required)" label if the faction in the current row is required by the scenario
    [HarmonyPatch(typeof(WorldFactionsUIUtility), nameof(WorldFactionsUIUtility.DoRow))]
    public static class Harmony_WorldFactionsUIUtility_DoRow
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ScenarioRequiredFaction_HideRemoveButton_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);        

            CodeMatch[] toMatch = new CodeMatch[]
            { 
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Widgets), nameof(Widgets.ButtonImage), new Type[] { typeof(Rect), typeof(Texture2D), typeof(bool) }))
            };

            CodeInstruction[] toInsert = new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),  // factionDef
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_WorldFactionsUIUtility_DoRow), nameof(Harmony_WorldFactionsUIUtility_DoRow.CondScenarioRequiredButtonImage)))
            };

            codeMatcher.MatchStartForward(toMatch);
            codeMatcher.RemoveInstruction();
            codeMatcher.Insert(toInsert);

            return codeMatcher.InstructionEnumeration();
        }

        // Show either a clickable Remove button, or a non-clickable "(Required)" label
        private static bool CondScenarioRequiredButtonImage(Rect butRect, Texture2D tex, bool doMouseoverSound, FactionDef factionDef)
        {
            List<FactionDef> factionsRequiredByScenario = new();

            IEnumerable<ScenPart_RequiredFaction> scenParts = Current.Game.Scenario.AllParts.OfType<ScenPart_RequiredFaction>();

            if (scenParts.Any())
            {
                foreach (var part in scenParts)
                {
                    factionsRequiredByScenario.AddDistinct(part.factionDef);
                }
            }

            bool output = false;

            if (factionsRequiredByScenario.Contains(factionDef))
            {
                Text.Anchor = TextAnchor.MiddleRight;
                Widgets.Label(new Rect(butRect.x - 100f + 20f, butRect.y, 100f, butRect.height), "MousekinRace_CreateWorldFactionType_RequiredByScenario".Translate().Colorize(Color.yellow));
                Text.Anchor = TextAnchor.UpperLeft;
            }
            else
            {
                output = Widgets.ButtonImage(butRect, tex, doMouseoverSound);
            }
            return output;
        }
    }
}
