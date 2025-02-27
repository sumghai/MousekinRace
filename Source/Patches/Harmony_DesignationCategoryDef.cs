using HarmonyLib;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    // Remove duplicate Town Square added by some Mousekin ideo building precepts from Architect Menu
    [HarmonyPatch(typeof(DesignationCategoryDef), nameof(DesignationCategoryDef.AllIdeoDesignators), MethodType.Getter)]
    public class Harmony_DesignationCategoryDef_AllIdeoDesignators_SkipDuplicateTownSquares
    {
        static IEnumerable<Designator> Postfix(IEnumerable<Designator> designators, DesignationCategoryDef __instance,
            Dictionary<DesignationCategoryDef.BuildablePreceptBuilding, Designator> ___ideoBuildingDesignatorsCached)
        {
            foreach (var designator in designators)
            {
                if (designator.Label.Contains(MousekinDefOf.Mousekin_TownSquare.LabelCap))
                { 
                    continue;
                }
                yield return designator;
            }
        }
    }
}
