using RimWorld;
using System.Collections.Generic;
using Verse.AI;
using Verse;

namespace MousekinRace
{
    // Variant of the regular JobDriver_Spectate, which forces the spectating pawn to always face the lectern
    class JobDriver_ChurchServiceAttendSermon : JobDriver_Spectate
    {
        public override IEnumerable<Toil> MakeNewToils()
        {
            if (job.GetTarget(ChairInd).HasThing)
            {
                this.EndOnDespawnedOrNull(ChairInd);
            }

            yield return Toils_Goto.GotoCell(MySpotOrChairInd, PathEndMode.OnCell);
            Toil toil = ToilMaker.MakeToil("MakeNewToils");
            toil.tickAction = delegate
            {
                Building churchLectern = TargetThingB as Building;
                pawn.Rotation = churchLectern.Rotation.Opposite;
                pawn.GainComfortFromCellIfPossible();

                if (pawn.IsHashIntervalTick(100))
                {
                    pawn.jobs.CheckForJobOverride();
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.handlingFacing = true;
            yield return toil;
        }
    }
}
