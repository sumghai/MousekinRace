using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class JobDriver_FertilizeCell : JobDriver
    {
        public static ThingDef fertilizerDef = MousekinDefOf.Mousekin_Saltpeter;

        public static DesignationDef desDef = MousekinDefOf.Mousekin_FertilizeSoil;

        public float fertilizingTime;

        public const TargetIndex FertilizingTargetIndex = TargetIndex.A;

        public const TargetIndex FertilizerIndex = TargetIndex.B;

        public const float FertilizingTimeSecondsBase = 1f;

        public IntVec3 FertilizingTarget => job.GetTarget(FertilizingTargetIndex).Cell;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            pawn.ReserveAsManyAsPossible(job.GetTargetQueue(FertilizingTargetIndex), job, 1, -1, ReservationLayerDefOf.Floor);
            pawn.ReserveAsManyAsPossible(job.GetTargetQueue(FertilizerIndex), job);
            return true;
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            Toil extractFromQueue = Toils_JobTransforms.ExtractNextTargetFromQueue(FertilizingTargetIndex);
            Toil checkFinished = Toils_JobTransforms.SucceedOnNoTargetInQueue(FertilizingTargetIndex);
            yield return Toils_Jump.JumpIf(extractFromQueue, () => job.GetTargetQueue(FertilizerIndex).NullOrEmpty());
            foreach (Toil item in CollectFertilizerToils())
            {
                yield return item;
            }
            yield return checkFinished;
            yield return extractFromQueue;

            yield return Toils_Goto.GotoCell(FertilizingTargetIndex, PathEndMode.Touch).JumpIf(() => Map.designationManager.DesignationAt(TargetLocA, desDef) == null, checkFinished);
            Toil toil = ToilMaker.MakeToil("MakeNewToils");
            toil.initAction = delegate
            {
                fertilizingTime = 0f;
            };
            toil.tickIntervalAction = delegate (int delta)
            {
                pawn.rotationTracker.FaceTarget(FertilizingTarget);
                fertilizingTime += pawn.GetStatValue(StatDefOf.WorkSpeedGlobal) / 60f * (float)delta;
                pawn.skills?.Learn(SkillDefOf.Plants, 0.1f * (float)delta);
                if (fertilizingTime >= 1f)
                {
                    pawn.carryTracker.CarriedThing?.SplitOff(1)?.Destroy();
                    Designation designation = Map.designationManager.DesignationAt(FertilizingTarget, desDef);
                    if (designation != null)
                    {
                        Map.GetComponent<MapComponent_Fertilizer>().cellFertilityBonus.AddDistinct(TargetLocA, fertilizerDef.GetModExtension<FertilizerExtension>().fertilityBonus);
                        Map.designationManager.RemoveDesignation(designation);
                    }
                    ReadyForNextToil();
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.WithEffect(MousekinDefOf.ExtinguisherPuffSmall, FertilizingTargetIndex);
            toil.WithProgressBar(FertilizingTargetIndex, () => fertilizingTime / 1f, interpolateBetweenActorAndTarget: true);
            toil.JumpIf(() => Map.designationManager.DesignationAt(TargetLocA, desDef) == null, checkFinished);
            toil.activeSkill = () => SkillDefOf.Plants;
            toil.handlingFacing = true;
            yield return toil;
            yield return Toils_Jump.Jump(checkFinished);
        }

        public IEnumerable<Toil> CollectFertilizerToils()
        {
            Toil extract = Toils_JobTransforms.ExtractNextTargetFromQueue(FertilizerIndex, failIfCountFromQueueTooBig: false);
            yield return extract;
            Toil jumpIfHaveTargetInQueue = Toils_Jump.JumpIfHaveTargetInQueue(FertilizerIndex, extract);
            yield return Toils_Goto.GotoThing(FertilizerIndex, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(FertilizerIndex).FailOnSomeonePhysicallyInteracting(FertilizerIndex);
            yield return Toils_Haul.StartCarryThing(FertilizerIndex, putRemainderInQueue: true);
            yield return jumpIfHaveTargetInQueue;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref fertilizingTime, "fertilizingTime", 0f);
        }
    }
}
