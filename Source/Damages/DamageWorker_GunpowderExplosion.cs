using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class DamageWorker_GunpowderExplosion : DamageWorker_Flame
    {
		public override void ExplosionAffectCell(Explosion explosion, IntVec3 c, List<Thing> damagedThings, List<Thing> ignoredThings, bool canThrowMotes)
		{
			base.ExplosionAffectCell(explosion, c, damagedThings, ignoredThings, canThrowMotes);
			if (def == MousekinDefOf.Mousekin_GunpowderExplosion && Rand.Chance(FireUtility.ChanceToStartFireIn(c, explosion.Map)))
			{
				FireUtility.TryStartFireIn(c, explosion.Map, Rand.Range(0.2f, 0.6f));
			}
		}
	}
}
