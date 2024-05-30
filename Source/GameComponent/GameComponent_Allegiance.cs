using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    public class GameComponent_Allegiance : GameComponent
    {
        public bool seenAllegianceSysIntroLetter = false;

        public bool anyColonistsWithShatteredEmpireTitle = false;
        
        public List<Building_TownSquare> townSquares = new();

        public List<Faction> alliableFactions = new();

        public Faction alignedFaction = null;

        public float availableSilver;

        public int ticksUntilNextRandomCaravanTrader = 0;

        public bool HasAnyTownSquares => townSquares.Count > 0;

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

            if (alignedFaction != null)
            {
                // Only try to recache silver every 2 seconds
                if (Find.TickManager.TicksGame % (GenTicks.TicksPerRealSecond * 2) == 0)
                {
                    RecacheAvailableSilver();
                }

                TickRandomCaravanTrader();
            }
        }

        public void RecacheTownSquares()
        { 
            townSquares.Clear();
            townSquares = Find.Maps.Where(m => m.IsPlayerHome).SelectMany(m => m.listerBuildings.AllBuildingsColonistOfClass<Building_TownSquare>()).ToList();
        }

        public void RecacheAvailableSilver()
        {
            // Only recache silver if the player has set an allegiance
            if (alignedFaction != null)
            {
                availableSilver = Find.Maps.Where(m => m.IsPlayerHome).SelectMany(m => m.listerThings.ThingsOfDef(ThingDefOf.Silver)
                                            .Where(x => !x.Position.Fogged(x.Map) && (m.areaManager.Home[x.Position] || x.IsInAnyStorage()))).Sum(t => t.stackCount);
            }
        }

        public void TickRandomCaravanTrader()
        {
            if (ticksUntilNextRandomCaravanTrader <= 0)
            {
                ticksUntilNextRandomCaravanTrader = MousekinRaceMod.Settings.AllegianceSys_DaysBetweenRandomTraders * GenDate.TicksPerDay;
                AllegianceSys_Utils.SpawnTradeCaravanFromAllegianceFaction();
            }
            else
            {
                ticksUntilNextRandomCaravanTrader--;
            }
        }
        
        public  void ShowAllegianceSysIntroLetterFirstTime()
        {
            if (!seenAllegianceSysIntroLetter)
            {
                Find.LetterStack.ReceiveLetter("MousekinRace_Letter_AllegianceSysIntro".Translate(), "MousekinRace_Letter_AllegianceSysIntroDesc".Translate(), LetterDefOf.PositiveEvent);
                seenAllegianceSysIntroLetter = true; // don't show the letter in the future for the current savegame
            }
                
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref seenAllegianceSysIntroLetter, "seenAllegianceSysIntroLetter", false, true);
            Scribe_Values.Look(ref anyColonistsWithShatteredEmpireTitle, "anyColonistsWithShatteredEmpireTitle");
            Scribe_References.Look(ref alignedFaction, "alignedFaction");
            Scribe_Values.Look(ref ticksUntilNextRandomCaravanTrader, "ticksUntilNextRandomCaravanTrader", 0);
            Instance = this;
        }
    }
}
