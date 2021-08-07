using RimWorld;
using Verse;

namespace MousekinRace
{
    [DefOf]
    public static class MousekinDefOf
    {
        public static ThingDef Mousekin;

        public static ThingDef Meat_Chicken;
        public static ThingDef Meat_Cow;
        public static ThingDef Meat_Deer;
        public static ThingDef Meat_Duck;
        public static ThingDef Meat_Goat;
        public static ThingDef Meat_Pig;
        public static ThingDef Meat_Sheep;

        public static TraitDef Mousekin_TraitSpectrum_Faith;

        public static BodyPartDef Mousekin_Ear;

        public static HediffDef Mousekin_ProstheticClothEar;
        public static HediffDef Mousekin_HemlockPoisoning;

        public static MentalStateDef Mousekin_MentalState_EarlessSuicide;

        public static DamageDef Mousekin_SuicideKnife;
        public static DamageDef Mousekin_SuicidePoison;

        public static JobDef Mousekin_Job_CommitSuicideWithKnife;
        public static JobDef Mousekin_Job_CommitSuicideWithPoison;
    }
}
