using RimWorld;
using Verse;

namespace GrimalkinRace
{
    public static class Utils
    {
        // Determine whether a given pawn or pawnKindDef is a Grimalkin
        public static bool IsGrimalkin(this Pawn pawn)
        {
            return pawn.def.Equals(GrimalkinDefOf.Grimalkin);
        }

        public static bool IsGrimalkin(this PawnKindDef pawnKindDef)
        {
            return (pawnKindDef != null) ? pawnKindDef.race.Equals(GrimalkinDefOf.Grimalkin) : false;
        }

        // Determine if a faction's ideo/culture is Grimalkin
        public static bool IsGrimalkin(this CultureDef culture)
        {
            return culture.defName.Contains("Grimalkin");
        }

        // Determine whether a given pawn or pawnKindDef is a Mousekin
        // (copied from MousekinRace assembly for redundancy)
        public static bool IsMousekin(this Pawn pawn)
        {
            return pawn.def.Equals(GrimalkinDefOf.Mousekin);
        }

        public static bool IsMousekin(this PawnKindDef pawnKindDef)
        {
            return (pawnKindDef != null) && pawnKindDef.race.Equals(GrimalkinDefOf.Mousekin);
        }
    }
}
