using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class JobDriver_MineResourcesFromMineEntrance : JobDriver
    {
        public int miningBillStartTick;

        public int ticksSpentDoingMiningWork;
        
        public const TargetIndex MineEntranceInd = TargetIndex.A;

        public Building_MineEntrance MineEntrance => job.GetTarget(MineEntranceInd).Thing as Building_MineEntrance;

        public override string GetReport()
        {
            return base.GetReport();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref miningBillStartTick, "miningBillStartTick", 0);
            Scribe_Values.Look(ref ticksSpentDoingMiningWork, "ticksSpentDoingMiningWork", 0);
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(MineEntranceInd);
            this.FailOnBurningImmobile(MineEntranceInd);
            this.FailOn(() => !MineEntranceJobsAvailable(MineEntrance));
            yield return Toils_Goto.GotoThing(MineEntranceInd, PathEndMode.InteractionCell).FailOn(() => !MineEntranceJobsAvailable(MineEntrance));
            
            // Handles actual mining of resources inside the mine entrance building
            Toil mineResourcesToil = ToilMaker.MakeToil("Mousekin_Toil_MineResources");
            mineResourcesToil.initAction = () =>
            {
                Pawn miner = mineResourcesToil.actor;
                if (MineEntrance.miningJobSlots.Any()) {
                    MineEntrance.miningJobSlots.First(x => (x.currentMiner == null) || (x.currentMiner == miner)).currentMiner = miner;
                    miningBillStartTick = Find.TickManager.TicksGame;
                    ticksSpentDoingMiningWork = 0;
                }
            };
            mineResourcesToil.tickIntervalAction = (int delta) =>
            {
                Pawn miner = mineResourcesToil.actor;
                
                ticksSpentDoingMiningWork += delta;

                miner.skills?.Learn(SkillDefOf.Mining, 0.1f * (float)delta);

                float miningWorkSpeed = miner.GetStatValue(StatDefOf.MiningSpeed);

                if (DebugSettings.fastCrafting)
                {
                    miningWorkSpeed *= 30f;
                }

                if (MineEntrance.GetMiningJobSlotForPawn(miner) is MiningJobSlot miningJobSlot)
                {
                    miningJobSlot.progress += miningWorkSpeed * delta;

                    if (miningJobSlot.Complete)
                    {
                        ReadyForNextToil();
                    }
                    else
                    {
                        int ticksSinceMiningStarted = Find.TickManager.TicksGame - miningBillStartTick;
                        if (ticksSinceMiningStarted >= 3000 && ticksSinceMiningStarted % 1000 == 0)
                        {
                            miner.jobs.CheckForJobOverride();
                        }
                    }
                }
            };
            mineResourcesToil.AddFinishAction(() =>
            {
                Pawn miner = mineResourcesToil.actor;
                if (MineEntrance.GetMiningJobSlotForPawn(miner) is MiningJobSlot miningJobSlot)
                {
                    miningJobSlot.previousMiner = miner;
                    miningJobSlot.currentMiner = null;
                }
            });
            mineResourcesToil.defaultCompleteMode = ToilCompleteMode.Never;
            mineResourcesToil.FailOnDespawnedNullOrForbiddenPlacedThings(MineEntranceInd).FailOnCannotTouch(MineEntranceInd, PathEndMode.InteractionCell).FailOn(() => !MineEntranceJobsAvailable(MineEntrance));
            mineResourcesToil.PlaySustainerOrSound(() => MineEntrance.TryGetComp<CompMineEntranceAnim>().Props.soundWorking);
            yield return mineResourcesToil;

            // Handles generating and hauling of mined product to storage
            Toil storeMinedResourceToil = ToilMaker.MakeToil("Mousekin_Toil_StoreMinedResources");
            storeMinedResourceToil.initAction = () =>
            {
                Pawn miner = storeMinedResourceToil.actor;

                // Increase the miner's mining skill
                if (miner.skills != null)
                {
                    float xp = ticksSpentDoingMiningWork * 0.1f;
                    miner.skills.GetSkill(SkillDefOf.Mining).Learn(xp);
                }

                // Look for the finished slot whose last/previous miner is the current pawn
                // (emphasis on **finished**, as it is possible a pawn may have jumped between slots in the interim)
                MiningJobSlot finishedSlot = MineEntrance.miningJobSlots.First(x => x.previousMiner == miner && x.Complete);

                // Generate and extract the stack of mined resource 
                ThingDef minedThingDef = finishedSlot.mineableThing;
                Thing minedThing = ThingMaker.MakeThing(minedThingDef);
                minedThing.stackCount = MineEntrance.TryGetComp<CompUndergroundMineDeposits>().Props.mineables.First(x => x.mineableThing == minedThingDef).minedPortionSize;
                MineEntrance.mapComp_UMD.TryExtractResource(ref minedThing);

                // Notify various places regarding the mined resource
                MineEntrance.MiningBillStack.FirstShouldDoNow.Notify_IterationCompleted();
                RecordsUtility.Notify_BillDone(miner, [minedThing]);
                Find.QuestManager.Notify_ThingsProduced(miner, [minedThing]);
                if (UndergroundMineSys_Utils.GetWorkRequiredToMineResource(finishedSlot.mineableThing) >= 10000f)
                {
                    TaleRecorder.RecordTale(TaleDefOf.CompletedLongCraftingProject, miner, minedThingDef);
                }

                // Remove the now-completed mining job slot
                MineEntrance.miningJobSlots.Remove(finishedSlot);

                // 20% chance to drop a bonus product (with selection weight)
                if (Rand.RangeInclusive(0, 4) == 0)
                {
                    if (MineEntrance.TryGetComp<CompUndergroundMineDeposits>().Props.bonusMineables.RandomElementByWeight(x => x.selectionWeight) is BonusMineableOption bonusMineableOption)
                    {
                        ThingDef bonusThingDef = bonusMineableOption.bonusThing;
                        Thing bonusThing = ThingMaker.MakeThing(bonusThingDef);
                        GenPlace.TryPlaceThing(bonusThing, MineEntrance.InteractionCell, miner.Map, ThingPlaceMode.Near);
                    }
                }

                // Determine whether to drop the mined resource directly outside the mine, or get the miner to haul it to storage
                if (MineEntrance.MiningBillStack.mostRecentMiningBill.GetStoreMode() == BillStoreModeDefOf.DropOnFloor)
                {
                    if (!GenPlace.TryPlaceThing(minedThing, MineEntrance.InteractionCell, miner.Map, ThingPlaceMode.Near))
                    {
                        Log.Error($"{miner} could not drop recipe product {minedThing} near {MineEntrance.InteractionCell}");
                    }
                    miner.jobs.EndCurrentJob(JobCondition.Succeeded);
                }
                else
                {
                    IntVec3 foundCell = IntVec3.Invalid;
                    if (MineEntrance.MiningBillStack.mostRecentMiningBill.GetStoreMode() == BillStoreModeDefOf.BestStockpile)
                    {
                        StoreUtility.TryFindBestBetterStoreCellFor(minedThing, miner, miner.Map, StoragePriority.Unstored, miner.Faction, out foundCell);
                    }
                    else if (MineEntrance.MiningBillStack.mostRecentMiningBill.GetStoreMode() == BillStoreModeDefOf.SpecificStockpile)
                    {
                        StoreUtility.TryFindBestBetterStoreCellForIn(minedThing, miner, miner.Map, StoragePriority.Unstored, miner.Faction, MineEntrance.MiningBillStack.FirstShouldDoNow.GetSlotGroup(), out foundCell);
                    }
                    else
                    {
                        Log.ErrorOnce("Unknown store mode", 9158246);
                    }
                    if (foundCell.IsValid)
                    {
                        int num = miner.carryTracker.MaxStackSpaceEver(minedThingDef);
                        if (num < minedThing.stackCount)
                        {
                            int count = minedThing.stackCount - num;
                            Thing thing2 = minedThing.SplitOff(count);
                            if (!GenPlace.TryPlaceThing(thing2, miner.Position, miner.Map, ThingPlaceMode.Near))
                            {
                                Log.Error($"{miner} could not drop recipe extra product that pawn couldn't carry, {thing2} near {miner.Position}");
                            }
                        }
                        if (num == 0)
                        {
                            miner.jobs.EndCurrentJob(JobCondition.Succeeded);
                        }
                        else
                        {
                            miner.carryTracker.TryStartCarry(minedThing);
                            miner.jobs.StartJob(HaulAIUtility.HaulToCellStorageJob(miner, minedThing, foundCell, fitInStoreCell: false), JobCondition.Succeeded, null, resumeCurJobAfterwards: false, cancelBusyStances: true, null, null, fromQueue: false, canReturnCurJobToPool: false, true);
                        }
                    }
                    else
                    {
                        if (!GenPlace.TryPlaceThing(minedThing, miner.Position, miner.Map, ThingPlaceMode.Near))
                        {
                            Log.Error($"Bill doer could not drop product {minedThing} near {miner.Position}");
                        }
                        miner.jobs.EndCurrentJob(JobCondition.Succeeded);
                    }
                }
            };
            storeMinedResourceToil.AddFinishAction(() => 
            {
                Pawn miner = storeMinedResourceToil.actor;
                if (MineEntrance.MiningBillStack.mostRecentMiningBill.repeatMode == BillRepeatModeDefOf.TargetCount)
                {
                    miner.Map.resourceCounter.UpdateResourceCounts();
                }

                // Update mining job slots as needed
                MineEntrance.UpdateMiningJobSlots();
            });
            yield return storeMinedResourceToil;
        }

        public bool MineEntranceJobsAvailable(Building_MineEntrance mineEntrance)
        {
            return mineEntrance.MiningBillStack.AnyShouldDoNow && mineEntrance.UnassignedMiningJobSlotAvailable;
        }
    }
}
