using Verse;

namespace MousekinRace
{
    public class JobDriver_CommitSuicideWithPoison : JobDriver_CommitSuicide
    {
        public override void ApplySuicideInjurySingle(Pawn pawn)
        {
            DamageInfo damageInfo = new DamageInfo(MousekinDefOf.Mousekin_SuicidePoison, 0.1f, instigatorGuilty: false);
            pawn.TakeDamage(damageInfo);
        }
    }
}
