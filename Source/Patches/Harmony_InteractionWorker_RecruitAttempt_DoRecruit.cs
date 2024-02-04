using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace MousekinRace
{
    // Force Mousekin-unique animals to use name generators if tamed by a Mousekin
    [HarmonyPatch(typeof(InteractionWorker_RecruitAttempt), nameof(InteractionWorker_RecruitAttempt.DoRecruit), new Type[] { typeof(Pawn), typeof(Pawn), typeof(string), typeof(string), typeof(bool), typeof(bool) },
        new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal })]
    public static class Harmony_InteractionWorker_RecruitAttempt_DoRecruit_NameAnimalOnTame
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            CodeMatch[] toMatch = new CodeMatch[] 
            {
                new CodeMatch(OpCodes.Ldstr, "MessageTameAndNameSuccess")
            };

            CodeInstruction[] toInsert = new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0, null),
                new CodeInstruction(OpCodes.Ldarg_1, null),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_InteractionWorker_RecruitAttempt_DoRecruit_NameAnimalOnTame), nameof(Harmony_InteractionWorker_RecruitAttempt_DoRecruit_NameAnimalOnTame.ConditionallySetAnimalName)))
            };

            codeMatcher.MatchEndForward(toMatch);
            codeMatcher.Insert(toInsert);

            return codeMatcher.InstructionEnumeration();
        }

        public static void ConditionallySetAnimalName(Pawn recruiter, Pawn recruitee)
        {
            if (Utils.IsMousekin(recruiter) && recruitee.kindDef.HasModExtension<NameAnimalOnTameExtension>())
            {
                recruitee.Name = PawnBioAndNameGenerator.GeneratePawnName(recruitee, NameStyle.Full, null);
            }
        }
    }
}
