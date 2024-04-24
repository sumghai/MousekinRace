using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    public class GameComponent_Allegiance : GameComponent
    {
        public bool seenAllegianceSysIntroLetter = false;
        
        public List<Building_TownSquare> townSquares = new();

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
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            PreInit();
            RecacheTownSquares();
            seenAllegianceSysIntroLetter = false; // Reset the first time Allegiance System letter for new games
        }

        public override void LoadedGame()
        {
            base.LoadedGame();
            PreInit();
            RecacheTownSquares();
        }

        public void RecacheTownSquares()
        { 
            townSquares.Clear();
            townSquares = Find.Maps.Where(m => m.IsPlayerHome).SelectMany(m => m.listerBuildings.AllBuildingsColonistOfClass<Building_TownSquare>()).ToList();
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
            Instance = this;
        }
    }
}
