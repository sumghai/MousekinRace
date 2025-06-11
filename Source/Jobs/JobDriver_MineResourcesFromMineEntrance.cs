using System.Collections.Generic;
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
            this.FailOn(() => !MineEntrance.UnassignedMiningJobSlotAvailable);
            yield return Toils_Goto.GotoThing(MineEntranceInd, PathEndMode.InteractionCell).FailOn(() => !MineEntrance.UnassignedMiningJobSlotAvailable);
            yield return Toils_MineEntrance.MineResource().FailOnDespawnedNullOrForbiddenPlacedThings(MineEntranceInd).FailOnCannotTouch(MineEntranceInd, PathEndMode.InteractionCell).FailOn(() => !MineEntrance.UnassignedMiningJobSlotAvailable);
            yield return Toils_MineEntrance.FinishMiningAndStartStoringProduct();
        }
    }
}
