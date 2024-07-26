using AlienRace;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    public static class Utils
    {
        // Determine whether a given pawn or pawnKindDef is a Mousekin
        public static bool IsMousekin(this Pawn pawn)
        {
            return pawn.def.Equals(MousekinDefOf.Mousekin);
        }

        public static bool IsMousekin(this PawnKindDef pawnKindDef)
        {
            return (pawnKindDef != null) ? pawnKindDef.race.Equals(MousekinDefOf.Mousekin) : false;
        }

        // Get the primary race of any given faction
        public static ThingDef_AlienRace GetRaceOfFaction(FactionDef faction) => (faction.basicMemberKind?.race ?? faction.pawnGroupMakers?.SelectMany(selector: groupMaker => groupMaker.options).GroupBy(keySelector: groupMaker => groupMaker.kind.race).OrderByDescending(keySelector: g => g.Count()).First().Key) as ThingDef_AlienRace;

        // Get percentage of player faction free colonists that are Mousekins
        public static float PercentColonistsAreMousekins()
        {
            List<Pawn> allColonists = PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists.Where(p => !p.IsSlave && !p.IsPrisoner).ToList();
            int playerFactionTotalColonistCount = allColonists.Count();
            int playerFactionMousekinColonistCount = allColonists.Where(p => IsMousekin(p)).Count();
            return (playerFactionTotalColonistCount == 0) ? 0 : (float) playerFactionMousekinColonistCount / playerFactionTotalColonistCount;
        }

        // Return a human-readable year with "year" being conditionally pluralized
        public static string YearHumanReadable(float years)
        {
            return (years != 1.0f) ? "PeriodYears".Translate(years.ToString("0.#")) : "Period1Year".Translate();
        }
    }
}
