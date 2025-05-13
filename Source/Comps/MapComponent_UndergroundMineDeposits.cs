using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    public class MapComponent_UndergroundMineDeposits : MapComponent
    {
        public HashSet<MineableThingCount> deposits = [];

        public MapComponent_UndergroundMineDeposits(Map map) : base(map)
        {
        }

        // Update all underground deposits on current map, and add/remove deposits as required        
        public void RescanDeposits(List<MineableCountRange> mineablesAvailable)
        {
            float mapScaleFactor = (map.Size.x * map.Size.z) / (250 * 250);

            // Add any new mineables
            foreach (MineableCountRange mineable in mineablesAvailable)
            {
                if (!deposits.Where(x => x.mineableThing == mineable.mineableThing).Any())
                {
                    // Larger maps (by cell area) get larger deposits
                    int depositSizeScaled = (int)(mineable.depositSize.RandomInRange * mapScaleFactor);
                    deposits.Add(new(mineable.mineableThing, depositSizeScaled));
                }
            }

            // Remove any obsolete (i.e. deleted) mineables
            deposits.RemoveWhere(x => !mineablesAvailable.Select(y => y.mineableThing).Contains(x.mineableThing));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref deposits, "deposits", LookMode.Deep);
        }
    }

    public class MineableThingCount : IExposable, ILoadReferenceable
    {
        public ThingDef mineableThing;
        public int amountRemaining;
        public bool IsEmpty => amountRemaining <= 0;

        // Argument-less constructor required for IExposable
        public MineableThingCount()
        {
        }

        public MineableThingCount(ThingDef mineableThingInput, int amountRemainingInput)
        {
            mineableThing = mineableThingInput;
            amountRemaining = amountRemainingInput;
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref mineableThing, "mineableThing");
            Scribe_Values.Look(ref amountRemaining, "amountRemaining");
        }

        public string GetUniqueLoadID()
        {
            return "MousekinMineDeposit_" + mineableThing.defName;
        }
    }
}
