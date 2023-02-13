using RimWorld;
using Verse;

namespace MousekinRace
{
    [DefOf]
    public static class MousekinDefOf
    {
        public static ThingDef Mousekin;

        public static ThingDef Mousekin_AnimalPudgemouse;

        public static ThingDef Meat_Chicken;
        public static ThingDef Meat_Cow;
        public static ThingDef Meat_Deer;
        public static ThingDef Meat_Duck;
        public static ThingDef Meat_Goat;
        public static ThingDef Meat_Pig;
        public static ThingDef Meat_Sheep;

        public static ThingDef Mousekin_Beehive;
        public static ThingDef Mousekin_Windmill;
        public static ThingDef Blueprint_Mousekin_Windmill;

        public static ThingDef Mousekin_Beeswax;
        public static ThingDef Mousekin_RawHoney;

        public static ThoughtDef Mousekin_Thought_AteCheese;

        public static TraitDef Mousekin_TraitSpectrum_Faith;

        public static BodyPartDef Mousekin_Ear;

        public static HediffDef Mousekin_ProstheticClothEar;
        public static HediffDef Mousekin_HemlockPoisoning;

        public static MentalStateDef Mousekin_MentalState_EarlessSuicide;

        public static DamageDef Mousekin_GunpowderExplosion;
        public static DamageDef Mousekin_SuicideKnife;
        public static DamageDef Mousekin_SuicidePoison;

        public static JobDef Mousekin_Job_CommitSuicideWithKnife;
        public static JobDef Mousekin_Job_CommitSuicideWithPoison;
        public static JobDef Mousekin_Job_HarvestFromBeehive;

        public static FactionDef Mousekin_FactionKingdom;

        [MayRequireIdeology]
        public static MemeDef AnimalPersonhood;

        [MayRequireIdeology]
        public static MemeDef Mousekin_IdeoMeme_AncestorWorship;

        [MayRequireIdeology]
        public static MemeDef Mousekin_IdeoMeme_RodentPurity;

        [MayRequireIdeology]
        public static PreceptDef HAR_AlienRaces_Standard;

        [MayRequireIdeology]
        public static PreceptDef HAR_AlienRaces_Respected;

        [MayRequireIdeology]
        public static PreceptDef HAR_AlienRaces_Exalted;
    }
}
