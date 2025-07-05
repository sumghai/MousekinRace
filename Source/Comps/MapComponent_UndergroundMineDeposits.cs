using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    public class MapComponent_UndergroundMineDeposits : MapComponent
    {
        public HashSet<MineableThingCount> deposits = [];

        private int nextMiningBillID;

        private bool wasLoaded;

        public MapComponent_UndergroundMineDeposits(Map map) : base(map)
        {
        }

        
        // Update all underground deposits on current map, and add/remove deposits as required        
        public void RescanDeposits(List<MineableCountRange> mineablesAvailable)
        {
            float mapScaleFactor = (float)(map.Size.x * map.Size.z) / (float)(250 * 250);

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

        // Find the appropriate deposit by def, and deduct the smaller of
        // - the actual amount remaining in the deposit
        // - the requested amount specified by minedThing
        // If the actual amount remaining is smaller, then only extract that much (essentially emptying the deposit), and change the original input stackcount accordingly
        public void TryExtractResource(ref Thing minedThing)
        {
            ThingDef thingDef = minedThing.def;
            minedThing.stackCount = Math.Min(minedThing.stackCount, deposits.First(x => x.mineableThing == thingDef).amountRemaining);
            deposits.First(x => x.mineableThing == thingDef).amountRemaining -= minedThing.stackCount;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref deposits, "deposits", LookMode.Deep);
            Scribe_Values.Look(ref nextMiningBillID, "nextMiningBillID", 0);
            if (Scribe.mode == LoadSaveMode.LoadingVars) 
            { 
                wasLoaded = true;
            }
        }

        public bool DepositIsDepleted(ThingDef minedThingDef)
        {
            return deposits.First(x => x.mineableThing == minedThingDef).amountRemaining <= 0; 
        }

        public int GetNextMiningBillID(Map map) => GetNextID(map, ref nextMiningBillID);

        private static int GetNextID(Map map, ref int nextID)
        {
            if (Scribe.mode == LoadSaveMode.LoadingVars && !map.GetComponent<MapComponent_UndergroundMineDeposits>().wasLoaded)
            {
                Log.Warning("Getting next unique ID during LoadingVars before UniqueIDsManager in MapComponent_UndergroundMineDeposits was loaded. Assigning a random value.");
                return Rand.Int;
            }
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                Log.Warning("Getting next unique ID during saving. This may cause bugs.");
            }
            int result = nextID;
            nextID++;
            if (nextID == int.MaxValue)
            {
                Log.Warning("Next ID is at max value. Resetting to 0. This may cause bugs.");
                nextID = 0;
            }
            return result;
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
            return "MousekinMineDeposit_" + mineableThing.defName + GetHashCode();
        }
    }
}
