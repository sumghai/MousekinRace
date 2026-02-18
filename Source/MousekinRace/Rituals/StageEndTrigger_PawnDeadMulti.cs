using RimWorld;
using System.Collections.Generic;
using Verse.AI.Group;
using Verse;

namespace MousekinRace
{
    public class StageEndTrigger_PawnDeadMulti : StageEndTrigger_AnyPawnDead
    {
        public override Trigger MakeTrigger(LordJob_Ritual ritual, TargetInfo spot, IEnumerable<TargetInfo> foci, RitualStage stage)
        {
            return new Trigger_TickCondition(delegate () 
            {
                foreach (string roleId in roleIds)
                {
                    foreach (Pawn pawn in ritual.assignments.AssignedPawns(roleId))
                    {
                        if (!pawn.Dead)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }, 300);
        }
    }
}
