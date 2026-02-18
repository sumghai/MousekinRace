using HarmonyLib;
using RimWorld;
using System.Text.RegularExpressions;
using Verse;

namespace GrimalkinRace
{
    // Prepend thought descriptions for Grimalkins with "yaong"-speak version
    // (similar to Biotech DLC's baby talk filter)
    [HarmonyPatch(typeof(Thought), nameof(Thought.Description), MethodType.Getter)]
    public static class Harmony_Thought_Description
    {
        static void Postfix(ref Thought __instance, ref string __result)
        {
            if (__instance.pawn.IsGrimalkin())
            {
                string yaong = "GrimalkinRace_Thought_Yaong".Translate();
                string originalDesc = __result;
                string originalDescNoTags = originalDesc.StripTags();
                string grimalkinSpeech = (originalDescNoTags.Contains(" ")) ? Regex.Replace(originalDescNoTags, @"\b\w+\b", yaong) : Regex.Replace(originalDescNoTags, @"[^\p{P}\p{Z}]", yaong);
                __result = GenText.CapitalizeSentences(grimalkinSpeech) + "\n\n" + ("Translation".Translate() + ": " + originalDesc).Colorize(ColoredText.SubtleGrayColor);
            }
        }
    }
}
