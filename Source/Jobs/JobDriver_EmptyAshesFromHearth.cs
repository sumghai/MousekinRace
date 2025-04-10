using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class JobDriver_EmptyAshesFromHearth : JobDriver
    {
        public const TargetIndex HearthInd = TargetIndex.A;

        public const TargetIndex AshesToHaulInd = TargetIndex.B;

        public const TargetIndex AshStorageCellInd = TargetIndex.C;

        public const int Duration = 200;

        public Thing AshMakerThing => job.GetTarget(HearthInd).Thing;

        public Thing Product => job.GetTarget(AshesToHaulInd).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(HearthInd), job, 1, -1, null, errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(HearthInd);
            this.FailOnBurningImmobile(HearthInd);
            yield return Toils_Goto.GotoThing(HearthInd, PathEndMode.Touch);
            yield return Toils_General.Wait(Duration).FailOnDestroyedNullOrForbidden(HearthInd).FailOnCannotTouch(HearthInd, PathEndMode.Touch)
                .FailOn(() => !(AshMakerThing.TryGetComp<CompAshMaker>()).IsFull)
                .WithProgressBarToilDelay(HearthInd);
            Toil toil = new()
            {
                initAction = delegate
                {
                    Thing ashes = AshMakerThing.TryGetComp<CompAshMaker>().DumpAsh();

                    GenPlace.TryPlaceThing(ashes, pawn.Position, base.Map, ThingPlaceMode.Near);

                    StoragePriority currentPriority = StoreUtility.CurrentStoragePriorityOf(ashes);
                    if (StoreUtility.TryFindBestBetterStoreCellFor(ashes, pawn, base.Map, currentPriority, pawn.Faction, out var foundCell))
                    {
                        job.SetTarget(AshStorageCellInd, foundCell);
                        job.SetTarget(AshesToHaulInd, ashes);
                        job.count = ashes.stackCount;
                    }
                    else
                    {
                        EndJobWith(JobCondition.Incompletable);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return toil;
            yield return Toils_Reserve.Reserve(AshesToHaulInd);
            yield return Toils_Reserve.Reserve(AshStorageCellInd);
            yield return Toils_Goto.GotoThing(AshesToHaulInd, PathEndMode.ClosestTouch);
            yield return Toils_Haul.StartCarryThing(AshesToHaulInd);
            Toil carryToCell = Toils_Haul.CarryHauledThingToCell(AshStorageCellInd);
            yield return carryToCell;
            yield return Toils_Haul.PlaceHauledThingInCell(AshStorageCellInd, carryToCell, storageMode: true);
        }
    }
}
