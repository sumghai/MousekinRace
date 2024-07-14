using Verse;

namespace MousekinRace
{
    // If LWM's Deep Storage is active, prevent the mod from trying to open the (now-removed)
    // Deep Storage Contents ITab (Mousekin Race uses its own Contents ITab)
    // 
    // Note: T  his patch is conditionally called by 
    public class Harmony_LWM_DeepStorage_Open_DS_Tab_On_Select_IgnoreRootCellars
    {
        public static bool Prefix()
        {
            Thing thing = Find.Selector.SingleSelectedThing;
            if (thing != null && thing is ThingWithComps && thing is Building_CellarOutdoor)
            { 
                return false;
            }
            return true;
        }
    }
}
