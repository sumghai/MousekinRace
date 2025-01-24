using RimWorld;
using Verse;

namespace MousekinRace
{
    public class RitualRoleNun : RitualRole
    {
        public override bool AppliesToPawn(Pawn p, out string reason, TargetInfo selectedTarget, LordJob_Ritual ritual = null, RitualRoleAssignments assignments = null, Precept_Ritual precept = null, bool skipReason = false)
        {
            if (base.AppliesToPawn(p, out reason, selectedTarget, ritual, assignments, precept, skipReason))
            { 
                return false;
            }

            if (p.kindDef != MousekinDefOf.MousekinNun) 
            {
                if (!skipReason) 
                {
                    reason = "MousekinRace_MessageRitualRoleMustBeMousekinNun".Translate(p, MousekinDefOf.MousekinNun.LabelCap);
                }
                return false;
            }

            return true;
        }

        public override bool AppliesToRole(Precept_Role role, out string reason, Precept_Ritual ritual = null, Pawn pawn = null, bool skipReason = false)
        {
            reason = null;
            return false;
        }
    }
}
