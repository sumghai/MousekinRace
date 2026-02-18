using RimWorld;
using System.Collections.Generic;
using Verse.AI.Group;
using Verse;

namespace MousekinRace
{
    public class StageEndTrigger_RolesArrivedMulti : StageEndTrigger
    {
        public List<string> roleIds;

        public override Trigger MakeTrigger(LordJob_Ritual ritual, TargetInfo spot, IEnumerable<TargetInfo> foci, RitualStage stage)
        {
            Trigger_Custom trigger = new(delegate (TriggerSignal signal)
            {
                foreach (string roleId in roleIds) 
                {
                    foreach (Pawn pawn in ritual.assignments.AssignedPawns(roleId))
                    {
                        if (!ritual.PawnTagSet(pawn, "Arrived"))
                        {
                            return false;
                        }
                    }
                }
                return true;
            });
            return trigger;
        }
    }
}
