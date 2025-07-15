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

        public CompMineEntranceAnim compMEA;

        public MapComponent_UndergroundMineDeposits mapComp_UMD;

        public const int maxMiningJobSlots = 20;

        public List<MiningJobSlot> miningJobSlots;

        public MiningBillStack MiningBillStack => miningBillStack;

        public bool triggerAutoSlotUpdate = false;

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
            compMEA = GetComp<CompMineEntranceAnim>();
            mapComp_UMD = map.GetComponent<MapComponent_UndergroundMineDeposits>();
            mapComp_UMD.RescanDeposits(compUMD.Props.mineables);
            miningJobSlots ??= new(maxMiningJobSlots);
            UpdateMiningJobSlots();
        }

        public override bool IsWorking()
        {
            return miningJobSlots.Any(x => x.currentMiner is not null);
        }

        public override void Tick()
        {
            base.Tick();
            if (triggerAutoSlotUpdate)
            {
                UpdateMiningJobSlots();
                triggerAutoSlotUpdate = false;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref miningBillStack, "miningBillStack");
            Scribe_Collections.Look(ref miningJobSlots, "miningJobSlots", LookMode.Deep);
        }

        public void UpdateMiningJobSlots()
        {
            if (MiningBillStack.AnyShouldDoNow)
            {
                MiningBill currentMiningBill = MiningBillStack.FirstShouldDoNow;
                int slotsToAdd = 0;

                if (miningJobSlots.FirstOrDefault() is MiningJobSlot firstSlot)
                {
                    // If existing mining job(s) are for the current active mining bill, add/remove additional jobs as needed
                    if (firstSlot.billLoadId == currentMiningBill.loadID)
                    {
                        // Do X times
                        if (currentMiningBill.repeatMode == BillRepeatModeDefOf.RepeatCount)
                        {
                            slotsToAdd = currentMiningBill.targetCount - miningJobSlots.Count;
                        }
                        // Do until you have X
                        else if (currentMiningBill.repeatMode == BillRepeatModeDefOf.TargetCount)
                        {
                            int originalTargetCount = currentMiningBill.minedPortionSize * miningJobSlots.Count;
                            double newTargetCount = (currentMiningBill.targetCount - originalTargetCount - UndergroundMineSys_Utils.CountMinedProductsOnMap(currentMiningBill)) / currentMiningBill.minedPortionSize;
                            slotsToAdd = (int)Math.Floor(newTargetCount);
                        }
                        // Do forever
                        else
                        {
                            slotsToAdd = maxMiningJobSlots - miningJobSlots.Count;
                        }
                    }
                    // Otherwise, clear the queue (and regenerate from scatch later)
                    else
                    {
                        miningJobSlots.Clear();
                    }
                }

                // If there are no mining jobs in the queue (reasons: never initialized, last mining bill just ended)
                if (miningJobSlots.Empty())
                {
                    // Do X times
                    if (currentMiningBill.repeatMode == BillRepeatModeDefOf.RepeatCount)
                    {
                        slotsToAdd = currentMiningBill.targetCount;
                    }
                    // Do until you have X
                    else if (currentMiningBill.repeatMode == BillRepeatModeDefOf.TargetCount)
                    {
                        int amountRequired = currentMiningBill.targetCount - UndergroundMineSys_Utils.CountMinedProductsOnMap(currentMiningBill);
                        slotsToAdd = (int)Math.Ceiling((float)amountRequired / (float)currentMiningBill.minedPortionSize);
                    }
                    // Do forever
                    else
                    {
                        slotsToAdd = maxMiningJobSlots;
                    }
                }

                // Cap number of slots generated up to max limit
                slotsToAdd = miningJobSlots.Count < maxMiningJobSlots ? Math.Min(slotsToAdd, maxMiningJobSlots) : 0;

                // Add or remove the required number of mining job slots
                if (slotsToAdd >= 0)
                {
                    for (int i = 0; i < slotsToAdd; i++)
                    {
                        miningJobSlots.Add(new(currentMiningBill.loadID, currentMiningBill.mineableThing));
                    }
                }
                else
                {
                    miningJobSlots.RemoveRange(miningJobSlots.Count - Math.Abs(slotsToAdd), Math.Abs(slotsToAdd));
                }
            }
            else
            {
                miningJobSlots.Clear();
            }
        }

        public MiningJobSlot GetMiningJobSlotForPawn(Pawn pawn)
        {
            return miningJobSlots.Empty() ? null : miningJobSlots.FirstOrDefault(x => x.currentMiner == pawn);
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append(base.GetInspectString());

            if (Prefs.DevMode && DebugSettings.godMode)
            {
                if (miningJobSlots.Count > 0)
                {
                    stringBuilder.Append($"Job slots: {miningJobSlots.Count(x => x.currentMiner is not null)} / {miningJobSlots.Count}\n");
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
        public Pawn currentMiner;
        public Pawn previousMiner;

        public bool Complete => progress >= UndergroundMineSys_Utils.GetWorkRequiredToMineResource(mineableThing);

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
            Scribe_References.Look(ref currentMiner, "currentMiner");
            Scribe_References.Look(ref previousMiner, "previousMiner");
        }
    }
}
