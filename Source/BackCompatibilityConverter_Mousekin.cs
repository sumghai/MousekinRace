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
            AllegianceSys_Utils.ResetFactionRestrictedCraftingBills();
        }
    }
}
