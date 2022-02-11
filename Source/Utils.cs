using AlienRace;
using RimWorld;
using System.Linq;
using Verse;

namespace MousekinRace
{
    public class Utils
    {      
        //Determine whether a given pawn or pawnKindDef is a Mousekin
        public static bool IsMousekin(Pawn pawn)
        {
            return pawn.def.Equals(MousekinDefOf.Mousekin);
        }

        public static bool IsMousekin(PawnKindDef pawnKindDef)
        {
            return (pawnKindDef != null) ? pawnKindDef.race.Equals(MousekinDefOf.Mousekin) : false;
        }

        //Get the primary race of any given faction
        public static ThingDef_AlienRace GetRaceOfFaction(FactionDef faction) => (faction.basicMemberKind?.race ?? faction.pawnGroupMakers?.SelectMany(selector: groupMaker => groupMaker.options).GroupBy(keySelector: groupMaker => groupMaker.kind.race).OrderByDescending(keySelector: g => g.Count()).First().Key) as ThingDef_AlienRace;

    }
}
