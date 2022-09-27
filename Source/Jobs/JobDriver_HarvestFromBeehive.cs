using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class JobDriver_HarvestFromBeehive : JobDriver
    {
        public const TargetIndex BeehiveInd = TargetIndex.A;

        public const TargetIndex ProductToHaulInd = TargetIndex.B;

        public const TargetIndex ProductStorageCellInd = TargetIndex.C;

        public const int Duration = 200;

        public Building_Beehive Beehive => (Building_Beehive)job.GetTarget(TargetIndex.A).Thing;

        public Thing Product => job.GetTarget(TargetIndex.B).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(Beehive, job, 1, -1, null, errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(Duration).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
                .FailOn(() => !Beehive.ProductsReady)
                .WithProgressBarToilDelay(TargetIndex.A);
            Toil toil = new Toil();

            // Building drops both items
            // Pawn only immediately hauls the first one if suitable storage exists

            toil.initAction = delegate
            {
                List<Thing> products = Beehive.TakeOutProducts();

                foreach (Thing currProduct in products)
                {
                    GenPlace.TryPlaceThing(currProduct, pawn.Position, base.Map, ThingPlaceMode.Near);
                }

                StoragePriority currentPriority = StoreUtility.CurrentStoragePriorityOf(products[0]);
                if (StoreUtility.TryFindBestBetterStoreCellFor(products[0], pawn, base.Map, currentPriority, pawn.Faction, out var foundCell))
                {
                    job.SetTarget(TargetIndex.C, foundCell);
                    job.SetTarget(TargetIndex.B, products[0]);
                    job.count = products[0].stackCount;
                }
                else
                {
                    EndJobWith(JobCondition.Incompletable);
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return toil;
            yield return Toils_Reserve.Reserve(TargetIndex.B);
            yield return Toils_Reserve.Reserve(TargetIndex.C);
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B);
            Toil carryToCell = Toils_Haul.CarryHauledThingToCell(TargetIndex.C);
            yield return carryToCell;
            yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.C, carryToCell, storageMode: true);
        }
    }
}
