using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace MousekinRace
{
    // Pre-fetch the constructible when looking up available resources on the map
    [HarmonyPatch(typeof(WorkGiver_ConstructDeliverResources), nameof(WorkGiver_ConstructDeliverResources.ResourceDeliverJobFor))]
    public static class Harmony_WorkGiver_ConstructDeliverResources_ResourceDeliverJobFor_PrefetchConstructible
    { 
        public static IConstructible constructible = null;

        static void Prefix(IConstructible c)
        { 
            constructible = c;
        }
    }

    // If the blueprint/frame is for a Mousekin Great Pine ideo building (Mousekin_IdeoXmasTree),
    // make sure that we only allow MinifiedTrees whose InnerPlant is Plant_TreePine
    [HarmonyPatch]
    public static class Harmony_WorkGiver_ConstructDeliverResources_ResourceDeliverJobFor_MousekinXmasTreePatch
    {
        // Original: ResourceValidator(pawn, need, r) from inside
        // GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(need.thingDef), PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, (Thing r) => ResourceValidator(pawn, need, r));
        public static MethodInfo TargetMethod() => AccessTools.Method(AccessTools.Inner(typeof(WorkGiver_ConstructDeliverResources), "<>c__DisplayClass10_1"), "<ResourceDeliverJobFor>b__1");

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> MinifiedTreeTypeFilter_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            CodeMatch[] toMatch =
            [
                new CodeMatch(OpCodes.Ret)
            ];

            CodeInstruction[] toInsert =
            [
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_WorkGiver_ConstructDeliverResources_ResourceDeliverJobFor_MousekinXmasTreePatch), nameof(Wrapper)))
            ];

            codeMatcher.MatchStartForward(toMatch);
            codeMatcher.Insert(toInsert);

            return codeMatcher.InstructionEnumeration();
        }

        // We basically catch the output of the original ResourceValidator() method call, and run additional checks
        // to see if the blueprint/frame is Mousekin_IdeoXmasTree AND the resource is a minified Pine tree
        private static bool Wrapper(bool input, Thing thing)
        {
            if (thing is MinifiedTree minifiedTree && Harmony_WorkGiver_ConstructDeliverResources_ResourceDeliverJobFor_PrefetchConstructible.constructible.ToString().Contains(MousekinDefOf.Mousekin_IdeoXmasTree.defName))
            {
                return input && minifiedTree.InnerTree.def == ThingDefOf.Plant_TreePine;
            }
            return input;
        }
    }
}
