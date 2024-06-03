﻿using RimWorld;
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

        public bool anyColonistsWithShatteredEmpireTitle = false;

        public List<Building_TownSquare> townSquares = new();

        public List<Faction> alliableFactions = new();

        public Faction alignedFaction = null;

        public float availableSilver;

        public List<RecruitedPawnGroup> recruitedColonistsQueue = new();

        public int nextNewColonistArrivalTick = -99999;

        public int nextRandTraderTick = -99999;

        public int nextRequestedTraderTick = -99999;

        public int nextRequestedTraderCooldownTick = -99999;

        public int militaryAidArrivalTick = -99999;

        public TraderKindDef nextRequestedTraderKind = null;

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
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            // Skip if player has no allegiance to any Mousekin faction
            if (!HasDeclaredAllegiance) 
            {
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
            nextRequestedTraderKind = null;
            nextRequestedTraderTick = -99999;
        }

        public void SetMilitaryAidArrivalTick()
        { 
            militaryAidArrivalTick = Find.TickManager.TicksGame + militaryAidDelayTicks;
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
            Scribe_Collections.Look(ref recruitedColonistsQueue, "recruitedColonistsQueue");
            Scribe_Values.Look(ref nextNewColonistArrivalTick, "nextNewColonistArrivalTick", 0, true);
            Scribe_Values.Look(ref nextRandTraderTick, "nextRandTraderTick", 0, true);
            Scribe_Values.Look(ref nextRequestedTraderTick, "nextRequestedTraderTick", 0, true);
            Scribe_Values.Look(ref nextRequestedTraderCooldownTick, "nextRequestedTraderCooldownTick", 0, true);
            Scribe_Defs.Look(ref nextRequestedTraderKind, "nextRequestedTraderKind");
            Scribe_Values.Look(ref militaryAidArrivalTick, "militaryAidArrivalTick", 0, true);

            if (Scribe.mode == LoadSaveMode.PostLoadInit && recruitedColonistsQueue == null)
            {
                recruitedColonistsQueue = new();
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
}
