using RimWorld;
using Verse;

namespace MousekinRace
{
    public class RitualRoleColonistNonHeretic : RitualRoleColonist
    {
        public override bool AppliesToPawn(Pawn p, out string reason, TargetInfo selectedTarget, LordJob_Ritual ritual = null, RitualRoleAssignments assignments = null, Precept_Ritual precept = null, bool skipReason = false)
        {
            if (!base.AppliesToPawn(p, out reason, selectedTarget, ritual, assignments, precept, skipReason))
            {
                return false;
            }

            if (p.IsHeretic())
            {
                if (!skipReason)
                {
                    reason = "MousekinRace_MessageRitualRoleMustBeNonHeretic".Translate(base.Label);
                }
                return false;
            }

            return true;
        }
    }
}
