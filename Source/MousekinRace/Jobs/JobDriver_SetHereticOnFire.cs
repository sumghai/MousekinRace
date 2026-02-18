using RimWorld;
using System.Collections.Generic;
using Verse.AI;
using Verse;

namespace MousekinRace
{
    public class JobDriver_SetHereticOnFire : JobDriver_Ignite
    {
        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnBurningImmobile(TargetIndex.A);
            Toil toil = ToilMaker.MakeToil("MakeNewToils");
            toil.initAction = delegate
            {
                TargetThingA.TryAttachFire(1, pawn);
            };
            yield return toil;
        }
    }
}
