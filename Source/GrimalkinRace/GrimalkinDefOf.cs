using RimWorld;
using Verse;

namespace GrimalkinRace
{
    [DefOf]
    public static class GrimalkinDefOf
    {
        public static ThingDef Grimalkin;
        public static ThingDef Mousekin;

        public static ThoughtDef Grimalkin_Thought_KilledMousekin;

        public static FactionDef Grimalkin_Faction;

        [MayRequireIdeology]
        public static RulePackDef NamerRoleMoralist_Grimalkin;
    }
}
