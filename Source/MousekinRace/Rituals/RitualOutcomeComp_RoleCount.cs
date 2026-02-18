using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class RitualOutcomeComp_RoleCount : RitualOutcomeComp_ParticipantCount
    {
        public string roleId;

        public override float Count(LordJob_Ritual ritual, RitualOutcomeComp_Data data)
        {
            return ritual.assignments.AssignedPawns(roleId).Count();
        }

        public override QualityFactor GetQualityFactor(Precept_Ritual ritual, TargetInfo ritualTarget, RitualObligation obligation, RitualRoleAssignments assignments, RitualOutcomeComp_Data data)
        {
            int num = assignments.AssignedPawns(roleId).Count();
            float quality = curve.Evaluate(num);
            bool flag = (num > 0);
            return new QualityFactor
            {
                label = label.CapitalizeFirst(),
                count = num + " / " + Mathf.Max(base.MaxValue, num),
                qualityChange = ExpectedOffsetDesc(flag, quality),
                quality = (flag ? quality : 0f),
                positive = flag,
                priority = 10f
            };
        }
    }
}
