﻿using AlienRace;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    [DefOf]
    public static class MousekinDefOf
    {
        public static ThingDef Mousekin;

        public static ThingDef Mousekin_GlassesLarge;

        public static ThingDef Mousekin_ApparelPriestCassock;
        public static ThingDef Mousekin_HatWoodsman;

        public static ThingDef Mousekin_AnimalGiantCavy;
        public static ThingDef Mousekin_AnimalPudgemouse;

        public static ThingDef Meat_Chicken;
        public static ThingDef Meat_Cow;
        public static ThingDef Meat_Deer;
        public static ThingDef Meat_Duck;
        public static ThingDef Meat_Goat;
        public static ThingDef Meat_Pig;
        public static ThingDef Meat_Sheep;

        public static ThingDef Mousekin_Beehive;
        public static ThingDef Mousekin_ChurchAltar;
        public static ThingDef Mousekin_ChurchLectern;
        public static ThingDef Mousekin_ChurchPew;
        public static ThingDef Mousekin_Windmill;
        public static ThingDef Mousekin_WorkbenchCrafting;
        public static ThingDef Mousekin_MineEntrance;
        public static ThingDef Mousekin_TownSquare;

        public static ThingDef Blueprint_Mousekin_Windmill;

        public static ThingDef Mousekin_Beeswax;
        public static ThingDef Mousekin_RawHoney;

        public static List<ThingDef> FactionRestrictedThingDefs
        {
            get 
            {
                List<ThingDef> thingDefs = new();
                foreach (Faction faction in GameComponent_Allegiance.Instance.alliableFactions)
                {
                    thingDefs.AddRange(faction.def.GetModExtension<AlliableFactionExtension>().factionRestrictedCraftableThingDefs);
                }
                return thingDefs;
            }
        }

        public static List<ResearchProjectDef> ResearchProjectsNeedingGlasses
        {
            get 
            {
                // Assumes that the first set of restricted projects we get that have apparel requirements
                // is the one that requires Mousekin_GlassesLarge
                // (we cannot directly reference a DefOf within a DefOf, as it hasn't been instantiated yet)
                return (Mousekin as ThingDef_AlienRace).alienRace.raceRestriction.researchList.FirstOrDefault(p => !p.apparelList.NullOrEmpty()).projects;
            }
        }

        public static ThoughtDef AteLavishMeal;
        public static ThoughtDef Mousekin_Thought_AteCheese;
        public static ThoughtDef Mousekin_Thought_ChurchAttendedService;
        public static ThoughtDef Mousekin_Thought_ChurchHeldService;
        public static ThoughtDef Mousekin_Thought_ChurchMissedService;
        public static ThoughtDef Humanlike_Thought_GardensDesired;

        public static TraitDef Mousekin_TraitSpectrum_Faith;

        public static BackstoryDef Mousekin_Childhood_SlaveChild;

        public static BodyPartDef Mousekin_Ear;

        public static HediffDef Mousekin_ProstheticClothEar;
        public static HediffDef Mousekin_HemlockPoisoning;

        public static InspirationDef Mousekin_Inspiration_FrenzyWork;

        public static MentalBreakDef Mousekin_MentalBreak_ExitAfterAllegianceChange;

        public static MentalStateDef Mousekin_MentalState_EarlessSuicide;
        public static MentalStateDef Mousekin_MentalState_ExitAfterAllegianceChange;

        public static DamageDef Mousekin_GunpowderExplosion;
        public static DamageDef Mousekin_SuicideKnife;
        public static DamageDef Mousekin_SuicidePoison;

        public static JobDef Mousekin_Job_ChurchServiceAttendSermon;
        public static JobDef Mousekin_Job_ChurchServiceGiveSermon;
        public static JobDef Mousekin_Job_CommitSuicideWithKnife;
        public static JobDef Mousekin_Job_CommitSuicideWithPoison;
        public static JobDef Mousekin_Job_EmptyAshesFromHearth;
        public static JobDef Mousekin_Job_HarvestFromBeehive;
        public static JobDef Mousekin_Job_MineResourcesFromMineEntrance;

        public static DutyDef Mousekin_DutyChurchServiceGiveSermon;
        public static DutyDef Mousekin_DutyChurchServiceSpectate;

        public static GatheringDef Mousekin_GatheringChurchService;

        public static PawnKindDef MousekinColonist;
        public static PawnKindDef MousekinPriest;
        public static PawnKindDef MousekinNun;
        public static PawnKindDef MousekinSlave;
        public static PawnKindDef MousekinTraderSlaver;

        public static RoomRoleDef Mousekin_RoomChurch;

        public static FactionDef Mousekin_PlayerFaction_Settlers;
        public static FactionDef Mousekin_FactionKingdom;
        public static FactionDef Mousekin_FactionIndyTown;

        public static MainButtonDef Mousekin_MainButton_Allegiance;

        public static FlowerListDef Mousekin_ValidFlowers;

        public static SoundDef Meal_Eat;

        [MayRequireRoyalty]
        public static QuestScriptDef EndGame_RoyalAscent;

        [MayRequireIdeology]
        public static DutyDef AcceptRole;

        [MayRequireIdeology]
        public static DutyDef DeliverPawnToAltar;

        [MayRequireIdeology]
        public static DutyDef DeliverPawnToCell;

        [MayRequireIdeology]
        public static DutyDef LayDownAwake;

        [MayRequireIdeology]
        public static DutyDef SpectateCircle;

        [MayRequireIdeology]
        public static MemeDef AnimalPersonhood;

        [MayRequireIdeology]
        public static MemeDef Mousekin_IdeoMeme_AncestorWorship;

        [MayRequireIdeology]
        public static MemeDef Mousekin_IdeoMeme_Raider;

        [MayRequireIdeology]
        public static MemeDef Mousekin_IdeoMeme_RodentPurity;

        [MayRequireIdeology]
        public static MemeDef HAR_Xenophilia;

        [MayRequireIdeology]
        public static PreceptDef HAR_AlienRaces_Standard;

        [MayRequireIdeology]
        public static PreceptDef HAR_AlienRaces_Respected;

        [MayRequireIdeology]
        public static PreceptDef HAR_AlienRaces_Exalted;

        [MayRequireIdeology]
        public static JobDef Mousekin_Job_EatAtBarbecueTable;

        [MayRequireIdeology]
        public static JobDef Mousekin_Job_PerformFlowerDance;

        [MayRequireIdeology]
        public static JobDef Mousekin_Job_SetHereticOnFire;

        [MayRequireIdeology]
        public static PreceptDef Mousekin_Precept_FlowersDesired;

        [MayRequireIdeology]
        public static RitualAttachableOutcomeEffectDef RandomRecruit;

        [MayRequireIdeology]
        public static RulePackDef NamerRoleMoralist_Mousekin;

        [MayRequireIdeology]
        public static RulePackDef NamerRitualFuneralMousekinKingdom;

        [MayRequireIdeology]
        public static RulePackDef NamerRitualFuneralMousekinIndyTown;

        [MayRequireIdeology]
        public static RulePackDef NamerRitualFuneralMousekinNomads;

        [MayRequireIdeology]
        public static RulePackDef NamerRitualBarbecueMousekin;

        [MayRequireIdeology]
        public static RulePackDef NamerRitualTreeFestivalMousekin;

        [MayRequireIdeology]
        public static ThingDef Mousekin_IdeoXmasTree;

        [MayRequireBiotech]
        public static PawnKindDef MousekinChild;

        [MayRequireBiotech]
        public static XenotypeDef Mousekin_XenotypeMousekin;
    }
}
