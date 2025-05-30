using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class JobDriver_MineResourcesFromMineEntrance : JobDriver
    {
        public const TargetIndex MineEntranceInd = TargetIndex.A;

        public const int Duration = 200; // todo - temp, replace with mined resource-specific durations

        public Thing MineEntranceThing => job.GetTarget(MineEntranceInd).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(MineEntranceInd);
            this.FailOnBurningImmobile(MineEntranceInd);
            yield return Toils_Goto.GotoThing(MineEntranceInd, PathEndMode.Touch);
            yield return Toils_General.Wait(Duration).FailOnDestroyedNullOrForbidden(MineEntranceInd).FailOnCannotTouch(MineEntranceInd, PathEndMode.Touch)
                .FailOn(() => (MineEntranceThing as Building_MineEntrance).compUndergroundMineDeposits.MiningBill.ShouldDoNow() == false)
                .WithProgressBarToilDelay(MineEntranceInd);
            // todo - add more toils for mining
        }
    }
}
