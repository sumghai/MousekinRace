using Verse;

namespace MousekinRace
{
    // Remove LWM.DeepStorage.ITab_DeepStorage_Inventory from Root Cellars if LWM's Deep Storage mod is active,
    // as the Mousekin Race cellars don't use the Deep Storage system, and calculates ambient temperatures differently
    [StaticConstructorOnStartup]
    public class LwmDeepStorageCompatPatch
    {
        static LwmDeepStorageCompatPatch()
        {
            if (ModCompatibility.LwmDeepStorageIsActive)
            {
                ThingDef rootCellar = DefDatabase<ThingDef>.AllDefsListForReading.FirstOrDefault(def => def.thingClass == typeof(Building_CellarOutdoor));

                rootCellar.inspectorTabs.RemoveWhere(tab => tab.Name == "ITab_DeepStorage_Inventory");
                rootCellar.inspectorTabsResolved.RemoveWhere(tab => tab.GetType().Name == "ITab_DeepStorage_Inventory");
                Log.Message("Mousekin Race :: Removed LWM.DeepStorage.ITab_DeepStorage_Inventory from " + rootCellar);
            }
        }
    }
}
