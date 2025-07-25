﻿using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace MousekinRace
{
    public class JobDriver_EatAtBarbecueTable : JobDriver
    {
        public const TargetIndex BarbecueTableIndex = TargetIndex.A;

        public const TargetIndex CellIndex = TargetIndex.B;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.ReserveSittableOrSpot(job.targetB.Cell, job, errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.EndOnDespawnedOrNull(BarbecueTableIndex);
            yield return Toils_Goto.Goto(CellIndex, PathEndMode.OnCell);
            float totalBuildingNutrition = base.TargetA.Thing.def.CostList.Sum((ThingDefCountClass x) => x.thingDef.GetStatValueAbstract(StatDefOf.Nutrition) * (float)x.count);
            Toil eat = ToilMaker.MakeToil("MakeNewToils");
            eat.tickIntervalAction = delegate (int delta)
            {
                pawn.rotationTracker.FaceCell(base.TargetA.Thing.OccupiedRect().ClosestCellTo(pawn.Position));
                pawn.GainComfortFromCellIfPossible(delta);
                if (pawn.needs.food != null)
                {
                    pawn.needs.food.CurLevel += totalBuildingNutrition / (float)pawn.GetLord().ownedPawns.Count / (float)eat.defaultDuration;
                }
            };
            eat.AddFinishAction(delegate
            {
                if (pawn.needs?.mood?.thoughts != null)
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(MousekinDefOf.AteLavishMeal);
                }
            });
            eat.WithEffect(EffecterDefOf.EatMeat, BarbecueTableIndex);
            eat.PlaySustainerOrSound(MousekinDefOf.Meal_Eat);
            eat.handlingFacing = true;
            eat.defaultCompleteMode = ToilCompleteMode.Delay;
            eat.defaultDuration = (job.doUntilGatheringEnded ? job.expiryInterval : job.def.joyDuration);
            yield return eat;
        }
    }
}