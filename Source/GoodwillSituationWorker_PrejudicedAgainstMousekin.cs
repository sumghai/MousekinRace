﻿using RimWorld;
using Verse;

namespace MousekinRace
{
    public class GoodwillSituationWorker_PrejudicedAgainstMousekin : GoodwillSituationWorker
    {
        public override string GetPostProcessedLabel(Faction other)
        {
			if (Applies(Faction.OfPlayer, other))
			{
				return "MemeGoodwillImpact_Other".Translate(base.GetPostProcessedLabel(other));
			}
			return "MemeGoodwillImpact_Player".Translate(base.GetPostProcessedLabel(other));
		}

        public override int GetNaturalGoodwillOffset(Faction other)
		{
			if (!Applies(other))
			{
				return 0;
			}
			return def.naturalGoodwillOffset;
		}

		private bool Applies(Faction other)
		{
			return Applies(Faction.OfPlayer, other) || Applies(other, Faction.OfPlayer);
		}

		private bool Applies(Faction a, Faction b)
		{
			return Utils.IsMousekin(a.def.basicMemberKind) && !Utils.IsMousekin(b.def.basicMemberKind);
		}
	}
}
