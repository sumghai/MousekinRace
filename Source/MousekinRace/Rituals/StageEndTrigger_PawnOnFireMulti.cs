using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace MousekinRace
{
    public class StageEndTrigger_PawnOnFireMulti : StageEndTrigger
    {
        [NoTranslate]
        public List<string> roleIds;

        public override Trigger MakeTrigger(LordJob_Ritual ritual, TargetInfo spot, IEnumerable<TargetInfo> foci, RitualStage stage)
        {
            return new Trigger_TickCondition(delegate ()
            {
                foreach (string roleId in roleIds)
                {
                    foreach (Pawn pawn in ritual.assignments.AssignedPawns(roleId))
                    {
                        if (!pawn.HasAttachment(ThingDefOf.Fire))
                        {
                            return false;
                        }
                    }
                }
                return true;
            });
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref roleIds, "roleIds", LookMode.Value);
        }
    }
}
