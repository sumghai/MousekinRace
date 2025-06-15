using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MousekinRace
{
    public class Building_MineEntrance : Building
    {
        public MiningBillStack miningBillStack;
        
        public CompUndergroundMineDeposits compUMD;

        public MapComponent_UndergroundMineDeposits mapComp_UMD;

        public const int maxMiningJobSlots = 20;

        public List<MiningJobSlot> miningJobSlots;

        public MiningBillStack MiningBillStack => miningBillStack;

        public bool UnassignedMiningJobSlotAvailable => miningJobSlots.Count(x => x.currentMiner is null) > 0;

        public Building_MineEntrance() 
        { 
            miningBillStack = new MiningBillStack();
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            Map.resourceCounter.UpdateResourceCounts();
            compUMD = GetComp<CompUndergroundMineDeposits>();
            mapComp_UMD = map.GetComponent<MapComponent_UndergroundMineDeposits>();
            mapComp_UMD.RescanDeposits(compUMD.Props.mineables);
            miningJobSlots ??= new(maxMiningJobSlots);
            UpdateMiningJobSlots();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref miningBillStack, "miningBillStack");
            Scribe_Collections.Look(ref miningJobSlots, "miningJobSlots", LookMode.Deep);
        }

        public void UpdateMiningJobSlots()
        {
            Log.Warning($"UpdateMiningJobSlots() @ {Find.TickManager.TicksGame}");

            MiningBill currentMiningBill = MiningBillStack.FirstCanDo;

            // If there is a valid and active mining bill,
            if (currentMiningBill != null && currentMiningBill.ShouldDoNow())
            {
                // If there are existing mining jobs in the queue, but the bill ID doesn't match,
                // clear the queue (and regenerate in the next conditional
                if (miningJobSlots.FirstOrDefault() is MiningJobSlot firstSlot && firstSlot.billLoadId != currentMiningBill.loadID)
                {
                    miningJobSlots.Clear();
                }

                // If there are no mining jobs in the queue (reasons: never initialized, last mining bill just ended)
                if (miningJobSlots.Empty())
                {
                    int slotsRequired;

                    // Do X times
                    if (currentMiningBill.repeatMode == BillRepeatModeDefOf.RepeatCount)
                    {
                        slotsRequired = currentMiningBill.targetCount;
                    }
                    // Do until you have X
                    else if (currentMiningBill.repeatMode == BillRepeatModeDefOf.TargetCount)
                    {
                        int amountRequired = currentMiningBill.targetCount - UndergroundMineSys_Utils.CountMinedProductsOnMap(currentMiningBill);
                        slotsRequired = (int)Math.Ceiling((float)amountRequired / (float)currentMiningBill.minedPortionSize);
                    }
                    // Do forever
                    else
                    {
                        slotsRequired = maxMiningJobSlots;
                    }

                    // Cap number of slots generated up to max limit
                    int slotsToReserve = Math.Min(slotsRequired, maxMiningJobSlots);

                    // Reserve the required number of mining job slots
                    for (int i = 0; i < slotsToReserve; i++)
                    {
                        miningJobSlots.Add(new(currentMiningBill.loadID, currentMiningBill.mineableThing));
                    }
                }
            }
            else
            {
                miningJobSlots.Clear();
            }
        }

        public MiningJobSlot GetMiningJobSlotForPawn(Pawn pawn)
        {
            Log.Warning($"GetMiningJobSlotForPawn() :: Looking for existing job slot for {pawn.NameShortColored} ({pawn.ThingID}) across {miningJobSlots.Count} slots...");
            Log.Warning($"{miningJobSlots.ToStringSafeEnumerable()}");
            Log.Warning($"{miningJobSlots.FindIndex(x => x.currentMiner == pawn)}");
            
            
            return miningJobSlots.First(x => x.currentMiner == pawn);
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append(base.GetInspectString());

            if (Prefs.DevMode && DebugSettings.godMode)
            {
                if (miningJobSlots.Count > 0)
                {
                    stringBuilder.Append($"Job slots: {miningJobSlots.Count(x => x.currentMiner is not null)} / {miningJobSlots.Count}");
                    stringBuilder.Append($"Unoccupied slots: {miningJobSlots.Count(x => x.currentMiner is null)}");
                }
                else
                {
                    stringBuilder.Append($"No mining bills or job slots active");
                } 
            }

            return stringBuilder.ToString();
        }
    }

    public class MiningJobSlot : IExposable
    {
        public int billLoadId;
        public ThingDef mineableThing;
        public float progress = 0f;
        public bool complete;
        public Pawn currentMiner;
        public Pawn previousMiner;

        public MiningJobSlot()
        {
        }

        public MiningJobSlot(int billLoadIdInput, ThingDef mineableThingInput)
        {
            billLoadId = billLoadIdInput;
            mineableThing = mineableThingInput;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref billLoadId, "billLoadId");
            Scribe_Defs.Look(ref mineableThing, "mineableThing");
            Scribe_Values.Look(ref progress, "progress", 0);
            Scribe_Values.Look(ref complete, "complete");
            Scribe_References.Look(ref currentMiner, "currentMiner");
            Scribe_References.Look(ref previousMiner, "previousMiner");
        }
    }
}
