using HarmonyLib;
using RimWorld;
using Verse;
using static RimWorld.IdeoFoundation_Deity;

namespace MousekinRace
{
    [HarmonyPatch(typeof(IdeoFoundation_Deity), nameof(IdeoFoundation_Deity.FillDeity))]
    public static class Harmony_IdeoFoundation_Deity_ForceMaleForMousekin
    {
        public static void Postfix(IdeoFoundation_Deity __instance, Deity deity)
        {
            if (__instance.ideo != null)
            {
                if (__instance.ideo.HasMeme(MousekinDefOf.Mousekin_IdeoMeme_AncestorWorship))
                {
                    deity.gender = Gender.Male;
                }
            }
        }
    }
}
