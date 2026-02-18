using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class JobDriver_CommitSuicideWithKnife : JobDriver_CommitSuicide
    {
        public override void ApplySuicideInjuryRepeatable(Pawn pawn)
        {
            BodyPartRecord bodyPartRecord = pawn.health.hediffSet.GetNotMissingParts().FirstOrDefault((BodyPartRecord x) => x.def == BodyPartDefOf.Heart);
            int num2 = Mathf.Clamp((int)pawn.health.hediffSet.GetPartHealth(bodyPartRecord) - 1, 1, 20);
            DamageInfo damageInfo = new DamageInfo(MousekinDefOf.Mousekin_SuicideKnife, num2, 999f, -1f, pawn, bodyPartRecord, instigatorGuilty: false);
            damageInfo.ignoreInstantKillProtectionInt = true;

            int num = Mathf.Max(GenMath.RoundRandom(pawn.BodySize * 8), 1);
            for (int i = 0; i < num; i++)
            {
                pawn.health.DropBloodFilth();
            }

            pawn.TakeDamage(damageInfo);
        }
    }
}
