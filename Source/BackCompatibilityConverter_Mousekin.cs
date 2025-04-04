using System;
using System.Xml;
using Verse;

namespace MousekinRace
{
    public class BackCompatibilityConverter_Mousekin : BackCompatibilityConverter
    {
        public override bool AppliesToVersion(int majorVer, int minorVer) => true;

        public override string BackCompatibleDefName(Type defType, string defName, bool forDefInjections = false, XmlNode node = null) => null;

        public override Type GetBackCompatibleType(Type baseType, string providedClassName, XmlNode node) => null;

        public override void PostExposeData(object obj) { }

        public override void PostLoadSavegame(string loadingVersion)
        {
            base.PostLoadSavegame(loadingVersion);
            AllegianceSys_Utils.UpdatePlayerFactionToMousekinOnJoining();
            AllegianceSys_Utils.ResetFactionRestrictedCraftingBills();

            // Remove null or missing flowers from the flower tracker map comps
            // This handles the edge case where a player plants some flowers from a third-party mod, saves the game,
            // removes the mod, and then reloads the savegame with missing references to the uninstalled flower mod
            Find.Maps.ForEach(map => {
                map.GetComponent<MapComponent_FlowerTracker>()?.playerFlowersPlanted.RemoveWhere(t => t == null);
            });
        }
    }
}
