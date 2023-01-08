using HarmonyLib;
using RimWorld;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    [HarmonyPatch(typeof(IdeoFoundation), nameof(IdeoFoundation.GetRandomIconDef))]
    public static class Harmony_IdeoFoundation_GetRandomIconDef_LimitToMatchingCultures
    {
        public static void Postfix(Ideo ideo, ref IdeoIconDef __result)
        {
            if (ideo.culture.GetModExtension<IdeoSymbolIgnoreRandomExtension>() != null)
            {
                IEnumerable<IdeoIconDef> icons = DefDatabase<IdeoIconDef>.AllDefs.Where(x => x.cultures != null && x.cultures.Contains(ideo.culture));
                if (icons.EnumerableCount() > 0)
                { 
                    __result = icons.RandomElement();
                }
            }
        }
    }
}
