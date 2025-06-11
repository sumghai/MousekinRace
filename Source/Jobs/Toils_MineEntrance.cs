using RimWorld;
using System;
using System.Linq;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    public static class Toils_MineEntrance
    {

        public static Toil MineResource()
        {
            Toil toil = ToilMaker.MakeToil("MousekinRace_MineResource");
            Pawn miner = toil.actor;

            toil.initAction = () =>
            {
                JobDriver_MineResourcesFromMineEntrance jobDriver_MineResources = (JobDriver_MineResourcesFromMineEntrance)miner.jobs.curDriver;
                Building_MineEntrance mineEntrance = jobDriver_MineResources.MineEntrance;
                mineEntrance.miningJobSlots.First(x => (x.currentMiner == null) || (x.currentMiner == miner)).currentMiner = miner;
                jobDriver_MineResources.miningBillStartTick = Find.TickManager.TicksGame;
                jobDriver_MineResources.ticksSpentDoingMiningWork = 0;
            };

            toil.tickAction = () =>
            {
                JobDriver_MineResourcesFromMineEntrance jobDriver_MineResources = (JobDriver_MineResourcesFromMineEntrance)miner.jobs.curDriver;
                Building_MineEntrance mineEntrance = jobDriver_MineResources.MineEntrance;

                jobDriver_MineResources.ticksSpentDoingMiningWork++;

                miner.skills?.Learn(SkillDefOf.Mining, 0.1f);

                float miningWorkSpeed = miner.GetStatValue(StatDefOf.MiningSpeed);

                if (DebugSettings.fastCrafting)
                {
                    miningWorkSpeed *= 30f;
                }

                mineEntrance.GetMiningJobSlotForPawn(miner).progress += miningWorkSpeed;

                if (mineEntrance.GetMiningJobSlotForPawn(miner).progress >= UndergroundMineSys_Utils.GetWorkRequiredToMineResource(mineEntrance.GetMiningJobSlotForPawn(miner).mineableThing))
                {
                    mineEntrance.GetMiningJobSlotForPawn(miner).complete = true;
                    jobDriver_MineResources.ReadyForNextToil();
                }
                else
                {
                    int ticksSinceMiningStarted = Find.TickManager.TicksGame - jobDriver_MineResources.miningBillStartTick;
                    if (ticksSinceMiningStarted >= 3000 && ticksSinceMiningStarted % 1000 == 0)
                    {
                        miner.jobs.CheckForJobOverride();
                    }
                }
            };

            toil.AddFinishAction(() => 
            {
                JobDriver_MineResourcesFromMineEntrance jobDriver_MineResources = (JobDriver_MineResourcesFromMineEntrance)miner.jobs.curDriver;
                Building_MineEntrance mineEntrance = jobDriver_MineResources.MineEntrance;

                mineEntrance.GetMiningJobSlotForPawn(miner).previousMiner = miner;
                mineEntrance.GetMiningJobSlotForPawn(miner).currentMiner = null;
            });

            toil.defaultCompleteMode = ToilCompleteMode.Never;

            // todo - toil.PlaySustainerOrSound(() => mining sound?);

            toil.FailOn((Func<bool>)delegate
            {
                JobDriver_MineResourcesFromMineEntrance jobDriver_MineResources = (JobDriver_MineResourcesFromMineEntrance)miner.jobs.curDriver;
                Building_MineEntrance mineEntrance = jobDriver_MineResources.MineEntrance;

                return mineEntrance.MiningBillStack.FirstCanDo.suspended || !mineEntrance.UnassignedMiningJobSlotAvailable;
            });

            return toil;
        }

        public static Toil FinishMiningAndStartStoringProduct()
        {
            Toil toil = ToilMaker.MakeToil("MousekinRace_FinishMiningAndStartStoringProduct");
            Pawn miner = toil.actor;

            toil.initAction = () =>
            {
                JobDriver_MineResourcesFromMineEntrance jobDriver_MineResources = (JobDriver_MineResourcesFromMineEntrance)miner.jobs.curDriver;
                Building_MineEntrance mineEntrance = jobDriver_MineResources.MineEntrance;

                if (miner.skills != null)
                {
                    float xp = (float)jobDriver_MineResources.ticksSpentDoingMiningWork * 0.1f;
                    miner.skills.GetSkill(SkillDefOf.Mining).Learn(xp);
                }

                // Look for the finished slot whose last/previous miner is the current pawn
                // (emphasis on **finished**, as it is possible a pawn may have jumped between slots in the interim)
                MiningJobSlot finishedSlot = mineEntrance.miningJobSlots.First(x => x.previousMiner == miner && x.complete);

                ThingDef minedThingDef = finishedSlot.mineableThing;
                Thing minedThing = ThingMaker.MakeThing(minedThingDef);
                minedThing.stackCount = mineEntrance.TryGetComp<CompUndergroundMineDeposits>().Props.mineables.First(x => x.mineableThing == minedThingDef).minedPortionSize;

                mineEntrance.MiningBillStack.FirstCanDo.Notify_IterationCompleted(miner);
                RecordsUtility.Notify_BillDone(miner, [minedThing]);

                if (UndergroundMineSys_Utils.GetWorkRequiredToMineResource(finishedSlot.mineableThing) >= 10000f)
                {
                    TaleRecorder.RecordTale(TaleDefOf.CompletedLongCraftingProject, miner, minedThingDef);
                }

                mineEntrance.miningJobSlots.Remove(finishedSlot);

                Find.QuestManager.Notify_ThingsProduced(miner, [minedThing]);

                if (mineEntrance.MiningBillStack.FirstCanDo.GetStoreMode() == BillStoreModeDefOf.DropOnFloor)
                {
                    if (!GenPlace.TryPlaceThing(minedThing, mineEntrance.InteractionCell, miner.Map, ThingPlaceMode.Near))
                    {
                        Log.Error($"{miner} could not drop recipe product {minedThing} near {mineEntrance.InteractionCell}");
                    }
                    miner.jobs.EndCurrentJob(JobCondition.Succeeded);
                }
                else
                {
                    IntVec3 foundCell = IntVec3.Invalid;
                    if (mineEntrance.MiningBillStack.FirstCanDo.GetStoreMode() == BillStoreModeDefOf.BestStockpile)
                    {
                        StoreUtility.TryFindBestBetterStoreCellFor(minedThing, miner, miner.Map, StoragePriority.Unstored, miner.Faction, out foundCell);
                    }
                    else if (mineEntrance.MiningBillStack.FirstCanDo.GetStoreMode() == BillStoreModeDefOf.SpecificStockpile)
                    {
                        StoreUtility.TryFindBestBetterStoreCellForIn(minedThing, miner, miner.Map, StoragePriority.Unstored, miner.Faction, mineEntrance.MiningBillStack.FirstCanDo.GetSlotGroup(), out foundCell);
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

            toil.AddFinishAction(() =>
            {
                JobDriver_MineResourcesFromMineEntrance jobDriver_MineResources = (JobDriver_MineResourcesFromMineEntrance)miner.jobs.curDriver;
                Building_MineEntrance mineEntrance = jobDriver_MineResources.MineEntrance;

                if (mineEntrance.MiningBillStack.FirstCanDo.repeatMode == BillRepeatModeDefOf.TargetCount)
                {
                    miner.Map.resourceCounter.UpdateResourceCounts();
                }
            });

            return toil;
        }
    }
}
