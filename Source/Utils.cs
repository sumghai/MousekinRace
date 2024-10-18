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

        // Determine if a faction's ideo/culture is Mousekin
        public static bool IsMousekin(this CultureDef culture)
        {
            return culture.defName.Contains("Mousekin");
        }

        // Determine if a faction's ideo/culture is (based on) the Mousekin Kingdom
        // (applies to both default Mousekin player and NPC Mousekin Kingdom faction
        public static bool IsMousekinKingdomLike(this CultureDef culture)
        {
            return culture.IsMousekin() && culture.defName.Contains("Kingdom");
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

        // Replace the leader and moral guide ideo role titles for Mousekin Player faction
        public static string ReplaceIdeoRoleTitlesForMousekinPlayer(string originalString, Precept_Role role)
        {
            if (role != null && role.ideo.culture.IsMousekin())
            {
                string tempOutput = originalString;
                if (role.def.leaderRole)
                {
                    originalString = tempOutput.Replace(role.LabelCap, "MousekinRace_PreceptRole_PlayerLeaderTitle".Translate());
                }
                if (role.ideo.culture.IsMousekinKingdomLike() && role.def == PreceptDefOf.IdeoRole_Moralist)
                {
                    originalString = tempOutput.Replace(role.LabelCap, MousekinDefOf.MousekinPriest.label.Replace(MousekinDefOf.Mousekin.label, "").Trim().CapitalizeFirst());
                }
            }
            return originalString;
        }
    }
}
