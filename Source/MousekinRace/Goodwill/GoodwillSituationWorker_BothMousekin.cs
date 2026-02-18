using RimWorld;

namespace MousekinRace
{
    public class GoodwillSituationWorker_BothMousekin : GoodwillSituationWorker
    {
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
            return Utils.IsMousekin(a.def.basicMemberKind) && Utils.IsMousekin(b.def.basicMemberKind);
        }
    }
}
