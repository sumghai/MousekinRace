using HarmonyLib;
using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    // Mark specific ritual type(s) as disabled if a conflicting meme exists in the ideo
    [HarmonyPatch(typeof(IdeoUIUtility), nameof(IdeoUIUtility.GetMemeTip))]
    public static class Harmony_IdeoUIUtility_GetMemeTip_AppendInfoRegardingConflictingRituals
    {
        static void Postfix(ref string __result, MemeDef meme, Ideo ideo)
        {
            if (ideo != null && ideo.culture.IsMousekin() && meme.HasModExtension<IdeoMemeRitualsExtension>())
            {
                foreach (RitualWithExtraParams ritualWithExtraParams in meme.GetModExtension<IdeoMemeRitualsExtension>().ritualsWithExtraParams)
                {
                    if (ritualWithExtraParams.conflictingMemes.Intersect(ideo.memes).Any())
                    {
                        string ritualTypeName = ritualWithExtraParams.ritual.pattern.shortDescOverride.CapitalizeFirst();

                        string ritualDisabledByMemes = "MousekinRace_MemeTip_RitualDisabledByMeme".Translate(string.Join(", ", ritualWithExtraParams.conflictingMemes.Select(x => x.LabelCap)));

                        __result = __result.Replace(ritualTypeName, (ritualTypeName + " " + ritualDisabledByMemes).Colorize(new Color(0.5f, 0.5f, 0.5f)));
                    }
                }
            }
        }
    }
}
