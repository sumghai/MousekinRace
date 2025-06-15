using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    public class GameComponent_Allegiance : GameComponent
    {
        public const int requestArrivalDelayDays = 2;

        public const int requestArrivalDelayTicks = requestArrivalDelayDays * GenDate.TicksPerDay;

        public const int requestTraderCooldownDays = 15; // One quadrum or season

        public const int requestTraderCooldownTicks = requestTraderCooldownDays * GenDate.TicksPerDay;

        public const int militaryAidDelayTicks = GenDate.TicksPerDay / 2; // 12 hours

        public bool seenAllegianceSysIntroLetter = false;

        public bool anyAllegianceFactionJoinReqsMet = false;

        public bool anyColonistsWithShatteredEmpireTitle = false;

        public List<Building_TownSquare> townSquares = new();

        public List<Faction> alliableFactions = new();

        public Faction alignedFaction = null;

        public int allegiancePledgedTick;

        public float availableSilver;

        public List<RecruitedPawnGroup> recruitedColonistsQueue = new();

        public int nextNewColonistArrivalTick = -99999;

        public int nextRandTraderTick = -99999;

        public int nextRequestedTraderTick = -99999;

        public int nextRequestedTraderCooldownTick = -99999;

        public float militaryAidThreatPointSize = -99999;

        public int militaryAidTargetMapID = -99999;

        public int militaryAidArrivalTick = -99999;

        public TraderKindDef nextRequestedTraderKind = null;

        public int histDemographicsPopulationTotal = 0;

        public int histDemographicsPopulationMousekin = 0;

        public int histDemographicsPopulationNonMousekinRodentkinds = 0;

        public int histDemographicsPopulationOther = 0;

        public int histDemographicsPopulationAgeAdults = 0;

        public int histDemographicsPopulationAgeChildren = 0;

        public int histDemographicsPopulationAgeBabies = 0;

        public int histDemographicsPopulationWithReligiousAffinity = 0;

        public List<ReligiousAffinityPawnCount> histDemographicsPopulationReligiousAffinity = new();

        public int histNewColonistsInvited = 0;

        public int histVisitsFromRandTraders = 0;

        public int histVisitsFromRequestedTraders = 0;

        public int histMilitaryAidRequested = 0;

        public List<AllegianceEventLogEntry> allegianceEventLog = new();

        public const int daysUntilNextRequestedCaravanTraderAllowed = GenDate.TicksPerSeason;

        public bool HasAnyTownSquares => townSquares.Count > 0;

        public bool HasDeclaredAllegiance => alignedFaction != null;

        public static GameComponent_Allegiance Instance;

        public GameComponent_Allegiance()
        {
            Instance = this;
        }

        public GameComponent_Allegiance(Game game)
        {
            Instance = this;
        }

        public void PreInit()
        {
            Instance = this;
            alliableFactions = Find.FactionManager.AllFactionsVisible.Where(f => f.def.GetModExtension<AlliableFactionExtension>() != null).ToList();
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            PreInit();
            RecacheTownSquares();
            RecacheAvailableSilver();
            seenAllegianceSysIntroLetter = false; // Reset the first time Allegiance System letter for new games
        }

        public override void LoadedGame()
        {
            base.LoadedGame();
            PreInit();
            RecacheTownSquares();
            RecacheAvailableSilver();
            if (HasDeclaredAllegiance) 
            {
                AllegianceSys_Utils.SyncRelationsWithAllegianceFaction(alignedFaction);
                AllegianceSys_Utils.RecacheDemographicsData();
            }
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            // Skip if player has no allegiance to any Mousekin faction
            if (!HasDeclaredAllegiance) 
            {
                // Check for any Mousekin factions whose joining criteria have been met
                anyAllegianceFactionJoinReqsMet = alliableFactions.Any(f => AllegianceSys_Utils.FactionJoinRequirementsMet(f));
                
                // Special use case: clergy recruited via altar
                if (!recruitedColonistsQueue.Empty() && nextNewColonistArrivalTick > 0 && Find.TickManager.TicksGame > nextNewColonistArrivalTick) 
                {
                    SpawnNextNewColonists();
                }
                
                return;
            }

            if (nextNewColonistArrivalTick > 0 && Find.TickManager.TicksGame > nextNewColonistArrivalTick) 
            {
                SpawnNextNewColonists();
            }

            if (nextRandTraderTick > 0 && Find.TickManager.TicksGame > nextRandTraderTick)
            {
                SpawnRandTrader();
            }

            if (nextRequestedTraderTick > 0 && Find.TickManager.TicksGame > nextRequestedTraderTick)
            {
                SpawnRequestedTrader();
            }

            if (nextRequestedTraderCooldownTick > 0 && Find.TickManager.TicksGame > nextRequestedTraderCooldownTick)
            { 
                ClearRequestedTraderCooldownTick();
            }

            if (militaryAidArrivalTick > 0 && Find.TickManager.TicksGame > militaryAidArrivalTick)
            { 
                SpawnMilitaryAid();
            }
        }

        public void ShowAllegianceSysIntroLetterFirstTime()
        {
            if (!seenAllegianceSysIntroLetter)
            {
                Find.LetterStack.ReceiveLetter("MousekinRace_Letter_AllegianceSysIntro".Translate(), "MousekinRace_Letter_AllegianceSysIntroDesc".Translate(), LetterDefOf.PositiveEvent);
                seenAllegianceSysIntroLetter = true; // don't show the letter in the future for the current savegame
            }

        }

        public void RecacheTownSquares()
        { 
            townSquares.Clear();
            townSquares = Find.Maps.Where(m => m.IsPlayerHome).SelectMany(m => m.listerBuildings.AllBuildingsColonistOfClass<Building_TownSquare>()).ToList();
        }

        public void RecacheAvailableSilver()
        {
            availableSilver = Find.Maps.Where(m => m.IsPlayerHome).SelectMany(m => m.listerThings.ThingsOfDef(ThingDefOf.Silver)
                                            .Where(x => !x.Position.Fogged(x.Map) && (m.areaManager.Home[x.Position] || x.IsInAnyStorage()))).Sum(t => t.stackCount);
        }

        public void AddLogEntry(AllegianceLogEntryType logType, TaggedString data)
        {
            allegianceEventLog.Add(new(logType, data));
        }

        public void AddNewColonistsToQueue(List<Pawn> pawns, int spawnTick)
        {
            recruitedColonistsQueue.Add(new RecruitedPawnGroup(pawns, spawnTick));
            if (recruitedColonistsQueue.Count == 1) 
            {
                nextNewColonistArrivalTick = spawnTick;
            }
        }

        public void SpawnNextNewColonists()
        {
            AllegianceSys_Utils.SpawnNewColonists(recruitedColonistsQueue.First().pawns);
            recruitedColonistsQueue.RemoveAt(0);
            nextNewColonistArrivalTick = (recruitedColonistsQueue.Count > 0) ? recruitedColonistsQueue.First().spawnTick : -99999;
        }

        public void SetNextRandTraderTick()
        {
            nextRandTraderTick = Find.TickManager.TicksGame + MousekinRaceMod.Settings.AllegianceSys_DaysBetweenRandomTraders * GenDate.TicksPerDay;
        }

        public void SpawnRandTrader()
        {
            AllegianceSys_Utils.SpawnTradeCaravanFromAllegianceFaction();
            SetNextRandTraderTick();
            histVisitsFromRandTraders++;
            // Record the event to the allegiance system log
            AddLogEntry(AllegianceLogEntryType.RandTraderArrival, null);
        }

        public void SetNextRequestedTraderKind(TraderKindDef traderKind)
        { 
            nextRequestedTraderKind = traderKind;
        }

        public void SetNextRequestedTraderTick()
        {
            nextRequestedTraderTick = Find.TickManager.TicksGame + requestArrivalDelayTicks;
        }

        public void SetNextRequestedTraderCooldownTick()
        {
            nextRequestedTraderCooldownTick = Find.TickManager.TicksGame + requestTraderCooldownTicks;
        }

        public void ClearRequestedTraderCooldownTick()
        {
            nextRequestedTraderCooldownTick = -99999;
        }

        public void SpawnRequestedTrader()
        { 
            AllegianceSys_Utils.SpawnTradeCaravanFromAllegianceFaction(nextRequestedTraderKind);
            // Record the event to the allegiance system log
            AddLogEntry(AllegianceLogEntryType.ReqTraderArrival, nextRequestedTraderKind.LabelCap);
            nextRequestedTraderKind = null;
            nextRequestedTraderTick = -99999;
            histVisitsFromRequestedTraders++;
        }

        public void SetMilitaryAidArrivalTick()
        {
            militaryAidTargetMapID = Find.Maps.Where(m => m.IsPlayerHome && AllegianceSys_Utils.MapHasHostiles(m)).FirstOrDefault().uniqueID;
            militaryAidThreatPointSize = StorytellerUtility.DefaultThreatPointsNow(Find.Maps.Where(m => m.IsPlayerHome).FirstOrDefault());
            militaryAidArrivalTick = Find.TickManager.TicksGame + militaryAidDelayTicks;
            histMilitaryAidRequested++;
            // Record the event to the allegiance system log
            AddLogEntry(AllegianceLogEntryType.MilitaryAid, null);
        }

        public void SpawnMilitaryAid()
        {
            AllegianceSys_Utils.SpawnMilitaryAidFromAllegianceFaction();
            militaryAidArrivalTick = -99999;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref seenAllegianceSysIntroLetter, "seenAllegianceSysIntroLetter", false, true);
            Scribe_Values.Look(ref anyColonistsWithShatteredEmpireTitle, "anyColonistsWithShatteredEmpireTitle");
            Scribe_References.Look(ref alignedFaction, "alignedFaction");
            Scribe_Values.Look(ref allegiancePledgedTick, "allegiancePledgedTick");
            Scribe_Collections.Look(ref recruitedColonistsQueue, "recruitedColonistsQueue");
            Scribe_Values.Look(ref nextNewColonistArrivalTick, "nextNewColonistArrivalTick", 0, true);
            Scribe_Values.Look(ref nextRandTraderTick, "nextRandTraderTick", 0, true);
            Scribe_Values.Look(ref nextRequestedTraderTick, "nextRequestedTraderTick", 0, true);
            Scribe_Values.Look(ref nextRequestedTraderCooldownTick, "nextRequestedTraderCooldownTick", 0, true);
            Scribe_Defs.Look(ref nextRequestedTraderKind, "nextRequestedTraderKind");
            Scribe_Values.Look(ref militaryAidThreatPointSize, "militaryAidThreatPointSize");
            Scribe_Values.Look(ref militaryAidTargetMapID, "militaryAidTargetMapID");
            Scribe_Values.Look(ref militaryAidArrivalTick, "militaryAidArrivalTick", 0, true);
            // Historical data for overview page
            Scribe_Values.Look(ref histNewColonistsInvited, "histNewColonistsInvited");
            Scribe_Values.Look(ref histVisitsFromRandTraders, "histVisitsFromRandTraders");
            Scribe_Values.Look(ref histVisitsFromRequestedTraders, "histVisitsFromRequestedTraders");
            Scribe_Values.Look(ref histMilitaryAidRequested, "histMilitaryAidRequested");
            // Log
            Scribe_Collections.Look(ref allegianceEventLog, "allegianceEventLog");

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (recruitedColonistsQueue == null)
                {
                    recruitedColonistsQueue = new();
                }
                if (allegianceEventLog == null)
                {
                    allegianceEventLog = new();
                }
            }
            Instance = this;
        }
    }

    public class RecruitedPawnGroup : IExposable
    { 
        public List<Pawn> pawns;
        public int spawnTick;

        public RecruitedPawnGroup()
        { 
        }

        public RecruitedPawnGroup(List<Pawn> pawns, int spawnTick)
        {
            this.pawns = pawns;
            this.spawnTick = spawnTick;
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref pawns, "pawns", LookMode.Reference);
            Scribe_Values.Look(ref spawnTick, "spawnTick");
        }
    }

    public class ReligiousAffinityPawnCount 
    {
        public int degree;
        public int pawnsWithTrait = 0;
        public TaggedString affinityLabel;

        public ReligiousAffinityPawnCount() 
        {
        }

        public ReligiousAffinityPawnCount(int degree, int pawnsWithTrait, TaggedString affinityLabel)
        {
            this.degree = degree;
            this.pawnsWithTrait = pawnsWithTrait;
            this.affinityLabel = affinityLabel;
        }
    }

    public class AllegianceEventLogEntry : IExposable
    {
        public int arrivalTick;
        public AllegianceLogEntryType logType;
        public TaggedString data;

        public AllegianceEventLogEntry()
        { 
        }
        
        public AllegianceEventLogEntry(AllegianceLogEntryType logType, TaggedString data)
        {
            arrivalTick = Find.TickManager.TicksAbs;
            this.logType = logType;
            this.data = data;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref arrivalTick, "arrivalTick");
            Scribe_Values.Look(ref logType, "logType", AllegianceLogEntryType.Undefined, true);
            Scribe_Values.Look(ref data, "data");
        }
    }

    public enum AllegianceLogEntryType
    { 
        Undefined,
        JoinedFaction,
        NewColonists,
        RandTraderArrival,
        ReqTraderArrival,
        MilitaryAid
    }
}
