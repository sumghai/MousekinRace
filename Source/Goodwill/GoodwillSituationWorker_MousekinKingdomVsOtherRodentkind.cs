using RimWorld;
using System.Collections.Generic;

namespace MousekinRace
{
    public class GoodwillSituationWorker_MousekinKingdomVsOtherRodentkind : GoodwillSituationWorker
    {
		public override int GetNaturalGoodwillOffset(Faction other)
		{
			if (Applies(other))
			{
				return def.naturalGoodwillOffset;
			}
			return 0;
		}

		private bool Applies(Faction other)
		{
			return Applies(Faction.OfPlayer, other) || Applies(other, Faction.OfPlayer);
		}

		private bool Applies(Faction a, Faction b)
		{
			OtherRodentRacesExtension otherRodentRaces = MousekinDefOf.Mousekin.GetModExtension<OtherRodentRacesExtension>();
			List<AlienRace.ThingDef_AlienRace> differentRodentRaces = otherRodentRaces.differentRodentRaces;
			List<AlienRace.ThingDef_AlienRace> hostileRodentRaces = otherRodentRaces.hostileRodentRaces;

			AlienRace.ThingDef_AlienRace raceOfFactionB = Utils.GetRaceOfFaction(b.def);

			return (a.def == MousekinDefOf.Mousekin_FactionKingdom || (a.ideos?.PrimaryIdeo?.memes.Contains(MousekinDefOf.Mousekin_IdeoMeme_RodentPurity) ?? false)) && (differentRodentRaces.Contains(raceOfFactionB) || hostileRodentRaces.Contains(raceOfFactionB));
		}
	}
}
