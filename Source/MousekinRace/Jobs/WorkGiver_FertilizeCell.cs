using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class WorkGiver_FertilizeCell : WorkGiver_Scanner
    {       
        public static List<Thing> tmpFertilizer = [];

        public static ThingDef fertilizerDef = MousekinDefOf.Mousekin_Saltpeter;

        public static DesignationDef desDef = MousekinDefOf.Mousekin_FertilizeSoil;

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return !pawn.Map.designationManager.AnySpawnedDesignationOfDef(desDef);
        }

        public override IEnumerable<IntVec3> PotentialWorkCellsGlobal(Pawn pawn)
        {
            foreach (Designation item in pawn.Map.designationManager.SpawnedDesignationsOfDef(desDef))
            {
                yield return item.target.Cell;
            }
        }

        public override bool HasJobOnCell(Pawn pawn, IntVec3 c, bool forced = false)
        {
            AcceptanceReport acceptanceReport = ShouldFertilizeCell(pawn, c, forced, checkFert: true);
            if (!acceptanceReport)
            {
                if (!acceptanceReport.Reason.NullOrEmpty())
                {
                    JobFailReason.Is(acceptanceReport.Reason);
                }
                return false;
            }
            return true;
        }

        public override Job JobOnCell(Pawn pawn, IntVec3 cell, bool forced = false)
        {
            tmpFertilizer.Clear();
            tmpFertilizer = FindNearbyFertilizer(pawn, forced);
            int stackCountFromThingList = ThingUtility.GetStackCountFromThingList(tmpFertilizer);
            if (!tmpFertilizer.Any())
            {
                return null;
            }
            tmpFertilizer.SortBy((Thing x) => x.Position.DistanceToSquared(cell));
            int num = 0;
            Job job = JobMaker.MakeJob(MousekinDefOf.Mousekin_Job_FertilizeSoil);
            job.AddQueuedTarget(TargetIndex.A, cell);
            job.AddQueuedTarget(TargetIndex.B, tmpFertilizer[num]);
            job.countQueue = [1];
            int num2 = Mathf.Min(10, stackCountFromThingList);
            for (int i = 0; i < 100; i++)
            {
                IntVec3 intVec = cell + GenRadial.RadialPattern[i];
                if (!intVec.InBounds(pawn.Map) || intVec.Fogged(pawn.Map) || !pawn.CanReach(intVec, PathEndMode.Touch, Danger.Deadly))
                {
                    continue;
                }
                if ((bool)ShouldFertilizeCell(pawn, intVec, forced, checkFert: false))
                {
                    if (job.targetQueueA.Contains(intVec))
                    {
                        continue;
                    }
                    job.AddQueuedTarget(TargetIndex.A, intVec);
                    job.countQueue[0]++;
                    if (job.countQueue[0] >= tmpFertilizer[num].stackCount)
                    {
                        num++;
                        if (num >= tmpFertilizer.Count)
                        {
                            break;
                        }
                        job.AddQueuedTarget(TargetIndex.B, tmpFertilizer[num]);
                    }
                }
                if (job.GetTargetQueue(TargetIndex.A).Count >= num2)
                {
                    break;
                }
            }
            if (job.targetQueueA != null && job.targetQueueA.Count >= 5)
            {
                job.targetQueueA.SortBy((LocalTargetInfo targ) => targ.Cell.DistanceToSquared(pawn.Position));
            }
            return job;
        }

        public AcceptanceReport ShouldFertilizeCell(Pawn pawn, IntVec3 c, bool forced, bool checkFert)
        {
            if (c.GetZone(pawn.Map) is not Zone_Growing)
            {
                return false;
            }

            if (pawn.Map.GetComponent<MapComponent_Fertilizer>().cellFertilityBonus.ContainsKey(c))
            {
                return false;
            }

            if (pawn.Map.designationManager.DesignationAt(c, desDef) == null)
            {
                return false;
            }

            if (!pawn.CanReserve(c, 1, -1, ReservationLayerDefOf.Floor, forced))
            {
                return false;
            }
            if (checkFert)
            {
                List<Thing> list = pawn.Map.listerThings.ThingsOfDef(fertilizerDef);
                for (int i = 0; i < list.Count; i++)
                {
                    if (!list[i].IsForbidden(pawn) && pawn.CanReserveAndReach(list[i], PathEndMode.ClosestTouch, Danger.Deadly, 1, 1, null, forced))
                    {
                        return true;
                    }
                }
                return "NoIngredient".Translate(fertilizerDef);
            }
            return true;
        }

        public static List<Thing> FindNearbyFertilizer(Pawn pawn, bool forced = false)
        {
            List<Thing> list = [];
            List<Thing> list2 = pawn.Map.listerThings.ThingsOfDef(fertilizerDef);
            for (int i = 0; i < list2.Count; i++)
            {
                if (!list2[i].IsForbidden(pawn) && pawn.CanReserveAndReach(list2[i], PathEndMode.ClosestTouch, Danger.Deadly, 1, -1, null, forced))
                {
                    list.Add(list2[i]);
                }
            }
            return list;
        }
    }
}
