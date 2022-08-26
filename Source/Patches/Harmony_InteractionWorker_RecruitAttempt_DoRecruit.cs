using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace MousekinRace
{
    [HarmonyPatch(typeof(InteractionWorker_RecruitAttempt), nameof(InteractionWorker_RecruitAttempt.DoRecruit), new Type[] { typeof(Pawn), typeof(Pawn), typeof(string), typeof(string), typeof(bool), typeof(bool) },
        new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal })]
    public static class Harmony_InteractionWorker_RecruitAttempt_DoRecruit_NameAnimalOnTame
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            bool found = false;

            for (var i = 0; i < codes.Count; i++)
            {
                // Find the method call where an unnamed wild animal is tamed, and add a conditional check (see below) 
                if (!found && codes[i].opcode == OpCodes.Ldstr && codes[i].OperandIs("MessageTameAndNameSuccess"))
                {
                    found = true;
                    yield return new CodeInstruction(OpCodes.Ldarg_0, null);
                    yield return new CodeInstruction(OpCodes.Ldarg_1, null);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_InteractionWorker_RecruitAttempt_DoRecruit_NameAnimalOnTame), "ConditionallySetAnimalName", null, null));
                }

                yield return codes[i];
            }
        }

        public static void ConditionallySetAnimalName(Pawn recruiter, Pawn recruitee)
        {
            // If the tamer is a Mousekin and the animal being tamed has a custom mod extension, immediately name the animal upon taming
            if (Utils.IsMousekin(recruiter) && recruitee.kindDef.HasModExtension<NameAnimalOnTameExtension>())
            {
                recruitee.Name = PawnBioAndNameGenerator.GeneratePawnName(recruitee, NameStyle.Full, null);
            }
        }
    }
}
