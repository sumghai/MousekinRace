using HarmonyLib;
using RimWorld;
using Verse;

namespace MousekinRace
{
    // Prevent animals and wild people from eating food stored in root cellars
    [HarmonyPatch(typeof(FoodUtility), nameof(FoodUtility.WillEat), [typeof(Pawn), typeof(Thing), typeof(Pawn), typeof(bool), typeof(bool)])]
    public static class Harmony_FoodUtility_WillEat_AnimalsSkipFoodInCellars
    {
        static void Postfix(ref bool __result, Pawn p, Thing food)
        {
            // Make sure to only prohibit food stored in the cellar's storage cell
            // (food on any other cell occupied by the cellar is considered "outside" the cellar, and thus valid)
            if ((p.IsAnimal || p.IsWildMan()) && food.Position.GetFirstBuilding(food.Map) is Building_CellarOutdoor cellar && food.Position == cellar.StorageCellPos)
            {
                __result = false;
            }
        }
    }
}
