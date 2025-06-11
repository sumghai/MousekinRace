using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class CompProperties_UndergroundMineDeposits : CompProperties
    {
        public List<MineableCountRange> mineables = [];

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
}
