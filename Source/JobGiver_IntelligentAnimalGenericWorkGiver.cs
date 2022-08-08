using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    public class JobGiver_IntelligentAnimalGenericWorkGiver : ThinkNode
    {
        public WorkGiverDef workGiverDef;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_IntelligentAnimalGenericWorkGiver obj = (JobGiver_IntelligentAnimalGenericWorkGiver)base.DeepCopy(resolve);
            obj.workGiverDef = workGiverDef;
            return obj;
        }

		public override float GetPriority(Pawn pawn)
		{
			if (pawn.workSettings == null || !pawn.workSettings.EverWork)
			{
				return 9f;
			}
			TimeAssignmentDef timeAssignmentDef = (pawn.timetable != null) ? pawn.timetable.CurrentAssignment : TimeAssignmentDefOf.Anything;
			if (timeAssignmentDef == TimeAssignmentDefOf.Anything)
			{
				return 5.5f;
			}
			if (timeAssignmentDef == TimeAssignmentDefOf.Work)
			{
				return 9f;
			}
			if (timeAssignmentDef == TimeAssignmentDefOf.Sleep)
			{
				return 3f;
			}
			if (timeAssignmentDef != TimeAssignmentDefOf.Joy)
			{
				throw new NotImplementedException();
			}
			return 2f;
		}

		public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
		{
			if (pawn.mindState.priorityWork.IsPrioritized)
			{
				List<WorkGiverDef> workGiversByPriority = pawn.mindState.priorityWork.WorkGiver.workType.workGiversByPriority;
				for (int i = 0; i < workGiversByPriority.Count; i++)
				{
					WorkGiver worker = workGiversByPriority[i].Worker;
					Job job = GiverTryGiveJobPrioritized(pawn, worker, pawn.mindState.priorityWork.Cell);
					if (job != null)
					{
						job.playerForced = true;
						return new ThinkResult(job, this, workGiversByPriority[i].tagToGive);
					}
				}
				pawn.mindState.priorityWork.Clear();
			}
			int num = -999;
			TargetInfo targetInfo = TargetInfo.Invalid;
			WorkGiver_Scanner workGiver_Scanner = null;
			if (workGiverDef != null)
			{
				WorkGiver worker2 = workGiverDef.Worker;
				if (worker2.def.priorityInType != num && targetInfo.IsValid)
				{
					return ThinkResult.NoJob;
				}
				if (PawnCanUseWorkGiver(pawn, worker2))
				{
					try
					{
						Job job2 = worker2.NonScanJob(pawn);
						if (job2 != null)
						{
							return new ThinkResult(job2, this, workGiverDef.tagToGive);
						}
						WorkGiver_Scanner scanner = worker2 as WorkGiver_Scanner;
						if (scanner != null)
						{
							if (worker2.def.scanThings)
							{
								Predicate<Thing> predicate = (Thing t) => !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t);
								IEnumerable<Thing> enumerable = scanner.PotentialWorkThingsGlobal(pawn);
								Thing thing;
								if (scanner.Prioritized)
								{
									IEnumerable<Thing> enumerable2 = enumerable;
									if (enumerable2 == null)
									{
										enumerable2 = pawn.Map.listerThings.ThingsMatching(scanner.PotentialWorkThingRequest);
									}
									Predicate<Thing> validator = predicate;
									thing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, enumerable2, scanner.PathEndMode, TraverseParms.For(pawn), 9999f, validator, (Thing x) => scanner.GetPriority(pawn, x));
								}
								else
								{
									Predicate<Thing> validator2 = predicate;
									bool forceAllowGlobalSearch = enumerable != null;
									thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, scanner.PotentialWorkThingRequest, scanner.PathEndMode, TraverseParms.For(pawn), 9999f, validator2, enumerable, 0, scanner.MaxRegionsToScanBeforeGlobalSearch, forceAllowGlobalSearch);
								}
								if (thing != null)
								{
									targetInfo = thing;
									workGiver_Scanner = scanner;
								}
							}
							if (worker2.def.scanCells)
							{
								IntVec3 position = pawn.Position;
								float num2 = 99999f;
								float num3 = float.MinValue;
								bool prioritized = scanner.Prioritized;
								foreach (IntVec3 item in scanner.PotentialWorkCellsGlobal(pawn))
								{
									bool flag = false;
									float num4 = (item - position).LengthHorizontalSquared;
									if (prioritized)
									{
										if (!item.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, item))
										{
											float num5 = scanner.GetPriority(pawn, item);
											if (num5 > num3 || (num5 == num3 && num4 < num2))
											{
												flag = true;
												num3 = num5;
											}
										}
									}
									else if (num4 < num2 && !item.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, item))
									{
										flag = true;
									}
									if (flag)
									{
										targetInfo = new TargetInfo(item, pawn.Map);
										workGiver_Scanner = scanner;
										num2 = num4;
									}
								}
							}
						}
					}
					catch (Exception ex)
					{
						Log.Message(string.Concat(pawn, " threw exception in WorkGiver ", worker2.def.defName, ": ", ex.ToString()));
					}
					if (targetInfo.IsValid)
					{
						Job job3 = (!targetInfo.HasThing) ? workGiver_Scanner.JobOnCell(pawn, targetInfo.Cell) : workGiver_Scanner.JobOnThing(pawn, targetInfo.Thing);
						if (job3 != null)
						{
							return new ThinkResult(job3, this, workGiverDef.tagToGive);
						}
						Log.ErrorOnce(string.Concat(workGiver_Scanner, " provided target ", targetInfo, " but yielded no actual job for pawn ", pawn, ". The CanGiveJob and JobOnX methods may not be synchronized."), 6112651);
					}
					num = worker2.def.priorityInType;
				}
			}
			return ThinkResult.NoJob;
		}

		private bool PawnCanUseWorkGiver(Pawn pawn, WorkGiver giver)
		{
			if (!giver.ShouldSkip(pawn))
			{
				return giver.MissingRequiredCapacity(pawn) == null;
			}
			return false;
		}

		private Job GiverTryGiveJobPrioritized(Pawn pawn, WorkGiver giver, IntVec3 cell)
		{
			if (!PawnCanUseWorkGiver(pawn, giver))
			{
				return null;
			}
			try
			{
				Job job = giver.NonScanJob(pawn);
				if (job != null)
				{
					return job;
				}
				WorkGiver_Scanner scanner = giver as WorkGiver_Scanner;
				if (scanner != null)
				{
					if (giver.def.scanThings)
					{
						Predicate<Thing> predicate = (Thing t) => !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t);
						List<Thing> thingList = cell.GetThingList(pawn.Map);
						for (int i = 0; i < thingList.Count; i++)
						{
							Thing thing = thingList[i];
							if (scanner.PotentialWorkThingRequest.Accepts(thing) && predicate(thing))
							{
								Job job2 = scanner.JobOnThing(pawn, thing);
								if (job2 != null)
								{
									job2.workGiverDef = giver.def;
								}
								return job2;
							}
						}
					}
					if (giver.def.scanCells && !cell.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, cell))
					{
						Job job3 = scanner.JobOnCell(pawn, cell);
						if (job3 != null)
						{
							job3.workGiverDef = giver.def;
						}
						return job3;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error(string.Concat(pawn, " threw exception in GiverTryGiveJobTargeted on WorkGiver ", giver.def.defName, ": ", ex.ToString()));
			}
			return null;
		}
	}
}
