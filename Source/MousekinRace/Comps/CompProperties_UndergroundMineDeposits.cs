using RimWorld;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace MousekinRace
{
    public class CompProperties_UndergroundMineDeposits : CompProperties
    {
        public List<MineableCountRange> mineables = [];

        public List<BonusMineableOption> bonusMineables = [];

        public CompProperties_UndergroundMineDeposits()
        {
            compClass = typeof(CompUndergroundMineDeposits);
        }
    }

    public class MineableCountRange
    {
        public ThingDef mineableThing;
        public IntRange depositSize = new (10000, 10000);
        public int minedPortionSize = 1;
        public int workRequiredPerPortionMined = GenDate.TicksPerHour;
    }

    public class BonusMineableOption
    {
        public ThingDef bonusThing;
        public float selectionWeight;

        public override string ToString()
        {
            return $"{bonusThing.LabelCap} w={selectionWeight:n2}";
        }

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "bonusThing", xmlRoot.Name);
            selectionWeight = ParseHelper.FromString<float>(xmlRoot.FirstChild.Value);
        }
    }
}
